using Necrofy.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class TileAnimationEditor : UserControl, ObjectSelector<WrappedAnimationTile>.IHost
    {
        private static readonly Brush tileOverlayBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private static readonly Pen borderPen = new Pen(Color.White, 1 / VisualZoom);

        private readonly LevelSettingsDialog levelSettings;
        private readonly TileAnimLevelMonster tileAnimLevelMonster;
        private readonly LoadedTileset tileset;

        private readonly TilesetObjectBrowserContents objectBrowserContents;
        private readonly ScrollWrapper scrollWrapper;
        private readonly ObjectSelector<WrappedAnimationTile> objectSelector;

        private const int TimelinePadding = 10;
        private const int PixelsPerTimelineRow = 20;
        private const float VisualZoom = 4.0f;
        private int horizontalZoom = 4;

        private Bitmap[] tiles = null;
        private int[] preferedPalettes = null;

        public TileAnimationEditor(LevelSettingsDialog levelSettings, TileAnimLevelMonster tileAnimLevelMonster, Project project, string paletteName, string graphicsName, string tilemapName, string collisionName, int visibleTilesEnd, int priorityTileCount) {
            InitializeComponent();

            this.levelSettings = levelSettings;
            this.tileAnimLevelMonster = tileAnimLevelMonster;
            tileset = new LoadedTileset(project, paletteName, graphicsName, tilemapName, collisionName, visibleTilesEnd, priorityTileCount, tileAnimLevelMonster);
            tileset.TilesChanged += Tileset_TilesChanged;
            tileset.Animated += Tileset_Animated;

            objectBrowserContents = new TilesetObjectBrowserContents(() => tileset, handler => {
                tileset.TilesChanged += handler;
                tileset.Animated += handler;
            });
            objectBrowserContents.SelectedIndexChanged += ObjectBrowserContents_SelectedIndexChanged;
            levelSettings.SetObjectBrowserContents(objectBrowserContents);
            objectBrowserContents.SelectedIndex = 0;

            scrollWrapper = new ScrollWrapper(mainCanvas, hScroll, vScroll);
            scrollWrapper.Zoom = VisualZoom;
            scrollWrapper.CenterContents = false;
            CalculateTimelineSize();
            scrollWrapper.Scrolled += ScrollWrapper_Scrolled;

            objectSelector = new ObjectSelector<WrappedAnimationTile>(this);

            LoadGraphics();
            Disposed += TileAnimationEditor_Disposed;
        }

        private void ObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            previewCanvas.Repaint();
        }

        private void tilePicker_SelectedTileChanged(object sender, EventArgs e) {
            previewCanvas.Repaint();
            levelSettings.SetInfo1($"Selected tile: 0x{tilePicker.SelectedTile:X}");
        }

        private void TileAnimationEditor_Disposed(object sender, EventArgs e) {
            SNESGraphics.DisposeAll(tiles);
            tileset.Dispose();
        }

        private void closeButton_Click(object sender, EventArgs e) {
            levelSettings.HideTileAnimationEditor(this);
        }

        private void Tileset_TilesChanged(object sender, EventArgs e) {
            LoadGraphics();
        }

        private void Tileset_Animated(object sender, EventArgs e) {
            previewCanvas.Repaint();
        }

        private void ScrollWrapper_Scrolled(object sender, EventArgs e) {
            mainCanvas.Repaint();
        }

        private void LoadGraphics() {
            SNESGraphics.DisposeAll(tiles);
            tiles = SNESGraphics.RenderAllTiles(tileset.graphics);
            tilePicker.SetTiles(tiles, tileset.palette.colors, zoom: 4);
            preferedPalettes = tileset.GetPreferedPalettes();
            tilePicker.PalettePerTile = preferedPalettes;
        }

        private void previewCanvas_MouseDown(object sender, MouseEventArgs e) {
            previewCanvas_MouseMove(sender, e);
        }

        private void previewCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                int selectedTilesetTile = objectBrowserContents.SelectedTile;
                float tileSize = previewCanvas.Width / 8f;
                int x = (int)(e.X / tileSize);
                int y = (int)(e.Y / tileSize);
                if (selectedTilesetTile >= 0 && x >= 0 && x < 8 && y >= 0 && y < 8) {
                    LoadedTilemap.Tile tile = tileset.tilemap.tiles[selectedTilesetTile][x, y];
                    tilePicker.SelectedTile = tile.tileNum;
                    tilePicker.Palette = tile.palette;
                }
            }
        }

        private void previewCanvas_Paint(object sender, PaintEventArgs e) {
            int selectedTilesetTile = objectBrowserContents.SelectedTile; ;
            if (selectedTilesetTile >= 0) {
                float tileSize = previewCanvas.Width / 8f;

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                e.Graphics.DrawImage(tileset.tiles[selectedTilesetTile], 0, 0, previewCanvas.Width, previewCanvas.Height);
                MapEditor.DrawGrid(e.Graphics, Pens.White, previewCanvas.ClientRectangle, tileSize);

                int selectedTile = tilePicker.SelectedTile;
                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        int tileNum = tileset.tilemap.tiles[selectedTilesetTile][x, y].tileNum;
                        if (tileNum == selectedTile) {
                            RectangleF r = new RectangleF(x * tileSize, y * tileSize, tileSize, tileSize);
                            e.Graphics.FillRectangle(tileOverlayBrush, r);
                        }
                    }
                }
            }
        }

        private void restartButton_Click(object sender, EventArgs e) {
            tileset.tileAnimator.Restart();
        }

        private void playPauseButton_Click(object sender, EventArgs e) {
            if (tileset.tileAnimator.Running) {
                tileset.tileAnimator.Pause();
                playPauseButton.Image = Resources.control;
            } else {
                tileset.tileAnimator.Run();
                playPauseButton.Image = Resources.control_pause;
            }
        }

        private void advanceFrameButton_Click(object sender, EventArgs e) {
            tileset.tileAnimator.Pause();
            tileset.tileAnimator.Advance();
            playPauseButton.Image = Resources.control;
        }

        private void stopButton_Click(object sender, EventArgs e) {
            tileset.tileAnimator.Pause();
            tileset.tileAnimator.Restart();
            playPauseButton.Image = Resources.control;
        }

        private void CalculateTimelineSize() {
            int width = tileAnimLevelMonster.entries.Max(e => e.frames.Sum(f => f.delay)) * horizontalZoom + 8 + TimelinePadding * 2;
            int height = (tileAnimLevelMonster.entries.Count - 1) * PixelsPerTimelineRow + 8 + TimelinePadding * 2;
            scrollWrapper.SetPadding(TimelinePadding, TimelinePadding);
            scrollWrapper.SetClientSize(width, height);
        }

        private void mainCanvas_Paint(object sender, PaintEventArgs e) {
            scrollWrapper.TransformGraphics(e.Graphics);
            for (int i = 0; i < tileAnimLevelMonster.entries.Count; i++) {
                TileAnimLevelMonster.Entry entry = tileAnimLevelMonster.entries[i];

                int x = 0;
                int y = i * PixelsPerTimelineRow;
                int palette = preferedPalettes[entry.initialTile];
                SNESGraphics.DrawWithPlt(e.Graphics, x, y, tiles[entry.initialTile], tileset.palette.colors, palette * 0x10, 0x10);

                foreach (TileAnimLevelMonster.Entry.Frame frame in entry.frames) {
                    int prevX = x;
                    x += frame.delay * horizontalZoom;

                    e.Graphics.DrawLine(Pens.White, prevX + 8, y + 4, x, y + 4);
                    SNESGraphics.DrawWithPlt(e.Graphics, x, y, tiles[frame.tile], tileset.palette.colors, palette * 0x10, 0x10);
                }
            }

            objectSelector.DrawSelectionRectangle(e.Graphics, VisualZoom);

            foreach (WrappedAnimationTile tile in objectSelector.GetSelectedObjects()) {
                e.Graphics.FillRectangle(tileOverlayBrush, tile.x, tile.y, 8, 8);
                e.Graphics.DrawRectangle(borderPen, tile.x, tile.y, 8, 8);
            }
        }

        public IEnumerable<WrappedAnimationTile> GetObjects() {
            for (int i = 0; i < tileAnimLevelMonster.entries.Count; i++) {
                TileAnimLevelMonster.Entry entry = tileAnimLevelMonster.entries[i];

                int x = 0;
                int y = i * PixelsPerTimelineRow;
                yield return new WrappedAnimationTile(entry, x, y);
                foreach (TileAnimLevelMonster.Entry.Frame frame in entry.frames) {
                    x += frame.delay * horizontalZoom;
                    yield return new WrappedAnimationTile(frame, x, y);
                }
            }
        }

        public void SelectionChanged() {
            mainCanvas.Repaint();
        }

        public void MoveSelectedObjects(int dx, int dy, int snap) {
            
        }

        public void SetSelectedObjectsPosition(int? x, int? y) {
            
        }

        public WrappedAnimationTile CreateObject(int x, int y) {
            return null;
        }

        public IEnumerable<WrappedAnimationTile> CloneSelection() {
            yield break;
        }

        private void mainCanvas_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Point transformed = scrollWrapper.TransformPoint(e.Location);
                objectSelector.MouseDown(transformed.X, transformed.Y);
            }
        }

        private void mainCanvas_MouseMove(object sender, MouseEventArgs e) {
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            objectSelector.MouseMove(transformed.X, transformed.Y);
        }

        private void mainCanvas_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                objectSelector.MouseUp();
            }
        }

        private void mainCanvas_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) {
                e.IsInputKey = true;
            }
        }

        private void mainCanvas_KeyDown(object sender, KeyEventArgs e) {
            objectSelector.KeyDown(e.KeyData);
        }
    }
}
