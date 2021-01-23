using Newtonsoft.Json;
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
    partial class SpriteEditor : EditorWindow, ObjectSelector<WrappedSpriteTile>.IHost
    {
        private const int AllowedSize = 512;
        private const int MaxDimension = AllowedSize / 2;

        private static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 255, 255, 255));

        private readonly RadioButton[] paletteButtons;

        private readonly LoadedSprites loadedSprites;
        private readonly SpriteEditorObjectBrowserContents browserContents;
        private readonly ObjectSelector<WrappedSpriteTile> objectSelector;

        public readonly ScrollWrapper scrollWrapper;
        public UndoManager<SpriteEditor> undoManager;

        private Sprite currentSprite = null;
        private bool showAxes;
        private bool showTileBorders;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.SpriteEditor;
        private static readonly ToolStripGrouper.ItemType[] wordSelectedItems = new ToolStripGrouper.ItemType[] {
            ToolStripGrouper.ItemType.FlipHorizontally, ToolStripGrouper.ItemType.FlipVertically,
            ToolStripGrouper.ItemType.CenterHorizontally, ToolStripGrouper.ItemType.CenterVertically,
            ToolStripGrouper.ItemType.MoveUp, ToolStripGrouper.ItemType.MoveDown,
            ToolStripGrouper.ItemType.MoveToFront, ToolStripGrouper.ItemType.MoveToBack
        };

        public SpriteEditor(LoadedSprites loadedSprites) {
            InitializeComponent();
            Disposed += SpriteEditor_Disposed;
            paletteButtons = new RadioButton[] { palette0Button, palette1Button, palette2Button, palette3Button, palette4Button, palette5Button, palette6Button, palette7Button };

            this.loadedSprites = loadedSprites;
            Title = "Sprites";

            objectSelector = new ObjectSelector<WrappedSpriteTile>(this, maxX: MaxDimension, maxY: MaxDimension, minX: -MaxDimension, minY: -MaxDimension);

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
            scrollWrapper.SetPadding(MaxDimension, MaxDimension);
            scrollWrapper.SetClientSize(AllowedSize, AllowedSize);

            browserContents = new SpriteEditorObjectBrowserContents(loadedSprites);
            browserContents.SelectedIndexChanged += BrowserContents_SelectedIndexChanged;
            BrowserContents = browserContents;

            tilePicker.SetTiles(loadedSprites.tileImages, loadedSprites.loadedPalette.colors);
        }

        private void SpriteEditor_Disposed(object sender, EventArgs e) {
            // TODO
        }

        protected override UndoManager Setup() {
            undoManager = new UndoManager<SpriteEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public override void Displayed() {
            base.Displayed();
            UpdateToolbar();
            UpdateViewOptions();
        }

        private void UpdateViewOptions() {
            showAxes = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewAxes).Checked;
            showTileBorders = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTileBorders).Checked;
        }

        private void UpdateToolbar() {
            foreach (ToolStripGrouper.ItemType item in wordSelectedItems) {
                mainWindow.GetToolStripItem(item).Enabled = selectedObjects.Count > 0;
            }
        }

        protected override void DoSave(Project project) {
            loadedSprites.spritesAsset.WriteFile(project);
        }

        public override bool CanCopy => selectedObjects.Count > 0;
        public override bool CanPaste => true;
        public override bool CanDelete => selectedObjects.Count > 0;
        public override bool HasSelection => true;
        public override bool CanZoom => true;

        public override void Copy() {
            Clipboard.SetText(JsonConvert.SerializeObject(selectedObjects.Select(t => t.tile)));
        }

        public override void Paste() {
            try {
                IEnumerable<WrappedSpriteTile> tiles = JsonConvert.DeserializeObject<List<Sprite.Tile>>(Clipboard.GetText()).Select(t => new WrappedSpriteTile(t));
                undoManager.Do(new AddSpriteTileAction(currentSprite, tiles));
                objectSelector.SelectObjects(tiles);
            } catch (Exception) { }
        }

        public override void Delete() {
            List<WrappedSpriteTile> tiles = new List<WrappedSpriteTile>(selectedObjects);
            List<int> zIndexes = objectSelector.SortAndGetZIndexes(tiles);
            undoManager.Do(new DeleteSpriteTileAction(currentSprite, tiles, zIndexes));
        }

        public override void SelectAll() {
            objectSelector.SelectAll();
        }

        public override void SelectNone() {
            objectSelector.SelectNone();
        }

        protected override void ZoomChanged() {
            scrollWrapper.Zoom = Zoom;
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            switch (item) {
                case ToolStripGrouper.ItemType.CenterHorizontally:
                    objectSelector.CenterHorizontally();
                    undoManager.ForceNoMerge();
                    break;
                case ToolStripGrouper.ItemType.CenterVertically:
                    objectSelector.CenterVertically();
                    undoManager.ForceNoMerge();
                    break;
                case ToolStripGrouper.ItemType.MoveUp: {
                    objectSelector.MoveUp(out List<WrappedSpriteTile> tiles, out List<int> oldZIndexes, out List<int> newZIndexes);
                    undoManager.Do(new ChangeSpriteTileZIndexAction(currentSprite, tiles, oldZIndexes, newZIndexes));
                    break;
                }
                case ToolStripGrouper.ItemType.MoveDown: {
                    objectSelector.MoveDown(out List<WrappedSpriteTile> tiles, out List<int> oldZIndexes, out List<int> newZIndexes);
                    undoManager.Do(new ChangeSpriteTileZIndexAction(currentSprite, tiles, oldZIndexes, newZIndexes));
                    break;
                }
                case ToolStripGrouper.ItemType.MoveToFront: {
                    objectSelector.MoveToFront(out List<WrappedSpriteTile> tiles, out List<int> oldZIndexes, out List<int> newZIndexes);
                    undoManager.Do(new ChangeSpriteTileZIndexAction(currentSprite, tiles, oldZIndexes, newZIndexes));
                    break;
                }
                case ToolStripGrouper.ItemType.MoveToBack: {
                    objectSelector.MoveToBack(out List<WrappedSpriteTile> tiles, out List<int> oldZIndexes, out List<int> newZIndexes);
                    undoManager.Do(new ChangeSpriteTileZIndexAction(currentSprite, tiles, oldZIndexes, newZIndexes));
                    break;
                }
                case ToolStripGrouper.ItemType.FlipHorizontally:
                    undoManager.Do(new FlipSpriteTilesHorizontallyAction(currentSprite, selectedObjects));
                    break;
                case ToolStripGrouper.ItemType.FlipVertically:
                    undoManager.Do(new FlipSpriteTilesVerticallyAction(currentSprite, selectedObjects));
                    break;
            }
        }

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            if (item == ToolStripGrouper.ItemType.ViewAxes || item == ToolStripGrouper.ItemType.ViewTileBorders) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        public void SetCurrentSprite(Sprite s) {
            if (currentSprite == s) {
                return;
            }
            currentSprite = s;
            objectSelector.SelectNone();
            browserContents.SelectedIndex = Array.IndexOf(loadedSprites.spritesAsset.sprites, currentSprite);
            Repaint();
        }
        
        private void scrollWrapper_Scrolled(object sender, EventArgs e) {
            Repaint();
        }

        private void BrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            if (browserContents.SelectedIndex >= 0) {
                if (currentSprite == null) {
                    scrollWrapper.ScrollToPoint(MaxDimension, MaxDimension);
                }
                SetCurrentSprite(loadedSprites.spritesAsset.sprites[browserContents.SelectedIndex]);
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (currentSprite == null) {
                return;
            }

            scrollWrapper.TransformGraphics(e.Graphics);
            e.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(-MaxDimension, -MaxDimension, AllowedSize, AllowedSize));
            if (showAxes) {
                // Move by 0.5 to make it so the lines still line up when zoomed in with "Half" PixelOffsetMode
                e.Graphics.DrawLine(Pens.BlueViolet, -MaxDimension, -0.5f, MaxDimension, -0.5f);
                e.Graphics.DrawLine(Pens.BlueViolet, -0.5f, -MaxDimension, -0.5f, MaxDimension);
            }

            foreach (Sprite.Tile tile in currentSprite.tiles) {
                SNESGraphics.DrawWithPlt(e.Graphics, tile.xOffset, tile.yOffset, loadedSprites.tileImages[tile.tileNum], loadedSprites.loadedPalette.colors, tile.palette * 0x10, 0x10, tile.xFlip, tile.yFlip);
            }

            if (showTileBorders) {
                Pen borderPen = new Pen(Color.LawnGreen, 1 / Zoom);
                foreach (Sprite.Tile tile in currentSprite.tiles) {
                    e.Graphics.DrawRectangle(borderPen, tile.xOffset, tile.yOffset, 16, 16);
                }
                borderPen.Dispose();
            }

            objectSelector.DrawSelectionRectangle(e.Graphics, Zoom);

            Pen p = new Pen(Color.White, 1 / Zoom);
            foreach (WrappedSpriteTile tile in selectedObjects) {
                Rectangle bounds = tile.Bounds;
                e.Graphics.FillRectangle(selectionFillBrush, bounds);
                e.Graphics.DrawRectangle(p, bounds);
            }
            p.Dispose();
        }

        public IEnumerable<WrappedSpriteTile> GetObjects() {
            if (currentSprite == null) {
                yield break;
            }
            foreach (Sprite.Tile tile in currentSprite.tiles) {
                yield return new WrappedSpriteTile(tile);
            }
        }

        private HashSet<WrappedSpriteTile> selectedObjects = new HashSet<WrappedSpriteTile>();
        private int updatingUI = 0;

        void ObjectSelector<WrappedSpriteTile>.IHost.SelectionChanged() {
            Repaint();
            if (!selectedObjects.SetEquals(objectSelector.GetSelectedObjects())) {
                selectedObjects = new HashSet<WrappedSpriteTile>(objectSelector.GetSelectedObjects());
                RaiseSelectionChanged();
                PropertyBrowserObjects = selectedObjects.ToArray();

                UpdateSelectedPalette();
                if (selectedObjects.Count == 1) {
                    UpdateUI(() => tilePicker.SelectedTile = selectedObjects.First().tile.tileNum);
                }
                UpdateToolbar();
            }
        }

        public void UpdateSelectedPalette() {
            if (selectedObjects.Count >= 1) {
                int palette = selectedObjects.First().tile.palette;
                tilePicker.Palette = palette;
                if (selectedObjects.All(t => t.tile.palette == palette)) {
                    UpdateUI(() => paletteButtons[palette].Checked = true);
                } else {
                    foreach (RadioButton button in paletteButtons) {
                        button.Checked = false;
                    }
                }
            }
        }

        private void UpdateUI(Action action) {
            updatingUI++;
            action();
            updatingUI--;
        }

        public void MoveSelectedObjects(int dx, int dy) {
            undoManager.Do(new MoveSpriteTileAction(currentSprite, selectedObjects, dx, dy));
        }

        public WrappedSpriteTile CreateObject(int x, int y) {
            if (currentSprite != null && tilePicker.SelectedTile >= 0) {
                WrappedSpriteTile tile = new WrappedSpriteTile(new Sprite.Tile {
                    palette = tilePicker.Palette,
                    tileNum = (ushort)tilePicker.SelectedTile,
                    xOffset = (short)(x - 8),
                    yOffset = (short)(y - 8),
                    xFlip = false,
                    yFlip = false
                });
                undoManager.Do(new AddSpriteTileAction(currentSprite, new[] { tile }));
                return tile;
            }
            return null;
        }

        public IEnumerable<WrappedSpriteTile> CloneSelection() {
            List<WrappedSpriteTile> clone = selectedObjects.Select(t => t.tile).JsonClone().Select(t => new WrappedSpriteTile(t)).ToList();
            undoManager.Do(new AddSpriteTileAction(currentSprite, clone));
            return clone;
        }

        private void tilePicker_SelectedTileChanged(object sender, EventArgs e) {
            if (updatingUI == 0 && tilePicker.SelectedTile >= 0 && selectedObjects.Count > 0) {
                undoManager.Do(new ChangeSpriteTileNumAction(currentSprite, selectedObjects, (ushort)tilePicker.SelectedTile));
            }
        }

        private bool mouseDown = false;

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MouseEventArgs transformed = scrollWrapper.TransformMouseArgs(e);
                objectSelector.MouseDown(transformed.X, transformed.Y);
                mouseDown = true;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            MouseEventArgs transformed = scrollWrapper.TransformMouseArgs(e);
            objectSelector.MouseMove(transformed.X, transformed.Y);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            if (mouseDown) {
                objectSelector.MouseUp();
                mouseDown = false;
            }
        }

        private void canvas_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) {
                e.IsInputKey = true;
            }
        }

        private static readonly Dictionary<Keys, int> paletteKeys = new Dictionary<Keys, int>() {
            { Keys.D0, 0 }, { Keys.D1, 1 }, { Keys.D2, 2 }, { Keys.D3, 3 }, { Keys.D4, 4 }, { Keys.D5, 5 }, { Keys.D6, 6 },{ Keys.D7, 7 },
            { Keys.NumPad0, 0 }, { Keys.NumPad1, 1 }, { Keys.NumPad2, 2 }, { Keys.NumPad3, 3 }, { Keys.NumPad4, 4 }, { Keys.NumPad5, 5 }, { Keys.NumPad6, 6 }, { Keys.NumPad7, 7 }
        };

        private void canvas_KeyDown(object sender, KeyEventArgs e) {
            objectSelector.KeyDown(e.KeyData);
            if (paletteKeys.TryGetValue(e.KeyCode, out int palette)) {
                paletteButtons[palette].Checked = true;
            }
        }
        
        public void AddTiles(Sprite s, List<WrappedSpriteTile> tiles, List<int> zIndexes = null) {
            for (int i = 0; i < tiles.Count; i++) {
                if (zIndexes == null) {
                    s.tiles.Add(tiles[i].tile);
                } else {
                    s.tiles.Insert(zIndexes[i], tiles[i].tile);
                }
            }
        }

        public void RemoveTiles(Sprite s, List<WrappedSpriteTile> tiles, bool updateSelection = true) {
            foreach (WrappedSpriteTile tile in tiles) {
                s.tiles.Remove(tile.tile);
            }
            if (updateSelection) {
                objectSelector.UpdateSelection();
            }
        }

        private void paletteButton_CheckedChanged(object sender, EventArgs e) {
            if (updatingUI == 0 && sender is RadioButton button && button.Checked) {
                int palette = Array.IndexOf(paletteButtons, button);
                tilePicker.Palette = palette;
                undoManager.Do(new ChangeSpriteTilePaletteAction(currentSprite, selectedObjects, palette));
                canvas.Focus();
            }
        }
    }
}
