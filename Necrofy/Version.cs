using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    public struct Version : IComparable<Version>
    {
        public readonly int Major;
        public readonly int Minor;

        public Version(int major, int minor) {
            Major = major;
            Minor = minor;
        }

        public int CompareTo(Version other) {
            if (Major == other.Major) {
                return Minor.CompareTo(other.Minor);
            }
            return Major.CompareTo(other.Major);
        }

        public override bool Equals(object obj) {
            if (!(obj is Version)) {
                return false;
            }

            var version = (Version)obj;
            return Major == version.Major &&
                   Minor == version.Minor;
        }

        public override int GetHashCode() {
            var hashCode = 317314336;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + Minor.GetHashCode();
            return hashCode;
        }
    }
}
