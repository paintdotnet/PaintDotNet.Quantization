using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class HashCodeUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15, int hash16)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            result = unchecked(((result << 5) + result) ^ hash16);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15, int hash16, int hash17)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            result = unchecked(((result << 5) + result) ^ hash16);
            result = unchecked(((result << 5) + result) ^ hash17);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15, int hash16, int hash17, int hash18)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            result = unchecked(((result << 5) + result) ^ hash16);
            result = unchecked(((result << 5) + result) ^ hash17);
            result = unchecked(((result << 5) + result) ^ hash18);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15, int hash16, int hash17, int hash18, int hash19)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            result = unchecked(((result << 5) + result) ^ hash16);
            result = unchecked(((result << 5) + result) ^ hash17);
            result = unchecked(((result << 5) + result) ^ hash18);
            result = unchecked(((result << 5) + result) ^ hash19);
            return result;
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9, int hash10, int hash11, int hash12, int hash13, int hash14, int hash15, int hash16, int hash17, int hash18, int hash19, int hash20)
        {
            int result = hash1;
            result = unchecked(((result << 5) + result) ^ hash2);
            result = unchecked(((result << 5) + result) ^ hash3);
            result = unchecked(((result << 5) + result) ^ hash4);
            result = unchecked(((result << 5) + result) ^ hash5);
            result = unchecked(((result << 5) + result) ^ hash6);
            result = unchecked(((result << 5) + result) ^ hash7);
            result = unchecked(((result << 5) + result) ^ hash8);
            result = unchecked(((result << 5) + result) ^ hash9);
            result = unchecked(((result << 5) + result) ^ hash10);
            result = unchecked(((result << 5) + result) ^ hash11);
            result = unchecked(((result << 5) + result) ^ hash12);
            result = unchecked(((result << 5) + result) ^ hash13);
            result = unchecked(((result << 5) + result) ^ hash14);
            result = unchecked(((result << 5) + result) ^ hash15);
            result = unchecked(((result << 5) + result) ^ hash16);
            result = unchecked(((result << 5) + result) ^ hash17);
            result = unchecked(((result << 5) + result) ^ hash18);
            result = unchecked(((result << 5) + result) ^ hash19);
            result = unchecked(((result << 5) + result) ^ hash20);
            return result;
        }
    }
}
