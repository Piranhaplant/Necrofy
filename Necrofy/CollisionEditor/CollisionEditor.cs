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
    partial class CollisionEditor : EditorWindow
    {
        private static readonly Brush tileOverlayBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private static readonly Font tileTypeFont = new Font(FontFamily.GenericMonospace, 10);
        private static readonly StringFormat tileTypeStringFormat = new StringFormat() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        private LoadedCollision loadedCollision;
        private ushort[] collisions;
        private LoadedTileset loadedTileset;
        private Bitmap[] tiles;
        private CollisionPreset presets;

        private TilesetObjectBrowserContents objectBrowserContents;
        private UndoManager<CollisionEditor> undoManager;

        public event EventHandler TilesetChanged;

        private CheckBox[] checkboxes;
        private int SelectedTilesetTile => objectBrowserContents?.SelectedIndex ?? -1;

        int uiUpdate = 0;
        ushort prevCollision;

        public CollisionEditor(LoadedCollision loadedCollision) {
            InitializeComponent();
            Disposed += CollisionEditor_Disposed;
            Title = loadedCollision.collisionName;

            checkboxes = new CheckBox[] { checkBoxA, checkBoxB, checkBoxC, checkBoxD, checkBoxE, checkBoxF, checkBoxG, checkBoxH, checkBoxI, checkBoxJ, checkBoxK, checkBoxL, checkBoxM, checkBoxN, checkBoxO, checkBoxP };

            this.loadedCollision = loadedCollision;
            collisions = (ushort[])loadedCollision.tiles.Clone();
        }

        private void CollisionEditor_Disposed(object sender, EventArgs e) {
            DisposeTiles();
        }

        private void DisposeTiles() {
            loadedTileset?.Dispose();
            if (tiles != null) {
                foreach (Bitmap tile in tiles) {
                    tile.Dispose();
                }
            }
        }

        protected override UndoManager Setup() {
            presets = EditorAsset<CollisionPreset>.FromProject(project, "CollisionPresets").data;
            foreach (CollisionPreset.Preset preset in presets.collisions) {
                presetList.Items.Add(preset);
            }

            paletteSelector.LoadProject(project, AssetCategory.Palette, loadedCollision.collisionName);
            graphicsSelector.LoadProject(project, AssetCategory.Graphics, loadedCollision.collisionName);
            tilemapSelector.LoadProject(project, AssetCategory.Tilemap, loadedCollision.collisionName);
            tilePicker.SelectedTile = 0;
            UpdateSelectedCollision();

            undoManager = new UndoManager<CollisionEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        protected override void DoSave(Project project) {
            Array.Copy(collisions, loadedCollision.tiles, collisions.Length);
            loadedCollision.Save(project);
        }

        public void SetCollision(int tileNum, ushort collision) {
            uiUpdate++;
            tilePicker.SelectedTile = tileNum;
            collisions[tileNum] = collision;
            for (int i = 0; i < checkboxes.Length; i++) {
                checkboxes[i].Checked = (collision & (1 << i)) > 0;
            }
            uiUpdate--;
            tilesetPreviewCollision.Invalidate();
        }

        private void assetSelector_SelectedItemChanged(object sender, EventArgs e) {
            if (paletteSelector.SelectedItem != null && graphicsSelector.SelectedItem != null && tilemapSelector.SelectedItem != null) {
                DisposeTiles();

                if (loadedTileset != null) {
                    loadedTileset.TilesChanged -= LoadedTileset_TilesChanged;
                }
                loadedTileset = new LoadedTileset(project, paletteSelector.SelectedItem, graphicsSelector.SelectedItem, tilemapSelector.SelectedItem, loadedCollision.collisionName, 511, 0, null);
                loadedTileset.TilesChanged += LoadedTileset_TilesChanged;
                LoadTiles();
            }
        }

        private void LoadTiles() {
            tiles = SNESGraphics.RenderAllTiles(loadedTileset.graphics);
            tilePicker.SetTiles(tiles, loadedTileset.palette.colors, zoom: 4);

            TilesetChanged?.Invoke(this, EventArgs.Empty);

            if (objectBrowserContents == null) {
                objectBrowserContents = new TilesetObjectBrowserContents(() => loadedTileset, handler => TilesetChanged += handler);
                BrowserContents = objectBrowserContents;
                objectBrowserContents.SelectedIndexChanged += ObjectBrowserContents_SelectedIndexChanged;
            }

            tilesetPreview.Invalidate();
            tilesetPreviewCollision.Invalidate();
        }

        private void LoadedTileset_TilesChanged(object sender, EventArgs e) {
            LoadTiles();
        }

        private void ObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            tilesetPreview.Invalidate();
            tilesetPreviewCollision.Invalidate();
        }

        private void tilePicker_SelectedTileChanged(object sender, EventArgs e) {
            tilesetPreview.Refresh();
            tilesetPreviewCollision.Refresh();

            if (tilePicker.SelectedTile >= 0) {
                undoManager?.ForceNoMerge();
                uiUpdate++;
                ushort tileCollision = collisions[tilePicker.SelectedTile];
                for (int i = 0; i < 16; i++) {
                    checkboxes[i].Checked = (tileCollision & (1 << i)) > 0;
                }
                uiUpdate--;
            }
        }

        private void tilesetPreview_Paint(object sender, PaintEventArgs e) {
            PaintPreview(e.Graphics, false);
        }

        private void tilesetPreviewCollision_Paint(object sender, PaintEventArgs e) {
            PaintPreview(e.Graphics, true);
        }

        private void PaintPreview(Graphics g, bool collisionMode) {
            int selectedTilesetTile = SelectedTilesetTile;
            if (selectedTilesetTile >= 0) {
                float tileSize = tilesetPreview.Width / 8f;

                if (!collisionMode) {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.DrawImage(loadedTileset.tiles[selectedTilesetTile], 0, 0, tilesetPreview.Width, tilesetPreview.Height);
                    MapEditor.DrawGrid(g, Pens.White, tilesetPreview.ClientRectangle, tileSize);
                }

                int selectedTile = tilePicker.SelectedTile;
                for (int y = 0; y < 8; y++) {
                    for (int x = 0; x < 8; x++) {
                        int tileNum = loadedTileset.tilemap.tiles[selectedTilesetTile][x, y].tileNum;
                        RectangleF r = new RectangleF(x * tileSize, y * tileSize, tileSize, tileSize);
                        if (collisionMode) {
                            ushort collision = collisions[tileNum];
                            GetCollisionTypeColors(collision, out Color lightColor, out Color darkColor);
                            using (Brush b = new SolidBrush(lightColor)) {
                                g.FillRectangle(b, r);
                            }
                            using (Pen p = new Pen(darkColor)) {
                                g.DrawRectangle(p, r.X, r.Y, r.Width, r.Height);
                            }
                            g.DrawString(collision.ToString("X4"), tileTypeFont, Brushes.White, r, tileTypeStringFormat);
                        }
                        if (tileNum == selectedTile) {
                            g.FillRectangle(tileOverlayBrush, r);
                        }
                    }
                }
            }
        }

        private CollisionColoring coloring = new CollisionColoring(
            new CollisionColoring(0x0001, 0x0000, // Not solid
                new CollisionColoring(0x0400, 0x0400).Weight(4), // Damage player
                new CollisionColoring(0x4000, 0x4000), // Weeds
                new CollisionColoring(0x0008, 0x0008, // Conveyor belt/trampoline middle
                    new CollisionColoring(0x0200, 0x0200), // Left conveyor
                    new CollisionColoring(0x0400, 0x0400), // Down conveyor
                    new CollisionColoring(0x0100, 0x0100), // Up conveyor
                    new CollisionColoring(0x0020, 0x0020), // Right conveyor
                    new CollisionColoring() // Trampoline middle
                ),
                new CollisionColoring(0x8000, 0x8000), // Boss spider webs
                new CollisionColoring( // No player collision
                    new CollisionColoring(0x1080)
                ).Weight(6)
            ),
            new CollisionColoring( // Solid
                new CollisionColoring(0x0100, 0x0100), // Water
                new CollisionColoring(0x0800, 0x0800), // Pop-up wall
                new CollisionColoring(0x0020, 0x0020), // Drops items
                new CollisionColoring(0x0010, 0x0010), // Key door
                new CollisionColoring(0x8000, 0x8000), // Skull key door
                new CollisionColoring(0x0200, 0x0200), // Trampoline edge
                new CollisionColoring( // Basic solid tiles
                    new CollisionColoring(0x2000, 0x2000), // Allow ants to climb over
                    new CollisionColoring(0x0042, 0x0042), // Destructible
                    new CollisionColoring(0x0004, 0x0004).Weight(6), // Solid to weapons
                    new CollisionColoring().Weight(15)
                ).Weight(5)
            ).Weight(2)
        );

        private void GetCollisionTypeColors(ushort collision, out Color lightColor, out Color darkColor) {
            coloring.GetColors(collision, out double main, out double sub);
            
            int h = (int)(main * Win32.HSLMax);
            int l = Win32.HSLMax / 2 + (int)((sub - 0.2) * Win32.HSLMax / 4);
            int s = Win32.HSLMax / 2 + (int)((1 - sub) * Win32.HSLMax / 4);
            
            darkColor = Win32.HLSToRGB(h, l - 20, s);
            lightColor = Win32.HLSToRGB(h, l, s);
        }

        private void tilesetPreview_MouseDown(object sender, MouseEventArgs e) {
            tilesetPreview_MouseMove(sender, e);
        }

        private void tilesetPreview_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                int selectedTilesetTile = SelectedTilesetTile;
                float tileSize = tilesetPreview.Width / 8f;
                int x = (int)(e.X / tileSize);
                int y = (int)(e.Y / tileSize);
                if (selectedTilesetTile >= 0 && x >= 0 && x < 8 && y >= 0 && y < 8) {
                    LoadedTilemap.Tile tile = loadedTileset.tilemap.tiles[selectedTilesetTile][x, y];
                    tilePicker.SelectedTile = tile.tileNum;
                    tilePicker.Palette = tile.palette;
                }
            }
        }

        private void tilesetPreviewMode_SelectedIndexChanged(object sender, EventArgs e) {
            tilesetPreview.Invalidate();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e) {
            ushort newCollision = 0;
            for (int i = 0; i < checkboxes.Length; i++) {
                if (checkboxes[i].Checked) {
                    newCollision |= (ushort)(1 << i);
                }
            }

            if (uiUpdate == 0) {
                undoManager.Do(new ChangeCollisionAction(tilePicker.SelectedTile, prevCollision, newCollision));
            }

            prevCollision = newCollision;

            UpdateSelectedCollision();
        }

        private void presetList_SelectedIndexChanged(object sender, EventArgs e) {
            if (uiUpdate == 0) {
                undoManager.Do(new ChangeCollisionAction(tilePicker.SelectedTile, prevCollision, ((CollisionPreset.Preset)presetList.SelectedItem).collision));
            }
        }

        private void UpdateSelectedCollision() {
            checkBoxE.ForeColor = GetColor(0x0001);
            checkBoxF.ForeColor = GetColor(0x0001);
            checkBoxF.ForeColor2 = GetColor(0x0008);
            checkBoxG.ForeColor = GetColor(0x0002);
            checkBoxI.ForeColor = GetColor(0x0001);
            checkBoxI.ForeColor2 = GetColor(0x0008);
            checkBoxJ.ForeColor = GetColor(0x0001);
            checkBoxJ.ForeColor2 = GetColor(0x0008);
            checkBoxK.ForeColor = GetColor(0xFB7F, 0);
            checkBoxK.ForeColor2 = GetColor(0x0008);
            checkBoxL.ForeColor = GetColor(0x0001);
            checkBoxO.ForeColor = GetColor(0xBF7F, 0);
            checkBoxP.ForeColor = GetColor(0x7F7F, 0);
            checkBoxP.ForeColor2 = GetColor(0x0009);

            uiUpdate++;
            presetList.SelectedItem = presets.collisions.FirstOrDefault(p => p.collision == prevCollision);
            uiUpdate--;
        }

        private Color GetColor(ushort bitmask) {
            return GetColor(bitmask, bitmask);
        }

        private Color GetColor(ushort bitmask, ushort value) {
            return ((prevCollision & bitmask) == value) ? SystemColors.ControlText : SystemColors.ControlDark;
        }

        private void CollisionEditor_Layout(object sender, LayoutEventArgs e) {
            // Fixes auto-scaling making these not square
            tilesetPreview.Height = tilesetPreview.Width;
            tilesetPreviewCollision.Height = tilesetPreviewCollision.Width;
        }
    }
}
