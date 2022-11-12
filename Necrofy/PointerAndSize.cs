using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    struct PointerAndSize
    {
        public int Pointer { get; set; }
        public int Size { get; set; }

        public PointerAndSize(int pointer, int size) {
            Pointer = pointer;
            Size = size;
        }
    }
}
