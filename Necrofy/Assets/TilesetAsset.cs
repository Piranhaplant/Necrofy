using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class TilesetAsset : Asset
    {
        private const string Folder = "Tilesets";

        protected class TilesetNameInfo : NameInfo
        {
            public readonly string tilesetName;
            public readonly string name;
            public readonly string extension;

            public TilesetNameInfo(string tilesetName, string name, string extension) {
                this.tilesetName = tilesetName;
                this.name = name;
                this.extension = extension;
            }

            public override string Name {
                get { return tilesetName + "/" + name; }
            }

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, tilesetName, name, extension, null);
            }

            public static TilesetNameInfo FromPath(NameInfo.PathParts parts, string extension) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder == null) return null;
                if (parts.fileExtension != extension) return null;
                if (parts.pointer != null) return null;
                return new TilesetNameInfo(parts.subFolder, parts.name, extension);
            }
        }

        protected class TilesetDefaultList : List<DefaultParams>
        {
            private readonly string extension;

            public TilesetDefaultList(string extension) {
                this.extension = extension;
            }

            public void Add(int pointer, string tilesetName, string name) {
                Add(new DefaultParams(pointer, new TilesetNameInfo(tilesetName, name, extension)));
            }
        }

        protected class TilesetFixedNameInfo : NameInfo
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

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, tilesetName, name, extension, null);
            }

            public static TilesetFixedNameInfo FromPath(NameInfo.PathParts parts, string name, string extension) {
                if (parts.topFolder != Folder) return null;
                if (parts.subFolder == null) return null;
                if (parts.name != name) return null;
                if (parts.fileExtension != extension) return null;
                if (parts.pointer != null) return null;
                return new TilesetFixedNameInfo(parts.subFolder, parts.name, extension);
            }
        }

        protected class TilesetFixedDefaultList : List<DefaultParams>
        {
            private readonly string name;
            private readonly string extension;

            public TilesetFixedDefaultList(string name, string extension) {
                this.name = name;
                this.extension = extension;
            }

            public void Add(int pointer, string tilesetName) {
                Add(new DefaultParams(pointer, new TilesetFixedNameInfo(tilesetName, name, extension)));
            }
        }
    }
}
