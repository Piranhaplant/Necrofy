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

        private const string DefaultStatus = "Click to select or move. Hold shift to add to the selection. Hold alt to remove from the selection. Double click in the tile panel or hold ctrl and click on the sprite area to create a new tile.";
        private const string DragStatus = "Move: {0}, {1}. Hold Shift to snap to 8x8 pixel grid.";

        private static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 255, 255, 255));

        private readonly RadioButton[] paletteButtons;

        private readonly LoadedSprites loadedSprites;
        private readonly SpriteEditorObjectBrowserContents browserContents;
        private readonly ObjectSelector<WrappedSpriteTile> objectSelector;

        public readonly ScrollWrapper scrollWrapper;
        public UndoManager<SpriteEditor> undoManager;
        private HashSet<Sprite> modifiedSprites = new HashSet<Sprite>();

        private Sprite currentSprite = null;
        private bool showAxes;
        private bool showTileBorders;
        private bool showGrid;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.SpriteEditor;
        private static readonly ToolStripGrouper.ItemType[] tileSelectedItems = new ToolStripGrouper.ItemType[] {
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

            UpdateStatus();
        }

        private void SpriteEditor_Disposed(object sender, EventArgs e) {
            loadedSprites.Dispose();
        }

        protected override UndoManager Setup() {
            Zoom = 2.0f;
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
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewSpriteGrid).Checked;
        }

        private void UpdateToolbar() {
            foreach (ToolStripGrouper.ItemType item in tileSelectedItems) {
                mainWindow.GetToolStripItem(item).Enabled = selectedObjects.Count > 0;
            }
        }

        protected override void DoSave(Project project) {
            loadedSprites.spritesAsset.WriteFile(project);
            UpdateSpritePreviews();
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
            if (item == ToolStripGrouper.ItemType.ViewAxes || item == ToolStripGrouper.ItemType.ViewTileBorders || item == ToolStripGrouper.ItemType.ViewSpriteGrid) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            object value = e.ChangedItem.Value;
            foreach (WrappedSpriteTile o in selectedObjects) {
                o.ClearBrowsableProperties();
            }
            if (value is string stringValue) {
                stringValue = stringValue.Trim();
                switch (e.ChangedItem.Label) {
                    case WrappedSpriteTile.XProperty:
                        objectSelector.ParsePositionChange(stringValue, isX: true);
                        break;
                    case WrappedSpriteTile.YProperty:
                        objectSelector.ParsePositionChange(stringValue, isX: false);
                        break;
                    case WrappedSpriteTile.PaletteProperty:
                        if (int.TryParse(stringValue, out int palette) && palette >= 0 && palette <= 7) {
                            undoManager.Do(new ChangeSpriteTilePaletteAction(currentSprite, selectedObjects, palette));
                        }
                        break;
                    case WrappedSpriteTile.TileNumProperty:
                        if (ushort.TryParse(stringValue, out ushort tileNum)) {
                            undoManager.Do(new ChangeSpriteTileNumAction(currentSprite, selectedObjects, tileNum));
                        }
                        break;
                }
            } else if (value is bool boolValue) {
                switch (e.ChangedItem.Label) {
                    case WrappedSpriteTile.XFlipProperty:
                        undoManager.Do(new SetSpriteTilesXFlip(currentSprite, selectedObjects, boolValue));
                        break;
                    case WrappedSpriteTile.YFlipProperty:
                        undoManager.Do(new SetSpriteTilesYFlip(currentSprite, selectedObjects, boolValue));
                        break;
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
            UpdateSpritePreviews();
            browserContents.SelectedIndex = Array.IndexOf(loadedSprites.spritesAsset.sprites, currentSprite);
            Repaint();
        }
        
        private void scrollWrapper_Scrolled(object sender, EventArgs e) {
            canvas.GenerateMouseMove();
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

            using (Pen linePen = new Pen(Color.White, 1 / Zoom)) {
                foreach (Sprite.Tile tile in currentSprite.tiles) {
                    if (tile.tileNum < loadedSprites.tileImages.Length) {
                        SNESGraphics.DrawWithPlt(e.Graphics, tile.xOffset, tile.yOffset, loadedSprites.tileImages[tile.tileNum], loadedSprites.loadedPalette.colors, tile.palette * 0x10, 0x10, tile.xFlip, tile.yFlip);
                    } else {
                        e.Graphics.DrawLine(linePen, tile.xOffset, tile.yOffset, tile.xOffset + 16, tile.yOffset + 16);
                        e.Graphics.DrawLine(linePen, tile.xOffset + 16, tile.yOffset, tile.xOffset, tile.yOffset + 16);
                    }
                }

                if (showGrid) {
                    using (Pen gridPen = new Pen(Color.Gray, 1 / Zoom)) {
                        for (int x = -MaxDimension + 16; x < MaxDimension; x += 16) {
                            e.Graphics.DrawLine(gridPen, x, -MaxDimension, x, MaxDimension);
                        }
                        for (int y = -MaxDimension + 16; y < MaxDimension; y += 16) {
                            e.Graphics.DrawLine(gridPen, -MaxDimension, y, MaxDimension, y);
                        }
                    }
                    using (Pen axesPen = new Pen(Color.BlueViolet, 1 / Zoom)) {
                        e.Graphics.DrawLine(axesPen, 0, -MaxDimension, 0, MaxDimension);
                        e.Graphics.DrawLine(axesPen, -MaxDimension, 0, MaxDimension, 0);
                    }
                }

                if (showTileBorders) {
                    using (Pen borderPen = new Pen(Color.LawnGreen, 1 / Zoom)) {
                        foreach (Sprite.Tile tile in currentSprite.tiles) {
                            e.Graphics.DrawRectangle(borderPen, tile.xOffset, tile.yOffset, 16, 16);
                        }
                    }
                }

                objectSelector.DrawSelectionRectangle(e.Graphics, Zoom);

                foreach (WrappedSpriteTile tile in selectedObjects) {
                    Rectangle bounds = tile.Bounds;
                    e.Graphics.FillRectangle(selectionFillBrush, bounds);
                    e.Graphics.DrawRectangle(linePen, bounds);
                }
            }
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

        public void MoveSelectedObjects(int dx, int dy, int snap) {
            undoManager.Do(new MoveSpriteTileAction(currentSprite, selectedObjects, dx, dy, snap));
            UpdateStatus();
        }

        public void SetSelectedObjectsPosition(int? x, int? y) {
            undoManager.Do(new MoveSpriteTileAction(currentSprite, selectedObjects, (short?)x, (short?)y));
        }

        public WrappedSpriteTile CreateObject(int x, int y) {
            WrappedSpriteTile tile = GetCreationObject(x, y);
            if (tile != null) {
                undoManager.Do(new AddSpriteTileAction(currentSprite, new[] { tile }));
                return tile;
            }
            return null;
        }

        private WrappedSpriteTile GetCreationObject(int x, int y) {
            if (currentSprite != null && tilePicker.SelectedTile >= 0) {
                return new WrappedSpriteTile(new Sprite.Tile {
                    palette = tilePicker.Palette,
                    tileNum = (ushort)tilePicker.SelectedTile,
                    xOffset = (short)(x - 8),
                    yOffset = (short)(y - 8),
                    xFlip = false,
                    yFlip = false
                });
            }
            return null;
        }

        public IEnumerable<WrappedSpriteTile> CloneSelection() {
            List<WrappedSpriteTile> clone = selectedObjects.Select(t => t.tile).JsonClone().Select(t => new WrappedSpriteTile(t)).ToList();
            undoManager.Do(new AddSpriteTileAction(currentSprite, clone));
            return clone;
        }

        ChangeSpriteTileNumAction prevChangeTileAction1 = null;
        ChangeSpriteTileNumAction prevChangeTileAction2 = null;

        private void tilePicker_SelectedTileChanged(object sender, EventArgs e) {
            if (updatingUI == 0) {
                prevChangeTileAction2 = prevChangeTileAction1;
                if (tilePicker.SelectedTile >= 0 && selectedObjects.Count > 0) {
                    prevChangeTileAction1 = new ChangeSpriteTileNumAction(currentSprite, selectedObjects, (ushort)tilePicker.SelectedTile);
                    undoManager.Do(prevChangeTileAction1);
                } else {
                    prevChangeTileAction1 = null;
                }
            }
        }

        private void tilePicker_TileDoubleClicked(object sender, EventArgs e) {
            Point center = scrollWrapper.GetViewCenter();
            WrappedSpriteTile newObject = GetCreationObject(center.X, center.Y);
            if (newObject != null) {
                undoManager.Revert(prevChangeTileAction1);
                undoManager.Revert(prevChangeTileAction2);
                undoManager.Do(new AddSpriteTileAction(currentSprite, new[] { newObject }));

                objectSelector.SelectObjects(new[] { newObject });
                Activate();
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Point transformed = scrollWrapper.TransformPoint(e.Location);
                objectSelector.MouseDown(transformed.X, transformed.Y);
                UpdateStatus();
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            objectSelector.MouseMove(transformed.X, transformed.Y);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                objectSelector.MouseUp();
                UpdateStatus();
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
            } else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.L)) {
                //CopySpriteASMText(); // TODO: Remove
            }
        }

        private void CopySpriteASMText() {
            if (currentSprite == null) {
                return;
            }
            StringBuilder s = new StringBuilder();
            s.AppendLine($"db #{currentSprite.tiles.Count}");
            for (int i = currentSprite.tiles.Count - 1; i >= 0; i--) {
                Sprite.Tile tile = currentSprite.tiles[i];
                s.AppendLine($"dw #{tile.xOffset}");
                s.AppendLine($"dw #{tile.yOffset}");
                s.AppendLine($"dw #${tile.GetProperties():X4}");
                s.AppendLine($"dw #${tile.tileNum + 0xC03:X4}");
            }
            Clipboard.SetText(s.ToString());
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

        public void AddModifiedSprite(Sprite sprite) {
            modifiedSprites.Add(sprite);
        }

        private void UpdateSpritePreviews() {
            foreach (Sprite s in modifiedSprites) {
                int index = Array.IndexOf(loadedSprites.spritesAsset.sprites, s);
                loadedSprites.LoadSprite(index);
            }
            modifiedSprites.Clear();
            browserContents.Refresh();
        }

        private void paletteButton_CheckedChanged(object sender, EventArgs e) {
            if (updatingUI == 0 && sender is RadioButton button && button.Checked) {
                int palette = Array.IndexOf(paletteButtons, button);
                tilePicker.Palette = palette;
                undoManager.Do(new ChangeSpriteTilePaletteAction(currentSprite, selectedObjects, palette));
                canvas.Focus();
            }
        }

        private void UpdateStatus() {
            if (objectSelector.MovingObjects) {
                Status = string.Format(DragStatus, objectSelector.TotalMoveX, objectSelector.TotalMoveY);
            } else {
                Status = DefaultStatus;
            }
        }
    }
}
