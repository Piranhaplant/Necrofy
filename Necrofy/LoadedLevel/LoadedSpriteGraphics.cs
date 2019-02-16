using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class LoadedSpriteGraphics
    {
        public readonly PaletteAsset paletteAsset;
        public readonly GraphicsAsset graphicsAsset;
        public readonly SpritesAsset spritesAsset;
        public readonly EditorAsset<SpriteDisplay[]> spriteDisplayAsset;

        public readonly Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>> sprites;

        public LoadedSpriteGraphics(Project project, string spritePaletteName) {
            paletteAsset = PaletteAsset.FromProject(project, spritePaletteName);
            graphicsAsset = GraphicsAsset.FromProject(project, GraphicsAsset.SpritesName);
            spritesAsset = SpritesAsset.FromProject(project);
            spriteDisplayAsset = EditorAsset<SpriteDisplay[]>.FromProject(project, "SpriteDisplay");

            Color[] colors = SNESGraphics.SNESToRGB(paletteAsset.data, transparent: true);

            sprites = new Dictionary<SpriteDisplay.Key.Type, Dictionary<int, LoadedSprite>>();
            foreach (SpriteDisplay.Key.Type keyType in Enum.GetValues(typeof(SpriteDisplay.Key.Type))) {
                sprites[keyType] = new Dictionary<int, LoadedSprite>();
            }
            foreach (SpriteDisplay spriteDisplay in spriteDisplayAsset.data) {
                LoadedSprite s = new LoadedSprite(spritesAsset.sprites[spriteDisplay.spriteIndex], graphicsAsset.data, colors, spriteDisplay.overridePalette);
                sprites[spriteDisplay.key.type][spriteDisplay.key.value] = s;
            }
        }

        public void Render(SpriteDisplay.Key.Type type, int value, Graphics g, int x, int y) {
            if (sprites[type].TryGetValue(value, out LoadedSprite s)) {
                s.Render(g, x, y);
            }
            // TODO: Render unknown sprites
        }

        public class LoadedSprite
        {
            private readonly Bitmap image;
            private readonly int anchorX;
            private readonly int anchorY;

            public LoadedSprite(Sprite sprite, byte[] graphics, Color[] colors, int? overridePalette) {
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
                    int palette = overridePalette ?? t.palette;
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x00, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), graphics, t.tileNum * 0x80 + 0x20, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x40, colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), graphics, t.tileNum * 0x80 + 0x60, colors, palette * 0x10, t.xFlip, t.yFlip);
                }
            }
            
            public void Render(Graphics g, int x, int y) {
                g.DrawImage(image, x - anchorX, y - anchorY);
            }
        }
    }
}
