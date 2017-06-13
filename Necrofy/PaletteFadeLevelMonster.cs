using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>
    /// Level monster that fades the level from the starting palette to a different palette (background and sprites).
    /// Also causes tourists to turn into werewolves.
    /// </summary>
    class PaletteFadeLevelMonster : LevelMonster
    {
        public const int Type = 0x12b95;

        public string bgPal { get; set; }
        public string spritePal { get; set; }

        public static void RegisterLoader() {
            LevelMonster.AddLoader(Type,
                (r, s) => {
                    s.GoToPointerPush();
                    LevelMonster m = new PaletteFadeLevelMonster(r, s);
                    s.PopPosition();
                    return m;
                },
                () => new PaletteFadeLevelMonster());
        }

        public PaletteFadeLevelMonster() { }

        public PaletteFadeLevelMonster(ROMInfo r, NStream s) : base(Type) {
            bgPal = PaletteAsset.GetAssetName(s, r, s.ReadPointer());
            spritePal = PaletteAsset.GetAssetName(s, r, s.ReadPointer());
        }

        public override void Build(MovableData data, ROMInfo rom) {
            data.data.AddPointer(type);
            MovableData paletteData = new MovableData();
            paletteData.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, bgPal));
            paletteData.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, spritePal));
            data.AddPointer(MovableData.PointerSize.FourBytes, paletteData);
        }
    }
}
