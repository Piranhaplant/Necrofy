using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class DemoAsset : Asset
    {
        public const string Folder = "Demos";

        private const AssetCategory AssetCat = AssetCategory.Demo;

        static DemoAsset() {
            AddCreator(new DemoCreator());
        }

        private readonly DemoNameInfo demoNameInfo;
        public Demo demo;
        public int slot => demoNameInfo.slot;
        
        public static DemoAsset FromNameInfo(NameInfo nameInfo, Project project) {
            if (nameInfo is DemoNameInfo) {
                return (DemoAsset)new DemoCreator().FromFile(nameInfo, nameInfo.GetFilename(project.path));
            } else {
                return null;
            }
        }

        public DemoAsset(int slot, Demo demo) : this(new DemoNameInfo(slot), demo) { }

        private DemoAsset(DemoNameInfo nameInfo, Demo demo) : base(nameInfo) {
            this.demoNameInfo = nameInfo;
            this.demo = demo;
        }

        private DemoAsset(DemoNameInfo nameInfo, string filename) : base(nameInfo) {
            this.demoNameInfo = nameInfo;
            Reload(filename);
        }

        protected override void Reload(string filename) {
            demo = JsonConvert.DeserializeObject<Demo>(File.ReadAllText(filename));
        }

        protected override void WriteFile(string filename) {
            File.WriteAllText(filename, JsonConvert.SerializeObject(demo, Formatting.Indented));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            rom.Seek(ROMPointers.DemoLevelNumbers + demoNameInfo.slot * 2);
            rom.WriteInt16((ushort)(demo.level | 0x8000)); // Set high bit to indicate that the two bytes before the data contain the length
            rom.Seek(ROMPointers.DemoCharacters + demoNameInfo.slot * 2);
            rom.WriteInt16(demo.character);

            byte[] inputData = demo.InputsToData();
            int pointer = romInfo.Freespace.Claim(inputData.Length + 2);
            rom.Seek(ROMPointers.DemoReplayPointers + demoNameInfo.slot * 4);
            rom.WritePointer(pointer + 2);
            rom.Seek(pointer);
            rom.WriteInt16((ushort)inputData.Length);
            rom.Write(inputData);
        }
        
        class DemoCreator : Creator
        {
            public override NameInfo GetNameInfo(PathParts pathParts, Project project) {
                return DemoNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new DemoAsset((DemoNameInfo)nameInfo, filename);
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0, new DemoNameInfo(0), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), reserved: true),
                    new DefaultParams(1, new DemoNameInfo(1), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), reserved: true),
                    new DefaultParams(2, new DemoNameInfo(2), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), reserved: true),
                    new DefaultParams(3, new DemoNameInfo(3), extractFromNecrofyROM: true, versionAdded: new Version(2, 0), reserved: true),
                };
            }

            public override List<string> GetReservedFolders() {
                return new List<string>() { Folder };
            }

            public override Asset FromRom(NameInfo nameInfo, NStream romStream, ROMInfo romInfo, int? size, out bool trackFreespace) {
                trackFreespace = false;
                DemoNameInfo demoNameInfo = (DemoNameInfo)nameInfo;

                romStream.Seek(ROMPointers.DemoLevelNumbers + demoNameInfo.slot * 2);
                ushort level = romStream.ReadInt16();
                if ((level & 0x8000) > 0) {
                    level = (ushort)(level & 0x7fff);
                    romStream.Seek(ROMPointers.DemoCharacters + demoNameInfo.slot * 2);
                    ushort character = romStream.ReadInt16();

                    romStream.Seek(ROMPointers.DemoReplayPointers + demoNameInfo.slot * 4);
                    int pointer = romStream.ReadPointer() - 2;
                    romStream.Seek(pointer);
                    int length = romStream.PeekInt16() + 2;
                    romInfo.Freespace.AddSize(pointer, length);
                    return new DemoAsset(demoNameInfo, new Demo(romStream, level, character));
                } else {
                    // Can't extract demo data from a non-Necrofy ROM
                    return null;
                }
            }
        }

        class DemoNameInfo : NameInfo
        {
            private const string Extension = "json";
            private const string FilenamePrefix = "demo";

            public int slot { get; private set; }

            private DemoNameInfo(PathParts parts, int slot) : base(parts) {
                this.slot = slot;
            }
            public DemoNameInfo(int slot) : this(new PathParts(Folder, FilenamePrefix + slot.ToString(), Extension, null, false), slot) { }

            public override string DisplayName => "Demo" + slot.ToString();
            public override AssetCategory Category => AssetCat;

            protected override void RenamedTo(NameInfo newNameInfo) {
                slot = ((DemoNameInfo)newNameInfo).slot;
            }

            public static DemoNameInfo FromPath(PathParts parts) {
                if (parts.folder != Folder) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (parts.compressed) return null;
                if (!parts.name.StartsWith(FilenamePrefix)) return null;
                if (!int.TryParse(parts.name.Substring(FilenamePrefix.Length), out int slot)) return null;
                return new DemoNameInfo(parts, slot);
            }
        }
    }
}
