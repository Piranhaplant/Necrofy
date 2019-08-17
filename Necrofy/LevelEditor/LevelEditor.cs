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
    partial class LevelEditor : EditorWindow
    {
        private const int LevelPadding = 64;

        public readonly LoadedLevel level;

        public readonly TilesetObjectBrowserContents tilesetObjectBrowserContents;
        public readonly SpriteObjectBrowserContents spriteObjectBrowserContents;

        private readonly ScrollWrapper scrollWrapper;
        public UndoManager undoManager;

        public readonly Selection selection;
        private GraphicsPath selectionPath = null;
        private Rectangle eraserRect = Rectangle.Empty;

        private readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 0, 0, 0));
        private readonly SolidBrush eraserFillBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        private readonly Pen selectionBorderDashPen = new Pen(Color.Black) {
            DashOffset = 0,
            DashPattern = new float[] { 4f, 4f },
        };

        private readonly PaintbrushTool paintbrushTool;
        private readonly RectangleSelectTool rectangleSelectTool;
        private readonly PencilSelectTool pencilSelectTool;
        private readonly TileSelectTool tileSelectTool;
        private readonly SpriteTool spriteTool;

        private readonly Dictionary<Tool, ToolStripMenuItem> toolMenuItems;
        private readonly Dictionary<Tool.ObjectType, ObjectBrowserContents> toolTypeToObjectContents;

        private ObjectBrowserContents currentContents;
        private Tool currentTool;

        public LevelEditor(LoadedLevel level) {
            InitializeComponent();

            this.level = level;
            this.Text = level.levelAsset.GetDisplayText();

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.SetClientSize(level.Level.width * 64 + LevelPadding * 2, level.Level.height * 64 + LevelPadding * 2);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;

            selection = new Selection(level.Level.width, level.Level.height);
            selection.Changed += Selection_Changed;
            
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
            rectangleSelectTool = new RectangleSelectTool(this);
            pencilSelectTool = new PencilSelectTool(this);
            tileSelectTool = new TileSelectTool(this);
            spriteTool = new SpriteTool(this);

            toolMenuItems = new Dictionary<Tool, ToolStripMenuItem> {
                { paintbrushTool, toolsPaintbrush },
                { rectangleSelectTool, toolsRectangleSelect },
                { pencilSelectTool, toolsPencilSelect },
                { tileSelectTool, toolsTileSelect },
                { spriteTool, toolsSprites },
            };
            ToolBarMenuLinker.Link(paintbrushButton, toolsPaintbrush);
            ToolBarMenuLinker.Link(rectangleSelectButton, toolsRectangleSelect);
            ToolBarMenuLinker.Link(pencilSelectButton, toolsPencilSelect);
            ToolBarMenuLinker.Link(tileSelectButton, toolsTileSelect);
            ToolBarMenuLinker.Link(spritesButton, toolsSprites);
            
            Repaint();
        }
        
        public void ScrollObjectBrowserToSelection() {
            mainWindow.ObjectBrowser.Browser.ScrollToSelection();
        }

        public void FillSelection() {
            if (tilesetObjectBrowserContents.SelectedIndex >= 0 && selectionPath != null) {
                undoManager.Do(new FillSelectionAction((ushort)tilesetObjectBrowserContents.SelectedIndex));
            }
        }

        protected override void FirstDisplayed() {
            undoManager = new UndoManager(mainWindow.UndoButton, mainWindow.RedoButton, this);
            ChangeTool(paintbrushTool);
            mainWindow.ObjectBrowser.Activate();
        }

        public override void Displayed() {
            mainWindow.ObjectBrowser.Browser.Contents = currentContents;
            undoManager.RefreshItems();
        }

        private void Selection_Changed(object sender, EventArgs e) {
            if (selectionPath != null) {
                selectionPath.Dispose();
            }
            selectionPath = selection.GetGraphicsPath();
            eraserRect = selection.GetEraserRectangle();
            Repaint();
        }

        public override void Undo() {
            undoManager.UndoLast();
        }

        public override void Redo() {
            undoManager.RedoLast();
        }

        private void ChangeTool(Tool tool) {
            if (tool != currentTool) {
                currentTool = tool;
                currentContents = toolTypeToObjectContents[tool.objectType];
                mainWindow.ObjectBrowser.Browser.Contents = currentContents;
                
                foreach (ToolStripMenuItem menuItem in toolMenuItems.Values) {
                    menuItem.Checked = false;
                }
                toolMenuItems[tool].Checked = true;
            }
        }

        private void TilesetObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            currentTool.TileChanged();
        }

        private void SpriteObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            currentTool.SpriteChanged();
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
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

            if (selectionPath != null) {
                e.Graphics.FillPath(selectionFillBrush, selectionPath);
                e.Graphics.DrawPath(Pens.White, selectionPath);
                e.Graphics.DrawPath(selectionBorderDashPen, selectionPath);
            }
            if (eraserRect != Rectangle.Empty) {
                e.Graphics.FillRectangle(eraserFillBrush, eraserRect);
                e.Graphics.DrawRectangle(Pens.White, eraserRect);
                e.Graphics.DrawRectangle(selectionBorderDashPen, eraserRect);
            }

            currentTool.Paint(e.Graphics);
        }

        // Double clicking on the title bar to maximize the window causes mouse move/up events to be sent without a mouse down. This is used to ignore those.
        private bool mouseDown = false;
        private void canvas_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseDown(args);
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseUp(args);
            }
            mouseDown = false;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseMove(args);
            }
        }

        private bool TransformMouseArgs(MouseEventArgs e, out LevelMouseEventArgs ret) {
            int x = e.X - scrollWrapper.LeftPosition - LevelPadding;
            int y = e.Y - scrollWrapper.TopPosition - LevelPadding;
            if (mouseDown && e.Button == MouseButtons.Left) {
                ret = new LevelMouseEventArgs(e.Button, e.Clicks, x, y, e.Delta, level.Level.width, level.Level.height);
                return true;
            }
            ret = null;
            return false;
        }

        private void paintbrush_Click(object sender, EventArgs e) {
            ChangeTool(paintbrushTool);
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
