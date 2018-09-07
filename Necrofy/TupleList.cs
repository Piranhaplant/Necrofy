using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 value1, T2 value2) {
            this.Add(new Tuple<T1, T2>(value1, value2));
        }
    }
}
