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
        private const AssetCategory AssetCat = AssetCategory.Demo;

        static DemoAsset() {
            AddCreator(new DemoCreator());
        }

        private readonly DemoNameInfo nameInfo;
        public readonly Demo demo;
        public int slot => nameInfo.slot;
        
        public static DemoAsset FromNameInfo(NameInfo nameInfo, Project project) {
            if (nameInfo is DemoNameInfo) {
                return (DemoAsset)new DemoCreator().FromFile(nameInfo, nameInfo.GetFilename(project.path));
            } else {
                return null;
            }
        }

        public DemoAsset(int slot, Demo demo) : this(new DemoNameInfo(slot), demo) { }

        private DemoAsset(DemoNameInfo nameInfo, Demo demo) {
            this.nameInfo = nameInfo;
            this.demo = demo;
        }
        
        public override void WriteFile(Project project) {
            File.WriteAllText(nameInfo.GetFilename(project.path, createDirectories: true), JsonConvert.SerializeObject(demo, Formatting.Indented));
        }

        public override void Insert(NStream rom, ROMInfo romInfo, Project project) {
            rom.Seek(ROMPointers.DemoLevelNumbers + nameInfo.slot * 2);
            rom.WriteInt16((ushort)(demo.level | 0x8000)); // Set high bit to indicate that the two bytes before the data contain the length
            rom.Seek(ROMPointers.DemoCharacters + nameInfo.slot * 2);
            rom.WriteInt16(demo.character);

            byte[] inputData = demo.InputsToData();
            int pointer = romInfo.Freespace.Claim(inputData.Length + 2);
            rom.Seek(ROMPointers.DemoReplayPointers + nameInfo.slot * 4);
            rom.WritePointer(pointer + 2);
            rom.Seek(pointer);
            rom.WriteInt16((ushort)inputData.Length);
            rom.Write(inputData);
        }
        
        protected override AssetCategory Category => nameInfo.Category;
        protected override string Name => nameInfo.Name;

        class DemoCreator : Creator
        {
            public override NameInfo GetNameInfo(NameInfo.PathParts pathParts, Project project) {
                return DemoNameInfo.FromPath(pathParts);
            }

            public override Asset FromFile(NameInfo nameInfo, string filename) {
                return new DemoAsset((DemoNameInfo)nameInfo, JsonConvert.DeserializeObject<Demo>(File.ReadAllText(filename)));
            }

            public override List<DefaultParams> GetDefaults() {
                return new List<DefaultParams>() {
                    new DefaultParams(0, new DemoNameInfo(0), extractFromNecrofyROM: true),
                    new DefaultParams(1, new DemoNameInfo(1), extractFromNecrofyROM: true),
                    new DefaultParams(2, new DemoNameInfo(2), extractFromNecrofyROM: true),
                    new DefaultParams(3, new DemoNameInfo(3), extractFromNecrofyROM: true),
                };
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
            private const string Folder = "Demos";
            private const string Extension = "json";
            private const string FilenamePrefix = "demo";

            public readonly int slot;

            public DemoNameInfo(int slot) : base(slot.ToString()) {
                this.slot = slot;
            }

            public override string DisplayName => "Demo " + slot.ToString();
            public override AssetCategory Category => AssetCat;

            protected override PathParts GetPathParts() {
                return new PathParts(Folder, FilenamePrefix + slot.ToString(), Extension, null, false);
            }
            
            public static DemoNameInfo FromPath(PathParts parts) {
                if (parts.folder != Folder) return null;
                if (parts.fileExtension != Extension) return null;
                if (parts.pointer != null) return null;
                if (!parts.name.StartsWith(FilenamePrefix)) return null;
                if (!int.TryParse(parts.name.Substring(FilenamePrefix.Length), out int slot)) return null;
                return new DemoNameInfo(slot);
            }
        }
    }
}
