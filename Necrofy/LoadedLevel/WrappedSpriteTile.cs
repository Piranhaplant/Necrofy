using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    class WrappedSpriteTile : ISelectableObject
    {
        public const string XProperty = "X";
        public const string YProperty = "Y";
        public const string PaletteProperty = "Palette";
        public const string TileNumProperty = "TileNum";

        public readonly Sprite.Tile tile;

        public WrappedSpriteTile(Sprite.Tile tile) {
            this.tile = tile;
        }
        
        [Browsable(false)]
        public Rectangle Bounds => new Rectangle(X, Y, 16, 16);
        public int GetX() {
            return X;
        }
        public int GetY() {
            return Y;
        }

        [Browsable(false)]
        public short X { get => tile.xOffset; set => tile.xOffset = value; }
        [Browsable(false)]
        public short Y { get => tile.yOffset; set => tile.yOffset = value; }
        [Browsable(false)]
        public int Palette { get => tile.palette; set => tile.palette = value; }
        [Browsable(false)]
        public ushort TileNum { get => tile.tileNum; set => tile.tileNum = value; }

        // Properties used in the property browser
        private string browsableX = null;
        private string browsableY = null;
        private string browsablePalette = null;
        private string browsableTileNum = null;
        public void ClearBrowsableProperties() {
            browsableX = null;
            browsableY = null;
            browsablePalette = null;
            browsableTileNum = null;
        }
        [DisplayName(XProperty)]
        public string BrowsableX { get => browsableX ?? X.ToString(); set => browsableX = value; }
        [DisplayName(YProperty)]
        public string BrowsableY { get => browsableY ?? Y.ToString(); set => browsableY = value; }
        [DisplayName(PaletteProperty)]
        public string BrowsablePalette { get => browsablePalette ?? tile.palette.ToString(); set => browsablePalette = value; }
        [DisplayName(TileNumProperty)]
        public string BrowsableTileNum { get => browsableTileNum ?? tile.tileNum.ToString(); set => browsableTileNum = value; }

        public override int GetHashCode() {
            return tile.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is WrappedSpriteTile other) {
                return tile.Equals(other.tile);
            }
            return base.Equals(obj);
        }
    }
}
