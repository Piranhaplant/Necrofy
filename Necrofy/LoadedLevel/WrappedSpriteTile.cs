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
        public const string XFlipProperty = "Flip Horizontal";
        public const string YFlipProperty = "Flip Vertical";

        public readonly Sprite.Tile tile;

        public WrappedSpriteTile(Sprite.Tile tile) {
            this.tile = tile;
        }
        
        [Browsable(false)]
        public Rectangle Bounds => new Rectangle(X, Y, 16, 16);
        int ISelectableObject.GetX() {
            return X;
        }
        int ISelectableObject.GetY() {
            return Y;
        }
        bool ISelectableObject.Selectable => true;

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
        private bool? browsableXFlip = null;
        private bool? browsableYFlip = null;
        public void ClearBrowsableProperties() {
            browsableX = null;
            browsableY = null;
            browsablePalette = null;
            browsableTileNum = null;
            browsableXFlip = null;
            browsableYFlip = null;
        }
        [DisplayName(XProperty)]
        public string BrowsableX { get => browsableX ?? X.ToString(); set => browsableX = value; }
        [DisplayName(YProperty)]
        public string BrowsableY { get => browsableY ?? Y.ToString(); set => browsableY = value; }
        [DisplayName(PaletteProperty)]
        public string BrowsablePalette { get => browsablePalette ?? tile.palette.ToString(); set => browsablePalette = value; }
        [DisplayName(TileNumProperty)]
        public string BrowsableTileNum { get => browsableTileNum ?? tile.tileNum.ToString(); set => browsableTileNum = value; }
        [DisplayName(XFlipProperty)]
        public bool BrowsableXFlip { get => browsableXFlip ?? tile.xFlip; set => browsableXFlip = value; }
        [DisplayName(YFlipProperty)]
        public bool BrowsableYFlip { get => browsableYFlip ?? tile.yFlip; set => browsableYFlip = value; }

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
