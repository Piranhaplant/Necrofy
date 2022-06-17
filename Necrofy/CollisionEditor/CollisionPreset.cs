using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class CollisionPreset
    {
        public List<Preset> collisions = new List<Preset>();

        public class Preset
        {
            public readonly ushort collision;
            public readonly string name;

            public Preset(ushort collision, string name) {
                this.collision = collision;
                this.name = name;
            }

            public override bool Equals(object obj) {
                var preset = obj as Preset;
                return preset != null &&
                       collision == preset.collision;
            }

            public override int GetHashCode() {
                return -462773017 + collision.GetHashCode();
            }

            public override string ToString() {
                return name;
            }
        }
    }
}
