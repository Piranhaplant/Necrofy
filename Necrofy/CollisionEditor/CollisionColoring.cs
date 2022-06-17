using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class CollisionColoring
    {
        private ushort bitmask;
        private int value;
        private int weight = 1;
        private List<CollisionColoring> children;

        public CollisionColoring(params CollisionColoring[] children) : this(0, 0, children) { }

        public CollisionColoring(ushort bitmask) : this(bitmask, -1) { }

        public CollisionColoring(ushort bitmask, int value, params CollisionColoring[] children) {
            this.bitmask = bitmask;
            this.value = value;
            this.children = new List<CollisionColoring>(children);
        }

        public CollisionColoring Weight(int weight) {
            this.weight = weight;
            return this;
        }

        public void GetColors(ushort collision, out double main, out double sub) {
            GetColors(collision, out main, out sub, 0);
        }

        private void GetColors(ushort collision, out double main, out double sub, ushort cumulativeMask) {
            main = 0.0;
            sub = 0.0;

            if (children.Count == 0) {
                if (value == -1) {
                    main = GetMaskedRatio(collision, bitmask);
                } else {
                    main = 1.0;
                }
                sub = GetMaskedRatio(collision, ~cumulativeMask);
            } else {
                int totalWeight = children.Sum(c => c.weight);
                int curWeightSum = 0;

                for (int i = 0; i < children.Count; i++) {
                    CollisionColoring child = children[i];
                    cumulativeMask |= child.bitmask;
                    if ((collision & child.bitmask) == child.value || child.value == -1) {
                        child.GetColors(collision, out double childMain, out sub, cumulativeMask);
                        main = (childMain * child.weight + curWeightSum) / totalWeight;
                        break;
                    }
                    curWeightSum += child.weight;
                }
            }
        }

        private static double GetMaskedRatio(int value, int mask) {
            int bitCount = 0;
            int result = 0;

            for (int i = 0; i < 16; i++) {
                if ((mask & 1) > 0) {
                    result <<= 1;
                    result |= (value & 1);
                    bitCount += 1;
                }
                mask >>= 1;
                value >>= 1;
            }

            return (result + 1) / (double)(1 << bitCount);
        }
    }
}
