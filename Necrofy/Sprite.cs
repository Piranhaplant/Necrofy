using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Necrofy
{
    class Sprite
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? pointer = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name = null;
        public List<Tile> tiles;

        public class Tile
        {
            public int graphicsIndex;
            public ushort tileNum;
            public short xOffset;
            public short yOffset;
            public bool xFlip;
            public bool yFlip;
            public int palette;

            public Tile() { }

            public Tile(short xOffset, short yOffset, ushort properties, ushort tileNum) {
                this.xOffset = xOffset;
                this.yOffset = yOffset;
                this.tileNum = tileNum;
                palette = (properties >> 9) & 7;
                xFlip = (properties & 0x4000) > 0;
                yFlip = (properties & 0x8000) > 0;
            }

            public ushort GetProperties() {
                int properties = 0;
                properties |= palette << 9;
                properties |= (xFlip ? 1 : 0) << 14;
                properties |= (yFlip ? 1 : 0) << 15;
                return (ushort)properties;
            }
        }

        public Rectangle GetBounds() {
            if (tiles.Count == 0) {
                return new Rectangle(0, 0, 16, 16);
            }
            int minX = tiles.Min(t => t.xOffset);
            int maxX = tiles.Max(t => t.xOffset + 16);
            int minY = tiles.Min(t => t.yOffset + 1);
            int maxY = tiles.Max(t => t.yOffset + 1 + 16);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public Bitmap Render(List<Graphics> graphics, Color[] colors, int? overridePalette, out int anchorX, out int anchorY) {
            Rectangle bounds = GetBounds();

            Bitmap image;
            if (bounds.Width > 0) {
                image = new Bitmap(bounds.Width, bounds.Height);
            } else {
                image = new Bitmap(1, 1);
            }

            anchorX = -bounds.X;
            anchorY = -bounds.Y;

            List<int> totalGraphicsLengths = graphics.Select(l => l.loadedGraphics.Sum(g => g.linearGraphics.Length) / 4).ToList();

            foreach (Tile t in tiles) {
                if (t.graphicsIndex < graphics.Count && t.tileNum < totalGraphicsLengths[t.graphicsIndex]) {
                    int palette = overridePalette ?? t.palette;

                    GetGraphicsAndTileNum(t.tileNum, graphics[t.graphicsIndex], out int curTile, out int graphicsNum);
                    LoadedGraphics.LinearGraphics curGraphics = graphics[t.graphicsIndex].loadedGraphics[graphicsNum].linearGraphics;

                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), curGraphics[curTile * 4 + 0], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 8 : 0), curGraphics[curTile * 4 + 1], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 8 : 0), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), curGraphics[curTile * 4 + 2], colors, palette * 0x10, t.xFlip, t.yFlip);
                    SNESGraphics.DrawTile(image, anchorX + t.xOffset + (t.xFlip ? 0 : 8), anchorY + t.yOffset + 1 + (t.yFlip ? 0 : 8), curGraphics[curTile * 4 + 3], colors, palette * 0x10, t.xFlip, t.yFlip);
                }
            }

            return image;
        }

        private static void GetGraphicsAndTileNum(int fullTileNum, Graphics graphics, out int tileNum, out int graphicsNum) {
            graphicsNum = 0;
            tileNum = fullTileNum;
            foreach (LoadedGraphics g in graphics.loadedGraphics) {
                if (tileNum < g.linearGraphics.Length / 4) {
                    break;
                }
                tileNum -= g.linearGraphics.Length / 4;
                graphicsNum++;
            }
        }

        public static void AddFromROM(List<Sprite> sprites, NStream romStream, int address, ROMInfo romInfo) {
            romStream.Seek(address);
            List<Tile> tiles = new List<Tile>();
            while (true) {
                int pointer = (int)romStream.Position;
                if (ReadTiles(tiles, romStream, romInfo, false)) {
                    return;
                }
                if (tiles.Count > 0) {
                    sprites.Add(new Sprite {
                        pointer = pointer,
                        tiles = tiles
                    });
                    tiles = new List<Tile>();
                }
            }
        }

        private static bool ReadTiles(List<Tile> tiles, NStream romStream, ROMInfo romInfo, bool trackFreespace) {
            int tileCount = romStream.ReadByte();
            if (tileCount == 0xff || tileCount < 0) {
                return true;
            }
            if (tileCount == 0) {
                return false;
            }
            bool hasExtraTiles = (tileCount & 0x80) > 0;
            tileCount &= 0x7f;

            if (trackFreespace) {
                romInfo.Freespace.AddSize((int)romStream.Position - 1, tileCount * 8 + (hasExtraTiles ? 8 : 0) + 1);
            }

            for (int i = 0; i < tileCount; i++) {
                short xOffset = (short)romStream.ReadInt16();
                short yOffset = (short)romStream.ReadInt16();
                ushort properties = romStream.ReadInt16();
                ushort tileNum = romStream.ReadInt16();
                if (tileNum >= 0x1000) {
                    return true; // Probably hit invalid data
                }
                tiles.Insert(0, new Tile(xOffset, yOffset, properties, tileNum)); // Insert at front so that the top most tile will be at the end of the array
            }

            if (hasExtraTiles) {
                romStream.GoToPointerPush();
                ReadTiles(tiles, romStream, romInfo, true);
                romStream.PopPosition();
                romStream.Seek(4, SeekOrigin.Current);
            }
            return false;
        }

        public static void WriteToROM(SpriteFile sprites, Stream romStream, ROMInfo romInfo, string folder) {
            List<PointerAndSize> graphics = sprites.graphicsAssets.Select(asset => romInfo.GetAssetPointerAndSize(AssetCategory.Graphics, folder + Asset.FolderSeparator + asset)).ToList();

            int customSpritePointer = romInfo.Freespace.Claim(sprites.sprites.Where(s => s.pointer == null).Sum(s => BytesForTiles(s.tiles.Count)));

            foreach (Sprite s in sprites.sprites) {
                int origTileCount;
                if (s.pointer == null) {
                    romStream.Seek(customSpritePointer);
                    customSpritePointer += BytesForTiles(s.tiles.Count);
                    origTileCount = s.tiles.Count;
                } else {
                    romStream.Seek((int)s.pointer);

                    origTileCount = romStream.ReadByte();
                    if ((origTileCount & 0x80) > 0) {
                        origTileCount = (origTileCount & 0x7f) + 1;
                    }

                    romStream.Seek(-1, SeekOrigin.Current);
                }

                if (s.name != null) {
                    romInfo.AddFolderDefine(folder, "Sprite_" + s.name, ROMPointers.PointerToHexString((int)romStream.Position));
                }

                if (s.tiles.Count > origTileCount) {
                    romStream.WriteByte((byte)((origTileCount - 1) | 0x80));
                } else {
                    romStream.WriteByte((byte)s.tiles.Count);
                }

                for (int tileIndex = 0; tileIndex < s.tiles.Count; tileIndex++) {
                    Tile t = s.tiles[s.tiles.Count - 1 - tileIndex];

                    if (tileIndex == origTileCount - 1 && s.tiles.Count > origTileCount) {
                        int extraTileCount = s.tiles.Count - origTileCount + 1;
                        int extraSpacePointer = romInfo.Freespace.Claim(BytesForTiles(extraTileCount));
                        romStream.WritePointer(extraSpacePointer);
                        romStream.Write(new byte[4]);
                        romStream.Seek(extraSpacePointer);
                        romStream.WriteByte((byte)extraTileCount);
                    }

                    romStream.WriteInt16((ushort)t.xOffset);
                    romStream.WriteInt16((ushort)t.yOffset);
                    romStream.WriteInt16(t.GetProperties());
                    romStream.WriteInt16(GetActualTileNum(t.graphicsIndex, t.tileNum, graphics, romInfo.ExtraSpriteGraphicsBasePointer, romInfo.ExtraSpriteGraphicsStartIndex));
                }
                // If there's extra space, fill it with 0s
                for (int tileIndex = s.tiles.Count; tileIndex < origTileCount; tileIndex++) {
                    romStream.Write(new byte[8]);
                }
            }
        }

        private static int BytesForTiles(int tileCount) {
            return tileCount * 8 + 1;
        }

        private static ushort GetActualTileNum(int graphicsIndex, ushort tileNum, List<PointerAndSize> graphics, int extraGraphicsPointer, int extraGraphicsStartNum) {
            if (graphicsIndex == 0) {
                return tileNum;
            }
            return (ushort)((graphics[graphicsIndex - 1].Pointer - extraGraphicsPointer) / 0x80 + tileNum + extraGraphicsStartNum);
        }

        public class Graphics
        {
            public string name;
            public List<LoadedGraphics> loadedGraphics = new List<LoadedGraphics>();
            public List<Bitmap> images = new List<Bitmap>();

            public Graphics(Project project, params string[] assets) {
                name = new Asset.ParsedName(assets[0]).FinalName;
                foreach (string asset in assets) {
                    try {
                        loadedGraphics.Add(new LoadedGraphics(project, asset, GetGraphicsAssetType(asset)));
                    } catch (AssetNotFoundException e) {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }

            private static GraphicsAsset.Type GetGraphicsAssetType(string name) {
                return name == GraphicsAsset.SpriteGraphics ? GraphicsAsset.Type.Normal : GraphicsAsset.Type.Sprite;
            }

            public void LoadTiles() {
                Dispose();

                foreach (LoadedGraphics g in loadedGraphics) {
                    for (int i = 0; i < g.linearGraphics.Length / 4; i++) {
                        Bitmap b = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);
                        BitmapData data = b.LockBits(new Rectangle(Point.Empty, b.Size), ImageLockMode.WriteOnly, b.PixelFormat);

                        SNESGraphics.DrawTile(data, 0, 0, new LoadedTilemap.Tile(i * 4 + 0, 0, false, false, false), g.linearGraphics);
                        SNESGraphics.DrawTile(data, 8, 0, new LoadedTilemap.Tile(i * 4 + 1, 0, false, false, false), g.linearGraphics);
                        SNESGraphics.DrawTile(data, 0, 8, new LoadedTilemap.Tile(i * 4 + 2, 0, false, false, false), g.linearGraphics);
                        SNESGraphics.DrawTile(data, 8, 8, new LoadedTilemap.Tile(i * 4 + 3, 0, false, false, false), g.linearGraphics);

                        b.UnlockBits(data);
                        images.Add(b);
                    }
                }
            }

            public void Dispose() {
                foreach (Bitmap image in images) {
                    image.Dispose();
                }
                images.Clear();
            }
        }
    }
}
