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

        private readonly LoadedSprites loadedSprites;
        private readonly SpriteEditorObjectBrowserContents browserContents;
        private readonly ObjectSelector<WrappedSpriteTile> objectSelector;

        public readonly ScrollWrapper scrollWrapper;
        public UndoManager<SpriteEditor> undoManager;

        private Sprite currentSprite = null;

        public SpriteEditor(LoadedSprites loadedSprites) {
            InitializeComponent();
            Disposed += SpriteEditor_Disposed;

            this.loadedSprites = loadedSprites;
            Title = "Sprites";

            objectSelector = new ObjectSelector<WrappedSpriteTile>(this, maxX: MaxDimension, maxY: MaxDimension, minX: -MaxDimension, minY: -MaxDimension);

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
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

        protected override void DoSave(Project project) {
            loadedSprites.spritesAsset.WriteFile(project);
        }

        public override bool CanCopy => selectedObjects.Count > 0;
        public override bool CanPaste => true;
        public override bool CanDelete => selectedObjects.Count > 0;
        public override bool HasSelection => true;

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
            undoManager.Do(new DeleteSpriteTileAction(currentSprite, selectedObjects));
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

        public Point GetViewCenter() {
            return new Point(canvas.Width / 2 - scrollWrapper.LeftPosition - MaxDimension, canvas.Height / 2 - scrollWrapper.TopPosition - MaxDimension);
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

            e.Graphics.TranslateTransform(scrollWrapper.LeftPosition + MaxDimension, scrollWrapper.TopPosition + MaxDimension);
            e.Graphics.FillRectangle(SystemBrushes.ControlDark, new Rectangle(-MaxDimension, -MaxDimension, AllowedSize, AllowedSize));
            e.Graphics.DrawLine(Pens.Black, -MaxDimension, 0, MaxDimension, 0);
            e.Graphics.DrawLine(Pens.Black, 0, -MaxDimension, 0, MaxDimension);

            foreach (Sprite.Tile tile in currentSprite.tiles) {
                SNESGraphics.DrawWithPlt(e.Graphics, tile.xOffset, tile.yOffset, loadedSprites.tileImages[tile.tileNum], loadedSprites.loadedPalette.colors, tile.palette * 0x10, 0x10, tile.xFlip, tile.yFlip);
            }
            objectSelector.DrawSelectionRectangle(e.Graphics);
            foreach (WrappedSpriteTile tile in selectedObjects) {
                Rectangle bounds = tile.Bounds;
                e.Graphics.FillRectangle(selectionFillBrush, bounds);
                e.Graphics.DrawRectangle(Pens.White, bounds);
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
        private bool updatingSelectedTile = false;

        void ObjectSelector<WrappedSpriteTile>.IHost.SelectionChanged() {
            Repaint();
            if (!selectedObjects.SetEquals(objectSelector.GetSelectedObjects())) {
                selectedObjects = new HashSet<WrappedSpriteTile>(objectSelector.GetSelectedObjects());
                RaiseSelectionChanged();
                PropertyBrowserObjects = selectedObjects.ToArray();

                UpdateSelectedPalette();
                if (selectedObjects.Count == 1) {
                    updatingSelectedTile = true;
                    tilePicker.SelectedTile = selectedObjects.First().TileNum;
                    updatingSelectedTile = false;
                }
            }
        }

        public void UpdateSelectedPalette() {
            if (selectedObjects.Count >= 1) {
                tilePicker.Palette = selectedObjects.First().Palette;
            }
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
            if (!updatingSelectedTile && tilePicker.SelectedTile >= 0 && selectedObjects.Count > 0) {
                undoManager.Do(new ChangeSpriteTileNumAction(currentSprite, selectedObjects, (ushort)tilePicker.SelectedTile));
            }
        }

        private bool mouseDown = false;

        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                objectSelector.MouseDown(e.X - scrollWrapper.LeftPosition - MaxDimension, e.Y - scrollWrapper.TopPosition - MaxDimension);
                mouseDown = true;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            objectSelector.MouseMove(e.X - scrollWrapper.LeftPosition - MaxDimension, e.Y - scrollWrapper.TopPosition - MaxDimension);
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

        private void canvas_KeyDown(object sender, KeyEventArgs e) {
            objectSelector.KeyDown(e.KeyData);
        }

        public List<int> SortAndGetZIndexes(Sprite s, List<WrappedSpriteTile> tiles) {
            tiles.Sort((a, b) => s.tiles.IndexOf(a.tile).CompareTo(s.tiles.IndexOf(b.tile)));
            return tiles.Select(t => s.tiles.IndexOf(t.tile)).ToList();
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
    }
}
