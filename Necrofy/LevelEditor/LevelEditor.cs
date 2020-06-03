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
        public TitleEditor titleEditor = null;

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
        
        private readonly SpriteTool spriteTool;
        
        private readonly Dictionary<Tool.ObjectType, ObjectBrowserContents> toolTypeToObjectContents;

        private readonly Dictionary<Keys, Tool> toolShortcutKeys = new Dictionary<Keys, Tool>();
        private readonly Dictionary<ToolStripGrouper.ItemType, Tool> toolForItemType = new Dictionary<ToolStripGrouper.ItemType, Tool>();
        private readonly Dictionary<Tool, ToolStripGrouper.ItemType> itemTypeForTool = new Dictionary<Tool, ToolStripGrouper.ItemType>();

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.LevelEditor;

        private Tool currentTool;
        
        public LevelEditor(LoadedLevel level) {
            InitializeComponent();
            FormClosed += LevelEditor_FormClosed;

            this.level = level;
            UpdateTitle();

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
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.LevelMonster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Monster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.OneShotMonster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Victim);

            toolTypeToObjectContents = new Dictionary<Tool.ObjectType, ObjectBrowserContents> {
                { Tool.ObjectType.Sprites, spriteObjectBrowserContents },
                { Tool.ObjectType.Tiles, tilesetObjectBrowserContents },
            };
            
            SetupTool(new PaintbrushTool(this), ToolStripGrouper.ItemType.PaintbrushTool, Keys.P);
            SetupTool(new TileSuggestionTool(this), ToolStripGrouper.ItemType.TileSuggestTool, Keys.S);
            SetupTool(new RectangleSelectTool(this), ToolStripGrouper.ItemType.RectangleSelectTool, Keys.R);
            SetupTool(new PencilSelectTool(this), ToolStripGrouper.ItemType.PencilSelectTool, Keys.C);
            SetupTool(new TileSelectTool(this), ToolStripGrouper.ItemType.TileSelectTool, Keys.T);
            SetupTool(new ResizeLevelTool(this), ToolStripGrouper.ItemType.ResizeLevelTool, Keys.L);
            spriteTool = new SpriteTool(this);
            SetupTool(spriteTool, ToolStripGrouper.ItemType.SpriteTool, Keys.I);

            level.tileAnimator.Animated += TileAnimator_Animated;
            level.tileAnimator.Run();

            Repaint();
        }

        protected override void CloseChildren(FormClosingEventArgs e) {
            if (titleEditor != null) {
                titleEditor.Close();
                if (titleEditor != null && titleEditor.Visible) {
                    e.Cancel = true;
                }
            }
        }
        
        private void LevelEditor_FormClosed(object sender, FormClosedEventArgs e) {
            level.Dispose();
            tileSelectionPath?.Dispose();
        }

        private void SetupTool(Tool tool, ToolStripGrouper.ItemType itemType, Keys shortcutKeys) {
            toolShortcutKeys[shortcutKeys] = tool;
            toolForItemType[itemType] = tool;
            itemTypeForTool[tool] = itemType;
        }

        protected override UndoManager Setup() {
            undoManager = new UndoManager<LevelEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public void UpdateTitle() {
            Title = level.levelAsset.LevelNumber.ToString() + " " + level.Level.displayName;
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

        public void SetPropertyBrowserObjects(object[] objects) {
            PropertyBrowserObjects = objects;
        }

        public void GenerateMouseMove() {
            if (currentTool != null) {
                Point mousePosition = PointToClient(MousePosition);
                DoMouseMove(new MouseEventArgs(MouseButtons, 0, mousePosition.X, mousePosition.Y, 0));
            }
        }

        public void UpdateLevelSize() {
            scrollWrapper.SetClientSize(level.Level.width * 64 + LevelPadding * 2, level.Level.height * 64 + LevelPadding * 2);
        }

        public void UpdateSpriteSelection() {
            spriteTool.UpdateSelection();
        }

        public override void Displayed() {
            base.Displayed();
            foreach (ToolStripGrouper.ItemType type in spriteCategoryForMenuItem.Keys) {
                mainWindow.GetToolStripItem(type).Checked = spriteCategoryEnabled[spriteCategoryForMenuItem[type]];
            }
            foreach (ToolStripGrouper.ItemType type in toolForItemType.Keys) {
                if (mainWindow.GetToolStripItem(type).Checked) {
                    ChangeTool(toolForItemType[type]);
                    return;
                }
            }
            ChangeTool(spriteTool);
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
                PropertyBrowserObjects = tool.PropertyBrowserObjects;
                
                foreach (ToolStripGrouper.ItemType type in toolForItemType.Keys) {
                    mainWindow.GetToolStripItem(type).Checked = false;
                }
                mainWindow.GetToolStripItem(itemTypeForTool[tool]).Checked = true;

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

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            currentTool.PropertyBrowserPropertyChanged(e);
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
            if (MouseButtons == MouseButtons.Left) {
                GenerateMouseMove();
            }
            Repaint();
        }

        private void TileAnimator_Animated(object sender, EventArgs e) {
            if (DockVisible) {
                Repaint();
                tilesetObjectBrowserContents.Repaint();
            }
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

            foreach (WrappedLevelObject obj in level.GetAllObjects()) {
                obj.Render(e.Graphics);
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
        // Used to ignore mouse move events that are generated by setting the property grid objects
        private Point prevMousePosition = new Point(int.MinValue, int.MinValue);

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
            if (e.Location == prevMousePosition) {
                return;
            }
            prevMousePosition = e.Location;
            DoMouseMove(e);
        }

        private void DoMouseMove(MouseEventArgs e) {
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseMove(args);
            }
        }

        private void canvas_MouseLeave(object sender, EventArgs e) {
            prevMousePosition = new Point(int.MinValue, int.MinValue);
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

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            if (toolForItemType.TryGetValue(item, out Tool tool)) {
                ChangeTool(tool);
            } else if (item == ToolStripGrouper.ItemType.SpritesAll) {
                foreach (ToolStripGrouper.ItemType type in spriteCategoryForMenuItem.Keys) {
                    mainWindow.GetToolStripItem(type).Checked = true;
                }
            } else if (item == ToolStripGrouper.ItemType.LevelEditTitle) {
                if (titleEditor == null) {
                    titleEditor = new TitleEditor(this, project);
                    titleEditor.FormClosed += TitleEditor_FormClosed;
                    mainWindow.ShowEditor(titleEditor, assetInfo: null);
                } else {
                    titleEditor.Activate();
                }
            } else if (item == ToolStripGrouper.ItemType.LevelSettings) {
                new LevelSettingsDialog(project, this).ShowDialog();
            } else if (item == ToolStripGrouper.ItemType.LevelClear) {
                undoManager.Do(new ClearLevelAction((ushort)Math.Max(tilesetObjectBrowserContents.SelectedTile, 0)));
            }
        }

        private void TitleEditor_FormClosed(object sender, FormClosedEventArgs e) {
            titleEditor = null;
        }

        private readonly Dictionary<ToolStripGrouper.ItemType, SpriteDisplay.Category> spriteCategoryForMenuItem =
            new Dictionary<ToolStripGrouper.ItemType, SpriteDisplay.Category>() {
                { ToolStripGrouper.ItemType.SpritesItems, SpriteDisplay.Category.Item },
                { ToolStripGrouper.ItemType.SpritesVictims, SpriteDisplay.Category.Victim },
                { ToolStripGrouper.ItemType.SpritesOneShotMonsters, SpriteDisplay.Category.OneShotMonster },
                { ToolStripGrouper.ItemType.SpritesMonsters, SpriteDisplay.Category.Monster },
                { ToolStripGrouper.ItemType.SpritesBossMonsters, SpriteDisplay.Category.LevelMonster },
                { ToolStripGrouper.ItemType.SpritesPlayers, SpriteDisplay.Category.Player }
            };

        private readonly Dictionary<SpriteDisplay.Category, bool> spriteCategoryEnabled =
            new Dictionary<SpriteDisplay.Category, bool>() {
                { SpriteDisplay.Category.Item, true },
                { SpriteDisplay.Category.Victim, true },
                { SpriteDisplay.Category.OneShotMonster, true },
                { SpriteDisplay.Category.Monster, true },
                { SpriteDisplay.Category.LevelMonster, true },
                { SpriteDisplay.Category.Player, true }
            };

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            if (spriteCategoryForMenuItem.TryGetValue(item, out SpriteDisplay.Category category)) {
                UpdateSpriteCategory(category, mainWindow.GetToolStripItem(item).Checked);
            }
        }

        public bool ItemsEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Item];
        public bool VictimsEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Victim];
        public bool OneShotMonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.OneShotMonster];
        public bool MonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Monster];
        public bool BossMonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.LevelMonster];
        public bool PlayersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Player];

        private void UpdateSpriteCategory(SpriteDisplay.Category category, bool enabled) {
            spriteCategoryEnabled[category] = enabled;
            if (enabled && category != SpriteDisplay.Category.Player) {
                spriteObjectBrowserContents.AddCategory(category);
            } else {
                spriteObjectBrowserContents.RemoveCategory(category);
            }
            ChangeTool(spriteTool);
            UpdateSpriteSelection();
        }
    }
}
