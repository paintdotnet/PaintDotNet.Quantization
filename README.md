# PaintDotNet.Quantization
**NOTE:** This is still pre-first draft! I'll update this to say final draft when it's all finished 🙂

This repository has the image quantization code from Paint.NET, along with the supporting utility and helper classes that are used along the way. Since images are always stored in-memory at 32-bit color depth (BGRA), quantization is necessary in order to permit saving images at 8-bit (or less) color depths. You can also use the Quantize effect (added in 4.2.16) to do this in-place (the image is still 32-bit BGRA, albeit using only up to 256 unique colors).

Please note that this is NOT intended to be used as a library or "nuget package" that you drop into your app and use as-is. You *can* do that (as per the license), but the code in this repository is meant for *integration*, not *linking.* As such, don't expect the public API surface to be beautiful or amazing or anything. Things like `IBitmapSource` and `IImagingFactory`, in particular, are mere shadows of their counterparts in the full Paint.NET source code.

## Background

Paint.NET's quantization code is based on an old MSDN article from 2003, _Optimizing Color Quantization for ASP.NET Images_ (https://docs.microsoft.com/en-us/previous-versions/dotnet/articles/aa479306(v=msdn.10)). The code in the article has made its way into a number of projects besides Paint.NET, including ImageSharp (https://github.com/SixLabors/ImageSharp). 

Unfortunately, it has a few bugs and quirks that this repository has fixes for. I recently dove in and completely gutted this code, making fixes and improvements, and I thought it would be useful to share this with everyone else. I've also optimized performance quite a lot, especially the code that maps from colors to palette entries (see section 7 below).

So, here are the changes I've made, in order from simple to crazy:

### 1) Right Shift Bug

The first fix is an easy one. The MSDN code has a `GetColorIndex` method whose job is to provide the child node index for navigating down the octree. However, it has what is essentially a precision bug that causes colors to land in the wrong leaf nodes:

```
private static int GetColorIndex(ref Rgba32 color, int level)
{
    DebugGuard.MustBeLessThan(level, Mask.Length, nameof(level));

    int shift = 7 - level;
    ref byte maskRef = ref MemoryMarshal.GetReference(Mask);
    byte mask = Unsafe.Add(ref maskRef, level);
    
    return ((color.R & mask) >> shift)
            | ((color.G & mask) >> (shift - 1))
            | ((color.B & mask) >> (shift - 2));
}
```

*(I'm using the code from ImageSharp, btw, not the MSDN article.)*

The problem is that when `level` is large, `shift` will be small, and `(shift - 1)` and `(shift - 2)` will be negative. The intention is to do a shift-left operation, and maybe that's how it works in C, but definitely not in C#. The result is zero.

The fix is to only shift-right by `shift`, and then do a shift-left by 1 or 2:

```
private static int GetColorIndex(ref Rgba32 color, int level)
{
    DebugGuard.MustBeLessThan(level, Mask.Length, nameof(level));

    int shift = 7 - level;
    ref byte maskRef = ref MemoryMarshal.GetReference(Mask);
    byte mask = Unsafe.Add(ref maskRef, level);
    
    return ((color.R & mask) >> shift)
            | (((color.G & mask) >> shift) << 1)
            | (((color.B & mask) >> shift) << 2);
}
```

This code will now produce the correct output. Be warned, however, that memory usage will now be higher since there will be substantially more leaf nodes. Reducing the octree and generating the palette will also be slower.

This is not just a theoretical quality improvement, by the way. I saw images being reduced to 64 colors instead of 256! I believe it was an image of a black-to-red gradient that caused this to happen, which should have fit nicely into 256 colors.

### 2) Unnecessary lookup table
There's also some silliness with regard to the use of a lookup table for calculating the mask in `GetColorIndex`:

```
/// <summary>
/// Gets the mask used when getting the appropriate pixels for a given node.
/// </summary>
private static ReadOnlySpan<byte> Mask => new byte[]
{
    0b10000000,
    0b1000000,
    0b100000,
    0b10000,
    0b1000,
    0b100,
    0b10,
    0b1
};
```
I'm guessing that, like most people, when I integrated this code into my project I thought it was a bunch of complicated wizardry that I didn't have the patience to read through and fully comprehend. However, upon finally doing this thorough inspection, it's obvious that this is a lookup table for `(1 << (7-i))`, which definitely doesn't need a lookup table.

So, kill the `Mask` and just compute the value when it's needed. We can even use `shift` because it already equals `(1 - level)`:

```
private static int GetColorIndex(ref Rgba32 color, int level)
{
    int shift = 7 - level;
    byte mask = (byte)(1 << shift)
    
    return ((color.R & mask) >> shift)
            | (((color.G & mask) >> shift) << 1))
            | (((color.B & mask) >> shift) << 2));
}
```

This will save the static field access, pointer math, and memory dereference.

### 3) Octree.AddColor() is slow

`Octree.AddColor()` is called once for every pixel in the image. The problem is, it's slow. It's not poorly written, it just requires a lot of jumping around to get its job done. When you have millions or even billions of pixels in an image, it gets really bad.

I found it was much faster to pre-process the image and create a color histogram, essentially a `[color, count]` list, and amend `AddColor()` to take the count value. Then, each color is only sent down the octree once and performance is much better. Generating the histogram is also easily parallelizable, greatly improving performance on higher core count systems.

Be sure to multiply the `red`, `green`, and `blue` values by the `count` if you take this approach. `red = color.R * count`, in other words, and set `pixelCount` to `count`.

In this repo, look for the `ColorHistogram` class for my implementation. I support full 64-bit counts, as Paint.NET is intended to work with very large images, so storing the histogram efficiently is important: since there are 2^24 maximum RGB colors, and a `[colorBGR, long]` tuple would take 11 bytes, that would mean about 176 MiB for an image that uses all colors. Instead, I store 3 separate lists: one for colors that only show up once (no need to store count), another list that uses 32-bit `uint`s, and a final list that uses 64-bit `long`s. Enumeration for the client is still homogenous, it's just `IEnumerable<Entry>`, where `Entry` is a struct with a `ColorBgr24` and a `long` for the count.

More memory is required for this approach, but it's temporary, and is still quite a bit less than the `OctreeNode`s (see section 6). It is probably possible to improve this, possibly by destroying/trimming the histogram while enumerating it, but I didn't think it was worth pursuing.

### 4) Exact palette size is not always achieved

A problem with using the standard Octree quantization code is that the `Reduce()` method, on a per-node basis, is all or nothing. If you have reduced the octree down to 260 leaf nodes, and you're reducing a node that has 8 children, the color count will be reduced by 7. You'll end up with 253 colors instead of 256. This is *very* common in practice.

I found a CodeProject article by someone who figured out a way to fix this: https://www.codeproject.com/Articles/109133/Octree-Color-Palette. Search for "Merging for Exact Colors Count".

The code here in this repo doesn't use this approach because the next section fixes both this and another problem. However, if you're looking to ease into all of these fixes one-at-a-time, you don't need to skip this one before tackling the next section (which is much more complicated to implement).

### 5) Colors are unevenly reduced

The MSDN code makes use of a `reducibleNodes` array that is built while populating the Octree. It's essentially a 2D jagged list of all the nodes in the octree: the first index is the level, with the second index being an unordered (in principal anyway) list of the nodes at that level. *(Note that it is not actually a 2D jagged list! It's an array of references to the first node in the list, and then the nodes themselves form a linked list by way of their `NextReducible` property. Yuck!)*

In practice, however, the list's ordering is important to the overall quality of the palette. The second level of that list is filled in as you call `AddColor()`, which means that the final stages of the reduction process will prefer to merge colors that were added last; those that first appeared toward the bottom of the image, in other words, assuming the image is processed top-to-bottom.

To fix this, I changed the reduction algorithm somewhat. See `OctreeQuantizer::Octree::Reduce()` for the implementation.

I removed the `reducibleNodes` list and `NextReducible` property, opting instead to traverse the octree when I need to count or gather nodes (the alternative was extra bookkeeping fields and it got very complicated/buggy). The octree is, as usual, reduced from bottom to top. However, after each level is completely reduced, I look at the number of nodes in the last and last-1 levels. Once the last level has more nodes than the target color count, but the last-1 level has less than or equal, I break out of the loop and switch to the final stage of reduction.

In the final stage of reduction, all of the leaf nodes are gathered into a list and sorted by their weight (`pixelCount`). Nodes with the lowest weight, which store information about colors that were seen less often in the image, are reduced first. Because many nodes could have the same weight, tie-breaking for the sort order is done by comparing the hash code of the node's output color at high precision (32-bit floating point per component). This is done via the `ColorRgb96Float` struct in this repo's code.

Using the hash code achieves a pseudo-random balancing to the order that nodes are reduced. I contemplated other balancing methods, but ultimately nothing was satisfactory. Using the hash code has some benefits, especially around being straightforwardly portable to other platforms, languages, compilers, and not being dependent on an opaque random number generator.

I'm not fully convinced that reducing low-weight nodes first is the best approach, nor that using the color's hash code for tie-breaking the sort order is. However, it's a simple approach that is easy to change and experiment with, and it does produce good looking results.

**Note** that there is another change you must understand for this section. Nodes in an Octree are usually classified as just interior and leaf, with leaf nodes emitting colors after the octree is reduced and the palette is finally built. However, we must introduce an extra flag here: does the node have color information? Rather, can it produce a color? (normally the answer is yes for leaves, no for interior nodes) Because we are partially reducing nodes during the final stage of reduction, we will have non-leaf nodes that store color information about their children that were reduced, even if some children are still around. We are not reducing all-at-once, in other words. These nodes will emit a color when building the palette, as will their (leaf) children. This is encapsulated by the `OctreeNode.HasColorInfo` property.

### 6) Leaf nodes take up a ton of memory
If you're using 32-bit integers to store color sums and counts, like the MSDN article does, your `OctreeNode` will consume 40 bytes plus object/allocation overhead. If you're using 64-bit integers for sums and counts, this goes way up to around 56 bytes. If the image being processed has 500,000 unique colors, the total here is 20MB for 32-bit and 28MB for 64-bit. For a "worst" case image, with all 2^24 RGB colors, this balloons to 671MB and 939MB.

This can be ameliorated by using inheritance and polymorphism (virtual methods/properties, i.o.w.). In my `Octree` code, `OctreeNode` is a base class that does not store these values. Instead, derived classes with specializations for different tiers of color counts are used. See the code for more details. The code quality here is not as good as I'd like it to be, but it does work and isn't too complicated. It can also be optimized further with more `OctreeNode`-derived class that specialize different cases, and I experimented with this, but the complexity grew very quickly.

(Other approaches to solving this are also possible, such as using structs, pool allocators, and other clever ways of keeping track of sums and counts.)

### 7) Mapping colors to the palette is REALLY slow
The previous sections went into detail fixing and optimizing the palette generation process. Once you have a palette, which is just an arbitrarily ordered <=256 length array of `ColorBgr24`s, you need to do something with it by applying it to the image itself.

TODO: Write this up. For now, there's a summary (w/o pictures sadly) over on Twitter: https://twitter.com/rickbrewPDN/status/1379238853832155136