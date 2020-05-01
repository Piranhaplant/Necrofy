using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Necrofy
{
    class LoadedSpriteGraphics : IDisposable
    {
        public readonly PaletteAsset paletteAsset;
        public readonly GraphicsAsset graphicsAsset;
        public readonly SpritesAsset spritesAsset;
        public readonly EditorAsset<SpriteDisplayList> spriteDisplayAsset;

        public readonly Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>> sprites;
        public readonly Dictionary<SpriteDisplay.Category, List<LoadedSprite>> spritesByCategory;

        public LoadedSpriteGraphics(Project project, string spritePaletteName) {
            paletteAsset = PaletteAsset.FromProject(project, spritePaletteName);
            graphicsAsset = GraphicsAsset.FromProject(project, GraphicsAsset.SpritesName);
            spritesAsset = SpritesAsset.FromProject(project);
            spriteDisplayAsset = EditorAsset<SpriteDisplayList>.FromProject(project, "SpriteDisplay");

            Color[] colors = SNESGraphics.SNESToRGB(paletteAsset.data, transparent: true);

            sprites = new Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>>();
            foreach (SpriteDisplay.Key.Type keyType in Enum.GetValues(typeof(SpriteDisplay.Key.Type))) {
                sprites[keyType] = new Dictionary<int, LoadedSprite>();
            }
            spritesByCategory = new Dictionary<SpriteDisplay.Category, List<LoadedSprite>>();
            foreach (SpriteDisplay.Category category in Enum.GetValues(typeof(SpriteDisplay.Category))) {
                spritesByCategory[category] = new List<LoadedSprite>();
            }

            foreach (ImageSpriteDisplay spriteDisplay in spriteDisplayAsset.data.imageSprites) {
                LoadedSprite s = new ImageLoadedSprite(spriteDisplay, spritesAsset.sprites[spriteDisplay.spriteIndex], graphicsAsset.data, colors);
                AddLoadedSprite(spriteDisplay, s);
            }
            foreach (TextSpriteDisplay spriteDisplay in spriteDisplayAsset.data.textSprites) {
                LoadedSprite s = new TextLoadedSprite(spriteDisplay);
                AddLoadedSprite(spriteDisplay, s);
            }
        }

        public void Dispose() {
            foreach (LoadedSprite s in sprites.Values.SelectMany(d => d.Values)) {
                s.Dispose();
            }
        }

        private void AddLoadedSprite(SpriteDisplay spriteDisplay, LoadedSprite s) {
            sprites[spriteDisplay.key.type][spriteDisplay.key.value] = s;
            spritesByCategory[spriteDisplay.category].Add(s);
        }

        public void Render(SpriteDisplay.Key.Type type, int value, Graphics g, int x, int y) {
            if (sprites[type].TryGetValue(value, out LoadedSprite s)) {
                s.Render(g, x, y);
            }
            // TODO: unknown sprites
        }

        public Rectangle GetRectangle(SpriteDisplay.Key.Type type, int value, int x, int y) {
            if (sprites[type].TryGetValue(value, out LoadedSprite s)) {
                return s.GetRectangle(x, y);
            }
            // TODO: unknown sprites
            return Rectangle.Empty;
        }

        public abstract class LoadedSprite : IDisposable
        {
            public SpriteDisplay.Key Key { get; private set; }
            public string Description { get; private set; }
            public abstract Size Size { get; }
            public abstract Rectangle GetRectangle(int x, int y);
            public abstract void Render(Graphics g, int x, int y);
            public abstract void RenderFromTopCorner(Graphics g, int x, int y);
            public abstract void Dispose();

            public LoadedSprite(SpriteDisplay spriteDisplay) {
                Key = spriteDisplay.key;
                Description = spriteDisplay.description;
            }
        }

        private class ImageLoadedSprite : LoadedSprite
        {
            private readonly Bitmap image;
            private readonly int anchorX;
            private readonly int anchorY;

            public ImageLoadedSprite(ImageSpriteDisplay spriteDisplay, Sprite sprite, byte[] graphics, Color[] colors) : base(spriteDisplay) {
                int minX = 0, maxX = 0, minY = 0, maxY = 0;
                foreach (Sprite.Tile t in sprite.tiles) {
                    minX = Math.Min(minX, t.xOffset);
                    maxX = Math.Max(maxX, t.xOffset + 16);
                    minY = Math.Min(minY, t.yOffset + 1);
                    maxY = Math.Max(maxY, t.yOffset + 1 + 16);
                }

                anchorX = -minX;
                anchorY = -minY;
                if (sprite.tiles.Length > 0) {
                    image = new Bitmap(maxX - minX, maxY - minY);
                } else {
                    image = new Bitmap(1, 1);
                }

                foreach (Sprite.Tile t in sprite.tiles) {
                    int palette = spriteDisplay.overridePalette ?? t.palette;
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x00, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x20, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x40, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x60, colors, palette * 0x10, t.xFlip, t.yFlip);
                }
            }

            public override void Dispose() {
                image.Dispose();
            }

            public override Size Size => image.Size;

            public override Rectangle GetRectangle(int x, int y) {
                return new Rectangle(x - anchorX, y - anchorY, image.Width, image.Height);
            }

            public override void Render(Graphics g, int x, int y) {
                g.DrawImage(image, x - anchorX, y - anchorY);
            }

            public override void RenderFromTopCorner(Graphics g, int x, int y) {
                g.DrawImage(image, x, y);
            }
        }

        private class TextLoadedSprite : LoadedSprite
        {
            private static readonly Font font = SystemFonts.DefaultFont;
            private const int padding = 2;

            private readonly string text;
            private Size textSize;

            public TextLoadedSprite(TextSpriteDisplay spriteDisplay) : base(spriteDisplay) {
                text = spriteDisplay.text;
                Size s = TextRenderer.MeasureText(text, font);
                textSize = new Size(s.Width + padding * 2, s.Height + padding * 2);
            }

            public override void Dispose() {
                // Nothing to dispose
            }

            public override Size Size => textSize;

            public override Rectangle GetRectangle(int x, int y) {
                return new Rectangle(x - textSize.Width / 2, y - textSize.Height, textSize.Width, textSize.Height);
            }

            public override void Render(Graphics g, int x, int y) {
                Rectangle r = GetRectangle(x, y);
                g.FillRectangle(Brushes.Black, r);
                g.DrawRectangle(Pens.White, r);
                g.DrawString(text, font, Brushes.White, r.X + padding, r.Y + padding);
            }

            public override void RenderFromTopCorner(Graphics g, int x, int y) {
                g.DrawString(text, font, Brushes.Black, x + padding, y + padding);
            }
        }
    }
}
