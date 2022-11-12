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
        private readonly Project project;

        private LoadedPalette loadedPalette;
        private List<LoadedGraphics> loadedGraphics = new List<LoadedGraphics>();
        private SpritesAsset spritesAsset;
        private EditorAsset<SpriteDisplayList> spriteDisplayAsset;

        private static readonly Font unknownSpriteFont = new Font(FontFamily.GenericMonospace, 8);
        private static readonly StringFormat unknownSpriteStringFormat = new StringFormat() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>> sprites;
        public Dictionary<SpriteDisplay.Category, List<LoadedSprite>> spritesByCategory;

        public event EventHandler Updated;

        public LoadedLevelSprites(Project project, string spritePaletteName) {
            this.project = project;

            loadedPalette = new LoadedPalette(project, spritePaletteName, transparent: true);
            spritesAsset = SpritesAsset.FromProject(project, Asset.SpritesFolder + Asset.FolderSeparator + SpritesAsset.DefaultFileName);
            spriteDisplayAsset = EditorAsset<SpriteDisplayList>.FromProject(project, "SpriteDisplay");

            loadedPalette.Updated += Asset_Updated;
            spritesAsset.Updated += Asset_Updated;

            Load();
        }

        private void Load() {
            foreach (LoadedGraphics g in loadedGraphics) {
                g.Updated -= Asset_Updated;
            }
            loadedGraphics.Clear();

            foreach (string assetName in spritesAsset.sprites.graphicsAssets) {
                LoadGraphics(assetName);
            }
            if (loadedGraphics.Count == 0) {
                LoadGraphics(GraphicsAsset.SpriteGraphics);
            }

            sprites = new Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>>();
            foreach (SpriteDisplay.Key.Type keyType in Enum.GetValues(typeof(SpriteDisplay.Key.Type))) {
                sprites[keyType] = new Dictionary<int, LoadedSprite>();
            }
            spritesByCategory = new Dictionary<SpriteDisplay.Category, List<LoadedSprite>>();
            foreach (SpriteDisplay.Category category in Enum.GetValues(typeof(SpriteDisplay.Category))) {
                spritesByCategory[category] = new List<LoadedSprite>();
            }

            Dictionary<int, Sprite> spritePointers = new Dictionary<int, Sprite>();
            foreach (Sprite s in spritesAsset.sprites.sprites) {
                if (s.pointer != null) {
                    spritePointers[(int)s.pointer] = s;
                }
            }

            foreach (ImageSpriteDisplay spriteDisplay in spriteDisplayAsset.data.imageSprites) {
                if (spritePointers.ContainsKey(spriteDisplay.spritePointer)) {
                    LoadedSprite s = new ImageLoadedSprite(spriteDisplay, spritePointers[spriteDisplay.spritePointer], loadedGraphics, loadedPalette.colors);
                    AddLoadedSprite(spriteDisplay, s);
                }
            }
            foreach (TextSpriteDisplay spriteDisplay in spriteDisplayAsset.data.textSprites) {
                LoadedSprite s = new TextLoadedSprite(spriteDisplay);
                AddLoadedSprite(spriteDisplay, s);
            }
        }

        private void LoadGraphics(string graphicsName) {
            LoadedGraphics g = new LoadedGraphics(project, graphicsName, LoadedSprites.GetGraphicsAssetType(graphicsName));
            loadedGraphics.Add(g);
            g.Updated += Asset_Updated;
        }

        public void Dispose() {
            foreach (LoadedSprite s in sprites.Values.SelectMany(d => d.Values)) {
                s.Dispose();
            }
        }

        private void Asset_Updated(object sender, EventArgs e) {
            Dispose();
            Load();
            Updated?.Invoke(sender, e);
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

            public ImageLoadedSprite(ImageSpriteDisplay spriteDisplay, Sprite sprite, List<LoadedGraphics> loadedGraphics, Color[] colors) : base(spriteDisplay) {
                image = sprite.Render(loadedGraphics, colors, spriteDisplay.overridePalette, out anchorX, out anchorY);
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
