using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Necrofy
{
    class LoadedLevelSprites : IDisposable
    {
        private static readonly Font unknownSpriteFont = new Font(FontFamily.GenericMonospace, 8);
        private static readonly StringFormat unknownSpriteStringFormat = new StringFormat() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public readonly Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>> sprites;
        public readonly Dictionary<SpriteDisplay.Category, List<LoadedSprite>> spritesByCategory;

        public LoadedLevelSprites(Project project, string spritePaletteName) {
            LoadedPalette loadedPalette = new LoadedPalette(project, spritePaletteName, transparent: true);
            LoadedGraphics loadedGraphics = new LoadedGraphics(project, Asset.SpritesFolder + Asset.FolderSeparator + GraphicsAsset.DefaultName);
            SpritesAsset spritesAsset = SpritesAsset.FromProject(project, Asset.SpritesFolder);
            EditorAsset<SpriteDisplayList> spriteDisplayAsset = EditorAsset<SpriteDisplayList>.FromProject(project, "SpriteDisplay");

            sprites = new Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>>();
            foreach (SpriteDisplay.Key.Type keyType in Enum.GetValues(typeof(SpriteDisplay.Key.Type))) {
                sprites[keyType] = new Dictionary<int, LoadedSprite>();
            }
            spritesByCategory = new Dictionary<SpriteDisplay.Category, List<LoadedSprite>>();
            foreach (SpriteDisplay.Category category in Enum.GetValues(typeof(SpriteDisplay.Category))) {
                spritesByCategory[category] = new List<LoadedSprite>();
            }

            foreach (ImageSpriteDisplay spriteDisplay in spriteDisplayAsset.data.imageSprites) {
                LoadedSprite s = new ImageLoadedSprite(spriteDisplay, spritesAsset.sprites[spriteDisplay.spriteIndex], loadedGraphics.linearGraphics, loadedPalette.colors);
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
            } else {
                Rectangle r;
                int length;
                if (type == SpriteDisplay.Key.Type.Item) {
                    r = new Rectangle(x - 8, y - 8, 17, 17);
                    length = 2;
                } else if (type == SpriteDisplay.Key.Type.CreditHead) {
                    r = new Rectangle(x - 10, y - 28, 20, 28);
                    length = 4;
                } else {
                    r = new Rectangle(x - 10, y - 40, 20, 40);
                    length = 6;
                }
                g.FillRectangle(Brushes.Black, r);
                g.DrawRectangle(Pens.White, r);
                g.DrawString(value.ToString("X" + length.ToString()), unknownSpriteFont, Brushes.White, r, unknownSpriteStringFormat);
            }
        }

        public Rectangle GetRectangle(SpriteDisplay.Key.Type type, int value, int x, int y) {
            if (sprites[type].TryGetValue(value, out LoadedSprite s)) {
                return s.GetRectangle(x, y);
            } else {
                if (type == SpriteDisplay.Key.Type.Item) {
                    return new Rectangle(x - 8, y - 8, 17, 17);
                } else if (type == SpriteDisplay.Key.Type.CreditHead) {
                    return new Rectangle(x - 10, y - 28, 20, 28);
                } else {
                    return new Rectangle(x - 10, y - 40, 20, 40);
                }
            }
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

            public ImageLoadedSprite(ImageSpriteDisplay spriteDisplay, Sprite sprite, LoadedGraphics.LinearGraphics graphics, Color[] colors) : base(spriteDisplay) {
                image = sprite.Render(graphics, colors, spriteDisplay.overridePalette, out anchorX, out anchorY);
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
