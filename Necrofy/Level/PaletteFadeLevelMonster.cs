using Newtonsoft.Json;
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
            AddLoader(Type,
                (r, s, tileset) => {
                    s.GoToPointerPush();
                    LevelMonster m = new PaletteFadeLevelMonster(r, s, tileset);
                    s.PopPosition();
                    return m;
                },
                () => new PaletteFadeLevelMonster("", ""));
        }

        [JsonConstructor]
        public PaletteFadeLevelMonster(string bgPal, string spritePal) : base(Type) {
            this.bgPal = bgPal;
            this.spritePal = spritePal;
        }

        public PaletteFadeLevelMonster(ROMInfo r, NStream s, string tileset) : base(Type) {
            int bgPalPointer = s.ReadPointer();
            int spritePalPointer = s.ReadPointer();

            s.TogglePauseBlock();
            bgPal = PaletteAsset.GetAssetName(s, r, bgPalPointer, tileset);
            spritePal = PaletteAsset.GetAssetName(s, r, spritePalPointer);
            s.TogglePauseBlock();
        }

        public override void Build(MovableData data, ROMInfo rom) {
            data.data.AddPointer(type);
            MovableData paletteData = new MovableData();
            if (rom == null) {
                paletteData.data.AddPointer(0);
                paletteData.data.AddPointer(0);
            } else {
                paletteData.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, bgPal));
                paletteData.data.AddPointer(rom.GetAssetPointer(AssetCategory.Palette, spritePal));
            }
            data.AddPointer(MovableData.PointerSize.FourBytes, paletteData);
        }
    }
}
