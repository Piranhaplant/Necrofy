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
        public readonly EditorAsset spriteGraphicsAsset;

        public readonly Dictionary<int, Sprite> sprites;

        public LoadedSpriteGraphics(Project project, string spritePaletteName) {
            paletteAsset = PaletteAsset.FromProject(project, spritePaletteName);
            graphicsAsset = GraphicsAsset.FromProject(project, GraphicsAsset.Sprites);
            spriteGraphicsAsset = EditorAsset.FromProject(project, EditorAsset.SpriteGraphics);

            Color[] colors = SNESGraphics.SNESToRGB(paletteAsset.data, true);
            SpriteGraphics.Sprite[] sprites = JsonConvert.DeserializeObject<SpriteGraphics.Sprite[]>(spriteGraphicsAsset.text);

            this.sprites = new Dictionary<int, Sprite>();
            foreach (SpriteGraphics.Sprite sprite in sprites) {
                Sprite s = new Sprite(sprite, graphicsAsset.data, colors);
                this.sprites.Add(s.key, s);
            }
        }

        public class Sprite
        {
            private readonly Bitmap image;
            private readonly int anchorX;
            private readonly int anchorY;
            public readonly int key;

            public Sprite(SpriteGraphics.Sprite sprite, byte[] graphics, Color[] colors) {
                anchorX = sprite.anchorX;
                anchorY = sprite.anchorY;
                key = sprite.key;

                image = new Bitmap(sprite.displayWidth * 8, sprite.displayHeight * 8);
                DrawTiles(sprite, graphics, colors, false);
                DrawTiles(sprite, graphics, colors, true);
            }

            private void DrawTiles(SpriteGraphics.Sprite sprite, byte[] graphics, Color[] colors, bool priority) {
                for (int y = 0; y < sprite.height; y++) {
                    for (int x = 0; x < sprite.width; x++) {
                        SpriteGraphics.Tile tile = sprite.tiles[x, y];
                        if (tile.priority == priority && tile.index > 0) {
                            SNESGraphics.DrawTile(image, x * 8 + tile.xShift, y * 8 + tile.yShift, graphics, tile.index, colors, tile.palette * 0x10, tile.xFlip, tile.yFlip);
                        }
                    }
                }
            }

            public void Render(Graphics g, int x, int y) {
                g.DrawImage(image, x - anchorX, y - anchorY);
            }
        }
    }
}
