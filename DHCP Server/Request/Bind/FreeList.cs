using PIXIS.DHCP.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace PIXIS.DHCP.Request.Bind
{
    public class FreeList
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected BigInteger _start;
        protected BigInteger _end;
        private readonly static object _lock = new object();
        /** 
	 * The map of ranges, which are keyed by an index into a list of ranges, 
	 * each offset by the maximum size of an integer. For example: 
	 * index=0, for range of values from start to start+2147483647 
	 * index=1, for range of values from start+2147483648 to 4294967295 
	 * index=2, for range of values from start+4294967296 to 6442450943 
	 * ... 
	 */
        protected List<BitSet> _bitsetRanges;
        protected BitArray bitArrays;

        protected int _nextFreeIndex;
        //// the ReentrantLock class is better than synchronized
        //private final ReentrantLock lock = new ReentrantLock();
        /**
         * Instantiates a new free list.
         * 
         * @param start the range start
         * @param end the range end
         */
        public FreeList(BigInteger start, BigInteger end)
        {
            this._start = start;
            this._end = end;
            if (end.CompareTo(start) >= 0)
            {
                _bitsetRanges = new List<BitSet>();
                _bitsetRanges.Add(new BitSet()); // create one to start
            }
            else
            {
                throw new Exception("Failed to create FreeList: end < start");
            }
        }


        ///**
        // * Gets the index into the list for the given BigInteger
        // * 
        // * @param bi the bi
        // * 
        // * @return the index position in the list
        // */
        protected int GetIndex(BigInteger bi)
        {
            var mod = (bi - _start);
            return (mod / new BigInteger(int.MaxValue)).IntValue();
        }

        ///**
        // * Gets the offset into the BitSet for the given BigInteger
        // * 
        // * @param bi the bi
        // * 
        // * @return the offset
        // */
        protected int GetOffset(BigInteger bi)
        {
            var mod = (bi - _start);
            return mod.modPow(mod, new BigInteger(int.MaxValue)).IntValue();
        }

        /**
         * Sets the.
         * 
         * @param bi the bi
         * @param used the used
         */
        protected void Set(BigInteger bi, bool used)
        {
            Monitor.Enter(_lock);
            try
            {
                if (IsInList(bi))
                {
                    int offset = GetOffset(bi);
                    BitSet bitset = null;
                    int ndx = GetIndex(bi);
                    if (ndx < _bitsetRanges.Count)
                    {
                        bitset = _bitsetRanges[ndx];
                    }
                    else
                    {
                        while (ndx >= _bitsetRanges.Count)
                        {
                            bitset = new BitSet();
                            _bitsetRanges.Add(bitset);
                        }
                    }
                    if (used)
                    {
                        bitset.Set(offset);
                    }
                    else
                    {
                        bitset.Clear(offset);
                        if (ndx < _nextFreeIndex)
                        {
                            _nextFreeIndex = ndx;    // reset next free search index
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        ///**
        // * Sets the used.
        // * 
        // * @param used the new used
        // */
        public void SetUsed(BigInteger used)
        {
            this.Set(used, true);
        }

        ///**
        // * Sets the free.
        // * 
        // * @param free the new free
        // */
        public void SetFree(BigInteger free)
        {
            this.Set(free, false);
        }

        /// <summary>
        /// Checks if is used.
        /// </summary>
        /// <param name="used">used the used</param>
        /// <returns>true, if is used</returns>
        public bool IsUsed(BigInteger used)
        {
            if (IsInList(used))
            {
                int ndx = GetIndex(used);
                if (ndx < _bitsetRanges.Count)
                {
                    BitSet bitset = _bitsetRanges[ndx];
                    if (bitset != null)
                    {
                        return bitset.Get(GetOffset(used));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if is free.
        /// </summary>
        /// <param name="free">free the free</param>
        /// <returns>true, if is free</returns>
        public bool IsFree(BigInteger free)
        {
            return !IsUsed(free);
        }

        /**
         * Gets the next free.
         * 
         * @return the next free
         */
        public BigInteger GetNextFree()
        {
            Monitor.Enter(_lock);
            try
            {   // start = 42544061674327494850681284986417512449
                // nextFreeIndex = 0 
                // MAX_VALUE = 2147483647
                //BigInteger next = start.add(BigInteger.valueOf(nextFreeIndex).
                //                            multiply(BigInteger.valueOf(Integer.MAX_VALUE)));
                BigInteger next = _start + (new BigInteger(_nextFreeIndex) * new BigInteger(int.MaxValue));
                int clearBit = -1;

                //bitsetRanges
                //[{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
                //22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
                //42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 
                //63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154}]

                BitSet bitset = _bitsetRanges[_nextFreeIndex];
                clearBit = bitset.NextClearBit(0);
                if (clearBit >= 0)
                {
                    next = next + new BigInteger(clearBit);
                    if (IsInList(next))
                    {
                        bitset.Set(clearBit);
                        return next;
                    }
                }
                else
                {
                    // no more available in the last BitSet, so the next available
                    // would be the first in the next BitSet, so add max offset
                    next = next + new BigInteger(int.MaxValue);
                    if (IsInList(next))
                    {
                        _nextFreeIndex++;
                        bitset = new BitSet();
                        bitset.Set(0);
                        _bitsetRanges.Insert(_nextFreeIndex, bitset);
                    }
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return new BigInteger(0);
        }
        public bool IsInList(BigInteger bi)
        {
            if ((bi.CompareTo(_start) >= 0) && (bi.CompareTo(_end) <= 0))
            {
                return true;
            }
            return false;
        }
    }
}