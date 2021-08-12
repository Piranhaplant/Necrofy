using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class Demo
    {
        public ushort level;
        public ushort character;
        public List<Input> inputs = new List<Input>();

        public Demo() { }

        public Demo(Stream s, ushort level, ushort character) {
            this.level = level;
            this.character = character;

            int length = s.ReadInt16();
            for (int i = 0; i < length; i += 4) {
                inputs.Add(new Input(s.ReadInt16(), s.ReadInt16()));
            }
        }

        public byte[] InputsToData() {
            List<byte> data = new List<byte>();
            foreach (Input input in inputs) {
                data.AddInt16(input.buttons);
                data.AddInt16(input.duration);
            }
            return data.ToArray();
        }

        public class Input
        {
            public ushort buttons;
            public ushort duration;

            public Input(ushort buttons, ushort duration) {
                this.buttons = buttons;
                this.duration = duration;
            }
        }
    }
}
