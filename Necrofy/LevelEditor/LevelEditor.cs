using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using System.Windows.Forms.Layout;

namespace Necrofy
{
    partial class LevelEditor : EditorWindow {
        private const int LevelPadding = 64;

        public readonly LoadedLevel level;

        public readonly TilesetObjectBrowserContents tilesetObjectBrowserContents;
        public readonly SpriteObjectBrowserContents spriteObjectBrowserContents;

        public readonly ScrollWrapper scrollWrapper;
        public UndoManager<LevelEditor> undoManager;

        public readonly TileSelection tileSelection;
        private GraphicsPath tileSelectionPath = null;
        private Rectangle tileSelectionEraserRect = Rectangle.Empty;

        public static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 0, 0, 0));
        public static readonly SolidBrush eraserFillBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        public static readonly Pen selectionBorderDashPen = new Pen(Color.Black) {
            DashOffset = 0,
            DashPattern = new float[] { 4f, 4f },
        };

        private readonly PaintbrushTool paintbrushTool;
        private readonly TileSuggestionTool tileSuggestionTool;
        private readonly RectangleSelectTool rectangleSelectTool;
        private readonly PencilSelectTool pencilSelectTool;
        private readonly TileSelectTool tileSelectTool;
        private readonly ResizeLevelTool resizeLevelTool;
        private readonly SpriteTool spriteTool;

        private readonly Dictionary<Tool, ToolStripMenuItem> toolMenuItems = new Dictionary<Tool, ToolStripMenuItem>();
        private readonly Dictionary<Tool.ObjectType, ObjectBrowserContents> toolTypeToObjectContents;
        private readonly Dictionary<Keys, Tool> toolShortcutKeys = new Dictionary<Keys, Tool>();
        
        private Tool currentTool;

        public LevelEditor(LoadedLevel level) {
            InitializeComponent();

            this.level = level;
            this.Text = level.levelAsset.DisplayText;

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
            UpdateLevelSize();

            tileSelection = new TileSelection(level.Level.width, level.Level.height);
            tileSelection.Changed += TileSelection_Changed;

            tilesetObjectBrowserContents = new TilesetObjectBrowserContents(level);
            tilesetObjectBrowserContents.SelectedIndexChanged += TilesetObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents = new SpriteObjectBrowserContents(level.spriteGraphics);
            spriteObjectBrowserContents.SelectedIndexChanged += SpriteObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Item);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Victim);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.OneTimeMonster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Monster);

            toolTypeToObjectContents = new Dictionary<Tool.ObjectType, ObjectBrowserContents> {
                { Tool.ObjectType.Sprites, spriteObjectBrowserContents },
                { Tool.ObjectType.Tiles, tilesetObjectBrowserContents },
            };

            paintbrushTool = new PaintbrushTool(this);
            tileSuggestionTool = new TileSuggestionTool(this);
            rectangleSelectTool = new RectangleSelectTool(this);
            pencilSelectTool = new PencilSelectTool(this);
            tileSelectTool = new TileSelectTool(this);
            resizeLevelTool = new ResizeLevelTool(this);
            spriteTool = new SpriteTool(this);

            SetupTool(paintbrushTool, Keys.P, toolsPaintbrush, paintbrushButton);
            SetupTool(tileSuggestionTool, Keys.S, toolsTileSuggest, tileSuggestButton);
            SetupTool(rectangleSelectTool, Keys.R, toolsRectangleSelect, rectangleSelectButton);
            SetupTool(pencilSelectTool, Keys.C, toolsPencilSelect, pencilSelectButton);
            SetupTool(tileSelectTool, Keys.T, toolsTileSelect, tileSelectButton);
            SetupTool(resizeLevelTool, Keys.L, toolsResizeLevel, resizeLevelButton);
            SetupTool(spriteTool, Keys.I, toolsSprites, spritesButton);
            
            Repaint();
        }

        private void SetupTool(Tool tool, Keys shortcutKey, ToolStripMenuItem menuItem, ToolStripItem toolStripButton) {
            toolMenuItems[tool] = menuItem;
            toolShortcutKeys[shortcutKey] = tool;
            ToolBarMenuLinker.Link(toolStripButton, menuItem);
            menuItem.ShortcutKeyDisplayString = shortcutKey.ToString();
        }

        public void ScrollObjectBrowserToSelection() {
            mainWindow.ObjectBrowser.Browser.ScrollToSelection();
        }

        public void FillSelection() {
            if (tilesetObjectBrowserContents.SelectedIndex >= 0 && TileSelectionExists) {
                undoManager.Do(new FillSelectionAction((ushort)tilesetObjectBrowserContents.SelectedIndex));
            }
        }

        public Point GetViewCenter() {
            return new Point(canvas.Width / 2 - scrollWrapper.LeftPosition - LevelPadding, canvas.Height / 2 - scrollWrapper.TopPosition - LevelPadding);
        }

        public void SetCursor(Cursor cursor) {
            canvas.Cursor = cursor;
        }

        public void GenerateMouseMove() {
            Point mousePosition = PointToClient(MousePosition);
            canvas_MouseMove(this, new MouseEventArgs(MouseButtons, 0, mousePosition.X, mousePosition.Y, 0));
        }

        public void UpdateLevelSize() {
            scrollWrapper.SetClientSize(level.Level.width * 64 + LevelPadding * 2, level.Level.height * 64 + LevelPadding * 2);
        }

        protected override UndoManager Setup() {
            undoManager = new UndoManager<LevelEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            ChangeTool(paintbrushTool);
            mainWindow.ObjectBrowser.Activate();
            return undoManager;
        }

        public override void Displayed() {
            base.Displayed();
        }

        protected override void DoSave(Project project) {
            level.levelAsset.WriteFile(project);
        }

        public override bool CanCopy => currentTool.CanCopy;
        public override bool CanPaste => currentTool.CanPaste;
        public override bool CanDelete => currentTool.CanDelete;
        public override bool HasSelection => currentTool.HasSelection;

        public override void Copy() {
            currentTool.Copy();
        }

        public override void Paste() {
            currentTool.Paste();
        }

        public override void Delete() {
            currentTool.Delete();
        }

        public override void SelectAll() {
            currentTool.SelectAll();
        }

        public override void SelectNone() {
            currentTool.SelectNone();
        }

        public override int? LevelNumber => level.levelAsset.LevelNumber;

        private void TileSelection_Changed(object sender, EventArgs e) {
            tileSelectionPath?.Dispose();
            tileSelectionPath = tileSelection.GetGraphicsPath();
            tileSelectionEraserRect = tileSelection.GetEraserRectangle();
            RaiseSelectionChanged();
            Repaint();
        }

        public bool TileSelectionExists => tileSelectionPath != null;

        public void NonTileSelectionChanged() {
            RaiseSelectionChanged();
        }
        
        private void ChangeTool(Tool tool) {
            if (tool != currentTool) {
                currentTool?.DoneBeingUsed();
                currentTool = tool;
                BrowserContents = toolTypeToObjectContents[tool.objectType];
                
                foreach (ToolStripMenuItem menuItem in toolMenuItems.Values) {
                    menuItem.Checked = false;
                }
                toolMenuItems[tool].Checked = true;

                SetCursor(Cursors.Default);
                RaiseSelectionChanged();
                Repaint();
            }
        }

        private void TilesetObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            currentTool.TileChanged();
        }

        private void SpriteObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            currentTool.SpriteChanged();
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
            if (MouseButtons == MouseButtons.Left) {
                GenerateMouseMove();
            }
            Repaint();
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (level == null) {
                return;
            }
            e.Graphics.TranslateTransform(scrollWrapper.LeftPosition + LevelPadding, scrollWrapper.TopPosition + LevelPadding);

            for (int y = 0; y < level.Level.height; y++) {
                for (int x = 0; x < level.Level.width; x++) {
                    e.Graphics.DrawImage(level.tiles[level.Level.background[x, y]], x * 64, y * 64);
                }
            }

            level.spriteGraphics.Render(SpriteDisplay.Key.Type.Player, 0, e.Graphics, level.Level.p1startX, level.Level.p1startY);
            level.spriteGraphics.Render(SpriteDisplay.Key.Type.Player, 1, e.Graphics, level.Level.p2startX, level.Level.p2startY);

            foreach (OneTimeMonster m in level.Level.oneTimeMonsters) {
                if (m.type == OneTimeMonster.CreditHeadType) {
                    level.spriteGraphics.Render(SpriteDisplay.Key.Type.CreditHead, m.extra, e.Graphics, m.x, m.y);
                } else {
                    level.spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, m.type, e.Graphics, m.x, m.y);
                }
            }
            foreach (Monster m in level.Level.monsters) {
                level.spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, m.type, e.Graphics, m.x, m.y);
            }
            foreach (Item i in level.Level.items) {
                level.spriteGraphics.Render(SpriteDisplay.Key.Type.Item, i.type, e.Graphics, i.x, i.y);
            }

            //for (int y = 0; y < level.Level.height; y++) {
            //    for (int x = 0; x < level.Level.width; x++) {
            //        e.Graphics.DrawImage(level.priorityTiles[level.Level.background[x, y]], x * 64, y * 64);
            //    }
            //}

            if (TileSelectionExists) {
                e.Graphics.FillPath(selectionFillBrush, tileSelectionPath);
                e.Graphics.DrawPath(Pens.White, tileSelectionPath);
                e.Graphics.DrawPath(selectionBorderDashPen, tileSelectionPath);
            }
            if (tileSelectionEraserRect != Rectangle.Empty) {
                e.Graphics.FillRectangle(eraserFillBrush, tileSelectionEraserRect);
                e.Graphics.DrawRectangle(Pens.White, tileSelectionEraserRect);
                e.Graphics.DrawRectangle(selectionBorderDashPen, tileSelectionEraserRect);
            }

            currentTool.Paint(e.Graphics);
        }

        // Double clicking on the title bar to maximize the window causes mouse up events to be sent without a mouse down. This is used to ignore those.
        private bool mouseDown = false;
        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseDown(args);
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out LevelMouseEventArgs args, mouseDownOnly: true)) {
                currentTool.MouseUp(args);
            }
            mouseDown = false;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseMove(args);
            }
        }

        private bool TransformMouseArgs(MouseEventArgs e, out LevelMouseEventArgs ret, bool mouseDownOnly = false) {
            int x = e.X - scrollWrapper.LeftPosition - LevelPadding;
            int y = e.Y - scrollWrapper.TopPosition - LevelPadding;
            if ((mouseDown && e.Button == MouseButtons.Left) || (!mouseDown && !mouseDownOnly)) {
                ret = new LevelMouseEventArgs(e.Button, e.Clicks, x, y, e.Delta, level.Level.width, level.Level.height, mouseDown);
                return true;
            }
            ret = null;
            return false;
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e) {
            if (toolShortcutKeys.TryGetValue(e.KeyData, out Tool t)) {
                mouseDown = false;
                ChangeTool(t);
            } else {
                currentTool.KeyDown(e);
            }
        }

        private void canvas_KeyUp(object sender, KeyEventArgs e) {
            currentTool.KeyUp(e);
        }

        private void paintbrush_Click(object sender, EventArgs e) {
            ChangeTool(paintbrushTool);
        }

        private void tileSuggest_Click(object sender, EventArgs e) {
            ChangeTool(tileSuggestionTool);
        }

        private void rectangleSelect_Click(object sender, EventArgs e) {
            ChangeTool(rectangleSelectTool);
        }

        private void pencilSelect_Click(object sender, EventArgs e) {
            ChangeTool(pencilSelectTool);
        }

        private void tileSelect_Click(object sender, EventArgs e) {
            ChangeTool(tileSelectTool);
        }

        private void resizeLevel_Click(object sender, EventArgs e) {
            ChangeTool(resizeLevelTool);
        }

        private void sprites_Click(object sender, EventArgs e) {
            ChangeTool(spriteTool);
        }

        private void UpdateSpriteCategory(SpriteDisplay.Category category, bool enabled) {
            if (enabled) {
                spriteObjectBrowserContents.AddCategory(category);
            } else {
                spriteObjectBrowserContents.RemoveCategory(category);
            }
            ChangeTool(spriteTool);
        }

        private void spritesItems_CheckedChanged(object sender, EventArgs e) {
            UpdateSpriteCategory(SpriteDisplay.Category.Item, spritesItems.Checked);
        }

        private void spritesVictims_CheckedChanged(object sender, EventArgs e) {
            UpdateSpriteCategory(SpriteDisplay.Category.Victim, spritesVictims.Checked);
        }

        private void spritesOneShotMonsters_CheckedChanged(object sender, EventArgs e) {
            UpdateSpriteCategory(SpriteDisplay.Category.OneTimeMonster, spritesOneShotMonsters.Checked);
        }

        private void spritesMonsters_CheckedChanged(object sender, EventArgs e) {
            UpdateSpriteCategory(SpriteDisplay.Category.Monster, spritesMonsters.Checked);
        }

        private void spritesBossMonsters_CheckedChanged(object sender, EventArgs e) {
            // TODO
        }

        private void spritesAll_Click(object sender, EventArgs e) {
            spritesItems.Checked = true;
            spritesVictims.Checked = true;
            spritesOneShotMonsters.Checked = true;
            spritesMonsters.Checked = true;
            spritesBossMonsters.Checked = true;
        }
    }
}
