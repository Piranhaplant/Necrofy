using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    public abstract class Hinting
    {
        public abstract void Render(Graphics g, RectangleF visibleBounds, float zoom);

        private static readonly Dictionary<Type, Hinting> map = new Dictionary<Type, Hinting>() {
            { Type.None, new NoneHinting() },
            { Type.LevelTitle, new LevelTitleHinting() },
            { Type.Tileset, new TilesetHinting() },
        };

        public static Hinting ForType(Type type) {
            if (map.TryGetValue(type, out Hinting value)) {
                return value;
            }
            return map[Type.None];
        }

        public enum Type
        {
            None,
            LevelTitle,
            Tileset,
        }
    }

    class NoneHinting : Hinting
    {
        public override void Render(Graphics g, RectangleF visibleBounds, float zoom) { }
    }

    class LevelTitleHinting : Hinting
    {
        private const float RowWidth = 128 * 8;
        private const float RowHeight = 6 * 8;

        public override void Render(Graphics g, RectangleF visibleBounds, float zoom) {
            using (Pen p = new Pen(Color.White, 2 / zoom)) {
                RectangleF wholeArea = new RectangleF(0, 0, RowWidth, RowHeight * 3);
                wholeArea.Intersect(visibleBounds);
                MapEditor.DrawGrid(g, p, wholeArea, RowWidth, RowHeight);

                RectangleF row1 = new RectangleF(0, 0, RowWidth, RowHeight);
                row1.Intersect(visibleBounds);
                MapEditor.DrawGrid(g, p, row1, 3 * 8, row1.Height);

                RectangleF row2 = new RectangleF(0, RowHeight, RowWidth, RowHeight);
                row2.Intersect(visibleBounds);
                MapEditor.DrawGrid(g, p, row2, 2 * 8, row2.Height);

                RectangleF row3 = new RectangleF(0, RowHeight * 2, RowWidth, RowHeight);
                row3.Intersect(visibleBounds);
                MapEditor.DrawGrid(g, p, row3, 6 * 8, row3.Height);
            }
        }
    }

    class TilesetHinting : Hinting
    {
        public override void Render(Graphics g, RectangleF visibleBounds, float zoom) {
            using (Pen p = new Pen(Color.White, 2 / zoom)) {
                MapEditor.DrawGrid(g, p, visibleBounds, 64);
            }
            using (Font font = new Font(FontFamily.GenericMonospace, Math.Min(10, 20 / zoom))) {
                Size fontSize = TextRenderer.MeasureText("0", font);
                for (int tile = (int)Math.Floor(visibleBounds.Top / 64); tile < visibleBounds.Bottom / 64; tile++) {
                    g.DrawString((tile / 0x10).ToString("X"), font, Brushes.White, -fontSize.Width, tile * 64);
                    g.DrawString((tile % 0x10).ToString("X"), font, Brushes.White, -fontSize.Width, tile * 64 + fontSize.Height);
                }
            }
        }
    }
}
