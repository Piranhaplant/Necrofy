using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy.Assets
{
    class BaseROMAsset : Asset
    {
        private const AssetCategory AssetCat = AssetCategory.ROM;

        static BaseROMAsset() {
            AddCreator(new BaseROMCreator());
        }

        private readonly BaseROMNameInfo nameInfo;
        
        private BaseROMAsset(BaseROMNameInfo nameInfo) {
            this.nameInfo = nameInfo;
        }

        public override void WriteFile(Project project) {
            throw new NotImplementedException();
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) { }

        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class BaseROMCreator : Creator
        {
            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return BaseROMNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new BaseROMAsset((BaseROMNameInfo)nameInfo);
            }
        }

        class BaseROMNameInfo : NameInfo
        {
            private static readonly string ROMName = Path.GetFileNameWithoutExtension(Project.baseROMFilename);
            private static readonly string Extension = Path.GetExtension(Project.baseROMFilename).Substring(1); // Remove the '.'
            
            public BaseROMNameInfo() : base(ROMName) { }

            public override string DisplayName => "Base ROM";
            public override AssetCategory Category => AssetCat;
            public override bool Editable => true;

            public override EditorWindow GetEditor(Project project) {
                return new HexEditor(project);
            }

            protected override PathParts GetPathParts() {
                return new PathParts("", ROMName, Extension, null, false);
            }

            public static BaseROMNameInfo FromPath(PathParts parts) {
                if (parts.fileExtension != Extension) return null;
                if (parts.name != ROMName) return null;
                if (parts.folder != "") return null;
                return new BaseROMNameInfo();
            }
        }
    }
}
