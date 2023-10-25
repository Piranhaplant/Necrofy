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
using System.Threading.Tasks;

namespace Necrofy
{
    partial class LevelEditor : MapEditor<LevelEditorTool> {
        public readonly LoadedLevel level;
        private TitleEditor titleEditor = null;
        private LevelSettingsDialog levelSettings = null;

        public readonly TilesetObjectBrowserContents tilesetObjectBrowserContents;
        public readonly SpriteObjectBrowserContents spriteObjectBrowserContents;
        
        public new UndoManager<LevelEditor> undoManager { get; private set; }

        public static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 0, 0, 0));
        public static readonly SolidBrush eraserFillBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
        
        private SpriteTool spriteTool;
        private readonly Dictionary<LevelEditorTool.ObjectType, ObjectBrowserContents> toolTypeToObjectContents;

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.LevelEditor;

        public LevelEditor(LoadedLevel level) : base(64) {
            InitializeComponent();
            Disposed += LevelEditor_Disposed;

            this.level = level;
            UpdateTitle();
            
            tilesetObjectBrowserContents = new TilesetObjectBrowserContents(() => level.tileset, handler => level.TilesChanged += handler);
            tilesetObjectBrowserContents.SelectedIndexChanged += TilesetObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents = new SpriteObjectBrowserContents(level);
            spriteObjectBrowserContents.SelectedIndexChanged += SpriteObjectBrowserContents_SelectedIndexChanged;
            spriteObjectBrowserContents.DoubleClicked += SpriteObjectBrowserContents_DoubleClicked;
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Item);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.LevelMonster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Monster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.OneShotMonster);
            spriteObjectBrowserContents.AddCategory(SpriteDisplay.Category.Victim);

            toolTypeToObjectContents = new Dictionary<LevelEditorTool.ObjectType, ObjectBrowserContents> {
                { LevelEditorTool.ObjectType.Sprites, spriteObjectBrowserContents },
                { LevelEditorTool.ObjectType.Tiles, tilesetObjectBrowserContents },
            };
            
            level.TilesChanged += Level_GraphicsChanged;
            level.SpritesChanged += Level_GraphicsChanged;
        }

        protected override void CloseChildren(FormClosingEventArgs e) {
            CloseChild(titleEditor, e);
            CloseChild(levelSettings, e);
        }

        private static void CloseChild(EditorWindow child, FormClosingEventArgs e) {
            if (!e.Cancel && child != null) {
                child.Close();
                if (child.Visible) {
                    e.Cancel = true;
                }
            }
        }

        private void LevelEditor_Disposed(object sender, EventArgs e) {
            level.Dispose();
        }

        protected override UndoManager Setup() {
            SetupMapEditor(canvas, hscroll, vscroll);
            MapPadding = 64;
            ResizeMap(level.Level.width, level.Level.height);

            SetupTool(new LevelEditorBrushTool(this), ToolStripGrouper.ItemType.PaintbrushTool, Keys.P);
            SetupTool(new TileSuggestionTool(this), ToolStripGrouper.ItemType.TileSuggestTool, Keys.S);
            SetupTool(new LevelEditorRectangleSelectTool(this), ToolStripGrouper.ItemType.RectangleSelectTool, Keys.R);
            SetupTool(new LevelEditorPencilSelectTool(this), ToolStripGrouper.ItemType.PencilSelectTool, Keys.C);
            SetupTool(new LevelEditorTileSelectTool(this), ToolStripGrouper.ItemType.TileSelectTool, Keys.T);
            SetupTool(new ResizeLevelTool(this), ToolStripGrouper.ItemType.ResizeLevelTool, Keys.L);
            spriteTool = new SpriteTool(this);
            SetupTool(spriteTool, ToolStripGrouper.ItemType.SpriteTool, Keys.I);

            undoManager = new UndoManager<LevelEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public void UpdateTitle() {
            Title = level.LevelNumber.ToString() + " " + level.Level.displayName;
        }

        public void FillSelection() {
            if (tilesetObjectBrowserContents.SelectedIndex >= 0 && SelectionExists) {
                undoManager.Do(new FillSelectionAction((ushort)tilesetObjectBrowserContents.SelectedIndex));
            }
        }

        public void SetPropertyBrowserObjects(object[] objects) {
            PropertyBrowserObjects = objects;
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
        }

        public override void Hidden() {
            base.Hidden();
            if (!DockVisible) {
                level.tileset.tileAnimator.Pause();
            }
        }

        private void UpdateAnimationState() {
            if (mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewAnimate).Checked) {
                level.tileset.tileAnimator.Run();
            } else {
                level.tileset.tileAnimator.Pause();
            }
        }

        protected override void DoSave(Project project) {
            level.Save(project);
        }

        public override void RenameAssetReferences(Asset.RenameResults results) {
            LevelAsset.RenameReferences(level.Level, results);
        }

        public override int? LevelNumber => level.LevelNumber;
        
        public void NonTileSelectionChanged() {
            RaiseSelectionChanged();
        }
        
        private void TilesetObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            CurrentTool.TileChanged();
        }

        private void SpriteObjectBrowserContents_SelectedIndexChanged(object sender, EventArgs e) {
            CurrentTool.SpriteChanged();
        }

        private void SpriteObjectBrowserContents_DoubleClicked(object sender, EventArgs e) {
            CurrentTool.SpriteDoubleClicked();
        }

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            CurrentTool.PropertyBrowserPropertyChanged(e);
        }
        
        private void Level_GraphicsChanged(object sender, EventArgs e) {
            if (DockVisible) {
                Repaint();
            }
        }

        private bool showGrid;
        private bool solidTilesOnly;
        private bool showTilePriority;
        public bool showRespawnAreas { get; private set; }
        private bool showScreenSizeGuide;

        private void UpdateViewOptions() {
            showGrid = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewGrid).Checked;
            solidTilesOnly = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewSolidTilesOnly).Checked;
            showTilePriority = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewTilePriority).Checked;
            showRespawnAreas = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewRespawnAreas).Checked;
            showScreenSizeGuide = mainWindow.GetToolStripItem(ToolStripGrouper.ItemType.ViewScreenSizeGuide).Checked;

            tilesetObjectBrowserContents.SolidOnly = solidTilesOnly;
            GenerateMouseMove(); // Since sprite tool has UI elements that change based on this
        }

        private void SaveAsImage(string filename) {
            using (Bitmap image = new Bitmap(level.Level.width * 64, level.Level.height * 64))
            using (Graphics g = Graphics.FromImage(image)) {
                RenderLevel(g);
                image.Save(filename);
            }
        }

        protected override void PaintMap(Graphics g) {
            if (level == null) {
                return;
            }
            RenderLevel(g);
            if (showGrid) {
                DrawGrid(g, WhitePen, GetVisibleArea(), 64);
            }
        }

        protected override void PaintSelection(Graphics g, GraphicsPath path) {
            g.FillPath(selectionFillBrush, path);
            g.DrawPath(WhitePen, path);
            g.DrawPath(SelectionDashPen, path);
        }

        protected override void PaintSelectionEraser(Graphics g, Rectangle r) {
            g.FillRectangle(eraserFillBrush, r);
            g.DrawRectangle(WhitePen, r);
            g.DrawRectangle(SelectionDashPen, r);
        }

        protected override void PaintSelectionDrawRectangle(Graphics g, Rectangle r) {
            // Nothing to do
        }

        protected override void PaintExtras(Graphics g) {
            if (showScreenSizeGuide && screenSizeGuidePosition != Point.Empty) {
                int left = screenSizeGuidePosition.X - SNESGraphics.ScreenWidth / 2;
                int top = screenSizeGuidePosition.Y - SNESGraphics.ScreenHeight / 2;

                using (Pen pen = new Pen(Color.Snow, 1 / Zoom)) {
                    g.DrawRectangle(pen, left, top, SNESGraphics.ScreenWidth, SNESGraphics.ScreenHeight);
                    g.DrawLine(pen, left, screenSizeGuidePosition.Y, left + SNESGraphics.ScreenWidth, screenSizeGuidePosition.Y);
                    g.DrawLine(pen, screenSizeGuidePosition.X, top, screenSizeGuidePosition.X, top + SNESGraphics.ScreenHeight);
                }
            }
        }
        
        private void RenderLevel(Graphics g) {
            if (solidTilesOnly) {
                RenderBackground(g, level.tileset.solidOnlyTiles);
            } else {
                RenderBackground(g, level.tileset.tiles);
            }

            List<WrappedLevelObject> objects = level.GetAllObjects().ToList();
            foreach (WrappedLevelObject obj in objects) {
                obj.Render(g);
            }

            if (showTilePriority && !solidTilesOnly) {
                RenderBackground(g, level.tileset.priorityTiles);
            }

            foreach (WrappedLevelObject obj in objects) {
                obj.RenderExtras(g, showRespawnAreas, Zoom);
            }
        }

        private void RenderBackground(Graphics g, Bitmap[] tiles) {
            for (int y = 0; y < level.Level.height; y++) {
                for (int x = 0; x < level.Level.width; x++) {
                    ushort tile = level.Level.background[x, y];
                    if (tile < tiles.Length) {
                        g.DrawImage(tiles[tile], x * 64, y * 64);
                    } else {
                        g.FillRectangle(Brushes.Red, x * 64, y * 64, 64, 64);
                    }
                }
            }
        }

        private static readonly Font underTextFont = SystemFonts.DefaultFont;
        private static readonly StringFormat underTextFormat = new StringFormat() {
            Alignment = StringAlignment.Center
        };

        public static void DrawTextUnder(Graphics g, Rectangle r, string s, float zoom) {
            int x = r.X + r.Width / 2;
            int y = r.Bottom + 4;
            float pixelSize = 1 / zoom;
            g.DrawString(s, underTextFont, Brushes.Black, x + pixelSize, y + pixelSize, underTextFormat);
            g.DrawString(s, underTextFont, Brushes.Black, x + pixelSize, y - pixelSize, underTextFormat);
            g.DrawString(s, underTextFont, Brushes.Black, x - pixelSize, y - pixelSize, underTextFormat);
            g.DrawString(s, underTextFont, Brushes.Black, x - pixelSize, y + pixelSize, underTextFormat);
            g.DrawString(s, underTextFont, Brushes.White, x, y, underTextFormat);
        }

        private Point screenSizeGuidePosition = Point.Empty;

        private void UpdateScreenSizeGuide(MouseEventArgs e) {
            Point transformed = ScrollWrapper.TransformPoint(e.Location);
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

        private void canvas_MouseMove(object sender, MouseEventArgs e) {
            UpdateScreenSizeGuide(e);
        }

        private void canvas_MouseLeave(object sender, EventArgs e) {
            screenSizeGuidePosition = Point.Empty;
            if (showScreenSizeGuide) {
                Repaint();
            }
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            if (item == ToolStripGrouper.ItemType.SpritesAll) {
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
                    if (levelSettings == null) {
                        levelSettings = new LevelSettingsDialog(this);
                        levelSettings.FormClosed += LevelSettings_FormClosed;
                        mainWindow.ShowEditor(levelSettings, assetInfo: null);
                    } else {
                        levelSettings.Activate();
                    }
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
                level.tileset.tileAnimator.Advance();
            } else if (item == ToolStripGrouper.ItemType.ViewRestartAnimation) {
                level.tileset.tileAnimator.Restart();
            }
        }

        private void TitleEditor_FormClosed(object sender, FormClosedEventArgs e) {
            titleEditor = null;
        }

        private void LevelSettings_FormClosed(object sender, FormClosedEventArgs e) {
            levelSettings = null;
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
            base.ToolStripItemCheckedChanged(item);
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

        protected override void ToolChanged(LevelEditorTool currentTool) {
            BrowserContents = toolTypeToObjectContents[currentTool.objectType];
            PropertyBrowserObjects = currentTool.PropertyBrowserObjects;
        }
    }
}
