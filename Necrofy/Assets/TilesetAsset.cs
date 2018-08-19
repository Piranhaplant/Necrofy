using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Necrofy
{
    abstract class TilesetAsset : Asset
    {
        private const string Folder = "Tilesets";
        private const string NameSeparator = "/";

        protected const string Castle = "Castle";
        protected const string Grass = "Grass";
        protected const string Desert = "Desert";
        protected const string Office = "Office";
        protected const string Mall = "Mall";

        protected abstract class TilesetNameInfo : NameInfo
        {
            public readonly string tilesetName;
            public readonly string name;
            public readonly string extension;

            public TilesetNameInfo(string fullName, string extension) {
                int separatorPos = fullName.IndexOf(NameSeparator);
                this.tilesetName = fullName.Substring(0, separatorPos);
                this.name = fullName.Substring(separatorPos + 1);
                this.extension = extension;
            }

            public TilesetNameInfo(string tilesetName, string name, string extension) {
                this.tilesetName = tilesetName;
                this.name = name;
                this.extension = extension;
            }

            public override string Name {
                get { return tilesetName + NameSeparator + name; }
            }

            public override string DisplayName {
                get { return name; }
            }

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, tilesetName, name, extension, null);
            }

            public static TilesetNameInfo FromPath(NameInfo.PathParts parts, string extension, Func<string, string, TilesetNameInfo> constructor) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder == null) return null;
                if (parts.fileExtension != extension) return null;
                if (parts.pointer != null) return null;
                return constructor.Invoke(parts.subFolder, parts.name);
            }
        }

        protected abstract class TilesetFixedNameInfo : NameInfo
        {
            public readonly string tilesetName;
            public readonly string name;
            public readonly string extension;

            public TilesetFixedNameInfo(string tilesetName, string name, string extension) {
                this.tilesetName = tilesetName;
                this.name = name;
                this.extension = extension;
            }

            public override string Name {
                get { return tilesetName; }
            }

            public override string DisplayName {
                get { return name; }
            }

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, tilesetName, name, extension, null);
            }

            public static TilesetFixedNameInfo FromPath(NameInfo.PathParts parts, string name, string extension, Func<string, TilesetFixedNameInfo> constructor) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder == null) return null;
                if (parts.name != name) return null;
                if (parts.fileExtension != extension) return null;
                if (parts.pointer != null) return null;
                return constructor.Invoke(parts.subFolder);
            }
        }
    }
}
