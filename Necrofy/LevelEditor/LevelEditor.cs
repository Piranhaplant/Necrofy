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
        
        private readonly SpriteTool spriteTool;
        
        private readonly Dictionary<Tool.ObjectType, ObjectBrowserContents> toolTypeToObjectContents;

        private readonly Dictionary<Keys, Tool> toolShortcutKeys = new Dictionary<Keys, Tool>();
        private readonly Dictionary<ToolStripGrouper.ItemType, Tool> toolForItemType = new Dictionary<ToolStripGrouper.ItemType, Tool>();
        private readonly Dictionary<Tool, ToolStripGrouper.ItemType> itemTypeForTool = new Dictionary<Tool, ToolStripGrouper.ItemType>();

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.LevelEditor;

        private Tool currentTool;
        
        public LevelEditor(LoadedLevel level) {
            InitializeComponent();
            Disposed += LevelEditor_Disposed;

            this.level = level;
            UpdateTitle();

            scrollWrapper = new ScrollWrapper(canvas, hscroll, vscroll);
            scrollWrapper.SetPadding(LevelPadding, LevelPadding);
            scrollWrapper.Scrolled += scrollWrapper_Scrolled;
            UpdateLevelSize();

            tileSelection = new TileSelection(level.Level.width, level.Level.height);
            tileSelection.Changed += TileSelection_Changed;

            tilesetObjectBrowserContents = new TilesetObjectBrowserContents(level);
            tilesetObjectBrowserContents.SelectedIndexChanged += TilesetObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents = new SpriteObjectBrowserContents(level);
            spriteObjectBrowserContents.SelectedIndexChanged += SpriteObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents.DoubleClicked += SpriteObjectBrowserContents_DoubleClicked;
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

            level.TilesChanged += Level_GraphicsChanged;
            level.SpritesChanged += Level_GraphicsChanged;

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
        
        private void LevelEditor_Disposed(object sender, EventArgs e) {
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
            return scrollWrapper.GetViewCenter();
        }

        public void SetCursor(Cursor cursor) {
            canvas.Cursor = cursor;
        }

        public void SetPropertyBrowserObjects(object[] objects) {
            PropertyBrowserObjects = objects;
        }

        public void SetStatus(string status) {
            Status = status;
        }

        public void GenerateMouseMove() {
            if (currentTool != null) {
                DoMouseMove(new MouseEventArgs(MouseButtons, 0, prevMousePosition.X, prevMousePosition.Y, 0));
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
            UpdateAnimationState();
            UpdateViewOptions();
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
        
        private void UpdateAnimationState() {
            if (mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewAnimate).Checked) {
                level.tileAnimator.Run();
            } else {
                level.tileAnimator.Pause();
            }
        }

        protected override void DoSave(Project project) {
            level.levelAsset.WriteFile(project);
        }

        public override bool CanCopy => currentTool.CanCopy;
        public override bool CanPaste => currentTool.CanPaste;
        public override bool CanDelete => currentTool.CanDelete;
        public override bool HasSelection => currentTool.HasSelection;
        public override bool CanZoom => true;

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

        protected override void ZoomChanged() {
            scrollWrapper.Zoom = Zoom;
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
                Status = tool.Status;
                
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

        private void SpriteObjectBrowserContents_DoubleClicked(object sender, EventArgs e) {
            currentTool.SpriteDoubleClicked();
        }

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            currentTool.PropertyBrowserPropertyChanged(e);
        }

        void scrollWrapper_Scrolled(object sender, EventArgs e) {
            GenerateMouseMove();
            Repaint();
        }

        private void Level_GraphicsChanged(object sender, EventArgs e) {
            if (DockVisible) {
                Repaint();
            }
        }

        public void Repaint() {
            canvas.Invalidate();
        }

        private bool showGrid;
        private bool solidTilesOnly;
        private bool showTilePriority;
        private bool showRespawnAreas;
        private bool showScreenSizeGuide;

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewGrid).Checked;
            solidTilesOnly = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewSolidTilesOnly).Checked;
            showTilePriority = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTilePriority).Checked;
            showRespawnAreas = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewRespawnAreas).Checked;
            showScreenSizeGuide = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewScreenSizeGuide).Checked;

            tilesetObjectBrowserContents.SolidOnly = solidTilesOnly;
        }

        private void SaveAsImage(string filename) {
            using (Bitmap image = new Bitmap(level.Level.width * 64, level.Level.height * 64))
            using (Graphics g = Graphics.FromImage(image)) {
                RenderLevel(g);
                image.Save(filename);
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e) {
            if (level == null) {
                return;
            }
            scrollWrapper.TransformGraphics(e.Graphics);
            RenderLevel(e.Graphics);

            using (Pen selectionBorderDashPen = CreateSelectionBorderDashPen())
            using (Pen selectionBorderPen = CreateSelectionBorderPen()) {
                if (TileSelectionExists) {
                    e.Graphics.FillPath(selectionFillBrush, tileSelectionPath);
                    e.Graphics.DrawPath(selectionBorderPen, tileSelectionPath);
                    e.Graphics.DrawPath(selectionBorderDashPen, tileSelectionPath);
                }
                if (tileSelectionEraserRect != Rectangle.Empty) {
                    e.Graphics.FillRectangle(eraserFillBrush, tileSelectionEraserRect);
                    e.Graphics.DrawRectangle(selectionBorderPen, tileSelectionEraserRect);
                    e.Graphics.DrawRectangle(selectionBorderDashPen, tileSelectionEraserRect);
                }

                if (showGrid) {
                    for (int x = 1; x < level.Level.width; x++) {
                        e.Graphics.DrawLine(selectionBorderPen, x * 64, 0, x * 64, level.Level.height * 64);
                    }
                    for (int y = 1; y < level.Level.height; y++) {
                        e.Graphics.DrawLine(selectionBorderPen, 0, y * 64, level.Level.width * 64, y * 64);
                    }
                }
            }

            currentTool.Paint(e.Graphics);

            if (showScreenSizeGuide && screenSizeGuidePosition != Point.Empty) {
                int left = screenSizeGuidePosition.X - SNESGraphics.ScreenWidth / 2;
                int top = screenSizeGuidePosition.Y - SNESGraphics.ScreenHeight / 2;

                using (Pen pen = new Pen(Color.Snow, 1 / Zoom)) {
                    e.Graphics.DrawRectangle(pen, left, top, SNESGraphics.ScreenWidth, SNESGraphics.ScreenHeight);
                    e.Graphics.DrawLine(pen, left, screenSizeGuidePosition.Y, left + SNESGraphics.ScreenWidth, screenSizeGuidePosition.Y);
                    e.Graphics.DrawLine(pen, screenSizeGuidePosition.X, top, screenSizeGuidePosition.X, top + SNESGraphics.ScreenHeight);
                }
            }
        }

        private void RenderLevel(Graphics g) {
            if (solidTilesOnly) {
                RenderBackground(g, level.solidOnlyTiles);
            } else {
                RenderBackground(g, level.tiles);
            }

            List<WrappedLevelObject> objects = level.GetAllObjects().ToList();
            foreach (WrappedLevelObject obj in objects) {
                obj.Render(g);
            }

            if (showTilePriority && !solidTilesOnly) {
                RenderBackground(g, level.priorityTiles);
            }

            foreach (WrappedLevelObject obj in objects) {
                obj.RenderExtras(g, showRespawnAreas, Zoom);
            }
        }

        private void RenderBackground(Graphics g, Bitmap[] tiles) {
            for (int y = 0; y < level.Level.height; y++) {
                for (int x = 0; x < level.Level.width; x++) {
                    g.DrawImage(tiles[level.Level.background[x, y]], x * 64, y * 64);
                }
            }
        }

        public Pen CreateSelectionBorderPen() {
            return new Pen(Color.White, 1 / Zoom);
        }

        public Pen CreateSelectionBorderDashPen() {
            Pen dashPen = new Pen(Color.Black, 1 / Zoom);
            if (Zoom >= 1.0f) {
                dashPen.DashPattern = new float[] { 4 / Zoom, 4 / Zoom };
            } else {
                dashPen.DashPattern = new float[] { 4, 4 };
            }
            return dashPen;
        }

        // Double clicking on the title bar to maximize the window causes mouse up events to be sent without a mouse down. This is used to ignore those.
        private bool mouseDown = false;
        // Used to ignore mouse move events that are generated by setting the property grid objects
        private Point prevMousePosition = new Point(int.MinValue, int.MinValue);
        private Point screenSizeGuidePosition = Point.Empty;

        private void UpdateScreenSizeGuide(MouseEventArgs e) {
            Point transformed = scrollWrapper.TransformPoint(e.Location);
            int screenSizeGuideX = transformed.X;
            int screenSizeGuideY = transformed.Y;
            int maxX = level.Level.width * 64;
            int maxY = level.Level.height * 64;

            if (screenSizeGuideX < 0 || screenSizeGuideY < 0 || screenSizeGuideX >= maxX || screenSizeGuideY >= maxY) {
                screenSizeGuideX = 0;
                screenSizeGuideY = 0;
            } else {
                screenSizeGuideX = Math.Max(SNESGraphics.ScreenWidth / 2, Math.Min(maxX - SNESGraphics.ScreenWidth / 2, screenSizeGuideX));
                screenSizeGuideY = Math.Max(SNESGraphics.ScreenHeight / 2, Math.Min(maxY - SNESGraphics.ScreenHeight / 2 - 15, screenSizeGuideY));
            }
            if (screenSizeGuideX != screenSizeGuidePosition.X || screenSizeGuideY != screenSizeGuidePosition.Y) {
                screenSizeGuidePosition = new Point(screenSizeGuideX, screenSizeGuideY);
                if (showScreenSizeGuide) {
                    Repaint();
                }
            }
        }

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
            UpdateScreenSizeGuide(e);
            if (TransformMouseArgs(e, out LevelMouseEventArgs args)) {
                currentTool.MouseMove(args);
            }
        }

        private void canvas_MouseLeave(object sender, EventArgs e) {
            prevMousePosition = new Point(int.MinValue, int.MinValue);

            screenSizeGuidePosition = Point.Empty;
            if (showScreenSizeGuide) {
                Repaint();
            }
        }

        private bool TransformMouseArgs(MouseEventArgs e, out LevelMouseEventArgs ret, bool mouseDownOnly = false) {
            if ((mouseDown && e.Button == MouseButtons.Left) || (!mouseDown && !mouseDownOnly)) {
                Point transformed = scrollWrapper.TransformPoint(e.Location);
                ret = new LevelMouseEventArgs(e.Button, e.Clicks, transformed.X, transformed.Y, e.Delta, level.Level.width, level.Level.height, mouseDown);
                return true;
            }
            ret = null;
            return false;
        }

        private void canvas_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right) {
                e.IsInputKey = true;
            }
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
                ChangeTool(spriteTool);
            } else if (item == ToolStripGrouper.ItemType.LevelEditTitle) {
                if (titleEditor == null) {
                    titleEditor = new TitleEditor(this, project);
                    titleEditor.FormClosed += TitleEditor_FormClosed;
                    mainWindow.ShowEditor(titleEditor, assetInfo: null);
                } else {
                    titleEditor.Activate();
                }
            } else if (item == ToolStripGrouper.ItemType.LevelSettings) {
                try {
                    new LevelSettingsDialog(project, this).ShowDialog();
                } catch (Exception ex) {
                    MessageBox.Show($"Error opening level settings: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else if (item == ToolStripGrouper.ItemType.LevelClear) {
                undoManager.Do(new ClearLevelAction((ushort)Math.Max(tilesetObjectBrowserContents.SelectedTile, 0)));
            } else if (item == ToolStripGrouper.ItemType.LevelSaveAsImage) {
                if (saveAsImageDialog.ShowDialog() == DialogResult.OK) {
                    SaveAsImage(saveAsImageDialog.FileName);
                }
            } else if (item == ToolStripGrouper.ItemType.ViewNextFrame) {
                level.tileAnimator.Advance();
            } else if (item == ToolStripGrouper.ItemType.ViewRestartAnimation) {
                level.tileAnimator.Restart();
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
                { ToolStripGrouper.ItemType.SpritesPlayers, SpriteDisplay.Category.Player },
            };

        private readonly Dictionary<SpriteDisplay.Category, bool> spriteCategoryEnabled =
            new Dictionary<SpriteDisplay.Category, bool>() {
                { SpriteDisplay.Category.Item, true },
                { SpriteDisplay.Category.Victim, true },
                { SpriteDisplay.Category.OneShotMonster, true },
                { SpriteDisplay.Category.Monster, true },
                { SpriteDisplay.Category.LevelMonster, true },
                { SpriteDisplay.Category.Player, true },
            };

        private static HashSet<ToolStripGrouper.ItemType> viewOptionsItems = new HashSet<ToolStripGrouper.ItemType>() {
            ToolStripGrouper.ItemType.ViewGrid, ToolStripGrouper.ItemType.ViewSolidTilesOnly,
            ToolStripGrouper.ItemType.ViewTilePriority, ToolStripGrouper.ItemType.ViewRespawnAreas,
            ToolStripGrouper.ItemType.ViewScreenSizeGuide,
        };

        public override void ToolStripItemCheckedChanged(ToolStripGrouper.ItemType item) {
            if (DockPanel.ActiveDocument == this && spriteCategoryForMenuItem.TryGetValue(item, out SpriteDisplay.Category category)) {
                UpdateSpriteCategory(category, mainWindow.GetToolStripItem(item).Checked);
            } else if (item == ToolStripGrouper.ItemType.ViewAnimate) {
                UpdateAnimationState();
            } else if (viewOptionsItems.Contains(item)) {
                UpdateViewOptions();
                if (DockVisible) {
                    Repaint();
                }
            }
        }

        public bool ItemsEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Item];
        public bool VictimsEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Victim];
        public bool OneShotMonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.OneShotMonster];
        public bool MonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Monster];
        public bool BossMonstersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.LevelMonster];
        public bool PlayersEnabled => spriteCategoryEnabled[SpriteDisplay.Category.Player];

        private void UpdateSpriteCategory(SpriteDisplay.Category category, bool enabled) {
            if (spriteCategoryEnabled[category] != enabled) {
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
}
