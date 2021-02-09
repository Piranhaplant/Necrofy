namespace Necrofy
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2012LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2012LightTheme();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.createProjectButton = new System.Windows.Forms.ToolStripButton();
            this.openProjectButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.saveAllButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.undoButton = new System.Windows.Forms.ToolStripSplitButton();
            this.redoButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buildProjectButton = new System.Windows.Forms.ToolStripButton();
            this.runProjectButton = new System.Windows.Forms.ToolStripButton();
            this.runFromLevelButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.zoomLevelLabel = new System.Windows.Forms.ToolStripLabel();
            this.zoomInButton = new System.Windows.Forms.ToolStripButton();
            this.levelToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.paintbrushButton = new System.Windows.Forms.ToolStripButton();
            this.tileSuggestButton = new System.Windows.Forms.ToolStripButton();
            this.rectangleSelectButton = new System.Windows.Forms.ToolStripButton();
            this.pencilSelectButton = new System.Windows.Forms.ToolStripButton();
            this.tileSelectButton = new System.Windows.Forms.ToolStripButton();
            this.resizeLevelButton = new System.Windows.Forms.ToolStripButton();
            this.spritesButton = new Necrofy.CheckableToolStripSplitButton();
            this.spritesItems = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesVictims = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesOneShotMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesBossMonsters = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesPlayers = new Necrofy.SeparateCheckToolStripMenuItem();
            this.spritesSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.spritesAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.flipHorizontalButton = new System.Windows.Forms.ToolStripButton();
            this.flipVerticalButton = new System.Windows.Forms.ToolStripButton();
            this.levelTitleToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.centerHorizontallyButton = new System.Windows.Forms.ToolStripButton();
            this.centerVerticallyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.moveToFrontButton = new System.Windows.Forms.ToolStripButton();
            this.moveToBackButton = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.fileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjects = new Necrofy.RecentFilesMenu();
            this.fileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.editRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.editSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editCut = new System.Windows.Forms.ToolStripMenuItem();
            this.editCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.editPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.editDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.editSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.editSelectNone = new System.Windows.Forms.ToolStripMenuItem();
            this.buildMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.buildBuildProject = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunProject = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunFromLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.buildRunSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSolidTilesOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTilePriority = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewRespawnAreas = new System.Windows.Forms.ToolStripMenuItem();
            this.viewScreenSizeGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.viewAnimate = new System.Windows.Forms.ToolStripMenuItem();
            this.viewNextFrame = new System.Windows.Forms.ToolStripMenuItem();
            this.viewRestartAnimation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.viewAxes = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTileBorders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.viewZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.viewZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.levelMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.levelEditTitle = new System.Windows.Forms.ToolStripMenuItem();
            this.levelSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.levelClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.levelSaveAsImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsPaintbrush = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsTileSuggest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsRectangleSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsPencilSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsTileSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsResizeLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSprites = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeFlipHorizontal = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeFlipVertical = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.arrangeCenterHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeCenterVertically = new System.Windows.Forms.ToolStripMenuItem();
            this.titleSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.arrangeMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeMoveToFront = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeMoveToBack = new System.Windows.Forms.ToolStripMenuItem();
            this.windowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.windowProject = new System.Windows.Forms.ToolStripMenuItem();
            this.windowObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.windowProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.windowSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.windowRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.buildStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripGrouper = new Necrofy.ToolStripGrouper(this.components);
            this.toolBarMenuLinker = new Necrofy.ToolBarMenuLinker(this.components);
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.dockPanel.DockBottomPortion = 200D;
            this.dockPanel.DockLeftPortion = 220D;
            this.dockPanel.DockRightPortion = 200D;
            this.dockPanel.DockTopPortion = 200D;
            this.dockPanel.Location = new System.Drawing.Point(0, 49);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(1061, 579);
            this.dockPanel.TabIndex = 2;
            this.dockPanel.Theme = this.vS2012LightTheme1;
            this.dockPanel.ContentRemoved += new System.EventHandler<WeifenLuo.WinFormsUI.Docking.DockContentEventArgs>(this.dockPanel_ContentRemoved);
            this.dockPanel.ActiveDocumentChanged += new System.EventHandler(this.dockPanel_ActiveDocumentChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createProjectButton,
            this.openProjectButton,
            this.saveButton,
            this.saveAllButton,
            this.toolStripSeparator1,
            this.cutButton,
            this.copyButton,
            this.pasteButton,
            this.toolStripSeparator2,
            this.undoButton,
            this.redoButton,
            this.toolStripSeparator3,
            this.buildProjectButton,
            this.runProjectButton,
            this.runFromLevelButton,
            this.toolStripSeparator9,
            this.zoomOutButton,
            this.zoomLevelLabel,
            this.zoomInButton,
            this.levelToolStripSeparator,
            this.paintbrushButton,
            this.tileSuggestButton,
            this.rectangleSelectButton,
            this.pencilSelectButton,
            this.tileSelectButton,
            this.resizeLevelButton,
            this.spritesButton,
            this.toolStripSeparator11,
            this.flipHorizontalButton,
            this.flipVerticalButton,
            this.levelTitleToolStripSeparator,
            this.centerHorizontallyButton,
            this.centerVerticallyButton,
            this.toolStripSeparator4,
            this.moveUpButton,
            this.moveDownButton,
            this.moveToFrontButton,
            this.moveToBackButton});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1061, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // createProjectButton
            // 
            this.createProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.createProjectButton.Image = global::Necrofy.Properties.Resources.document__pencil;
            this.createProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createProjectButton.Name = "createProjectButton";
            this.createProjectButton.Size = new System.Drawing.Size(23, 22);
            this.createProjectButton.Text = "Create project";
            this.createProjectButton.Click += new System.EventHandler(this.CreateProject);
            // 
            // openProjectButton
            // 
            this.openProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openProjectButton.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.openProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openProjectButton.Name = "openProjectButton";
            this.openProjectButton.Size = new System.Drawing.Size(23, 22);
            this.openProjectButton.Text = "Open Project";
            this.openProjectButton.Click += new System.EventHandler(this.OpenProject);
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Enabled = false;
            this.saveButton.Image = global::Necrofy.Properties.Resources.disk;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(23, 22);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.Save);
            // 
            // saveAllButton
            // 
            this.saveAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveAllButton.Enabled = false;
            this.saveAllButton.Image = global::Necrofy.Properties.Resources.disks;
            this.saveAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllButton.Name = "saveAllButton";
            this.saveAllButton.Size = new System.Drawing.Size(23, 22);
            this.saveAllButton.Text = "Save All";
            this.saveAllButton.Click += new System.EventHandler(this.SaveAll);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // cutButton
            // 
            this.cutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutButton.Enabled = false;
            this.cutButton.Image = global::Necrofy.Properties.Resources.scissors;
            this.cutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(23, 22);
            this.cutButton.Text = "Cut";
            this.cutButton.Click += new System.EventHandler(this.Cut);
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Enabled = false;
            this.copyButton.Image = global::Necrofy.Properties.Resources.document_copy;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(23, 22);
            this.copyButton.Text = "Copy";
            this.copyButton.Click += new System.EventHandler(this.Copy);
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Enabled = false;
            this.pasteButton.Image = global::Necrofy.Properties.Resources.clipboard_paste;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(23, 22);
            this.pasteButton.Text = "Paste";
            this.pasteButton.Click += new System.EventHandler(this.Paste);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // undoButton
            // 
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoButton.Enabled = false;
            this.undoButton.Image = global::Necrofy.Properties.Resources.arrow_return_180;
            this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(32, 22);
            this.undoButton.Text = "Undo";
            this.undoButton.ButtonClick += new System.EventHandler(this.Undo);
            // 
            // redoButton
            // 
            this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoButton.Enabled = false;
            this.redoButton.Image = global::Necrofy.Properties.Resources.arrow_return;
            this.redoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(32, 22);
            this.redoButton.Text = "Redo";
            this.redoButton.ButtonClick += new System.EventHandler(this.Redo);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // buildProjectButton
            // 
            this.buildProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buildProjectButton.Enabled = false;
            this.buildProjectButton.Image = global::Necrofy.Properties.Resources.compile;
            this.buildProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buildProjectButton.Name = "buildProjectButton";
            this.buildProjectButton.Size = new System.Drawing.Size(23, 22);
            this.buildProjectButton.Text = "Build project";
            this.buildProjectButton.Click += new System.EventHandler(this.BuildProject);
            // 
            // runProjectButton
            // 
            this.runProjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runProjectButton.Enabled = false;
            this.runProjectButton.Image = global::Necrofy.Properties.Resources.control;
            this.runProjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runProjectButton.Name = "runProjectButton";
            this.runProjectButton.Size = new System.Drawing.Size(23, 22);
            this.runProjectButton.Text = "Run Project";
            this.runProjectButton.Click += new System.EventHandler(this.RunProject);
            // 
            // runFromLevelButton
            // 
            this.runFromLevelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runFromLevelButton.Enabled = false;
            this.runFromLevelButton.Image = global::Necrofy.Properties.Resources.control_cursor;
            this.runFromLevelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runFromLevelButton.Name = "runFromLevelButton";
            this.runFromLevelButton.Size = new System.Drawing.Size(23, 22);
            this.runFromLevelButton.Text = "Run From Level";
            this.runFromLevelButton.Click += new System.EventHandler(this.RunFromLevel);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutButton.Enabled = false;
            this.zoomOutButton.Image = global::Necrofy.Properties.Resources.magnifier_zoom_out;
            this.zoomOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(23, 22);
            this.zoomOutButton.Text = "Zoom Out";
            this.zoomOutButton.Click += new System.EventHandler(this.ZoomOut);
            // 
            // zoomLevelLabel
            // 
            this.zoomLevelLabel.AutoSize = false;
            this.zoomLevelLabel.Enabled = false;
            this.zoomLevelLabel.Name = "zoomLevelLabel";
            this.zoomLevelLabel.Size = new System.Drawing.Size(41, 22);
            this.zoomLevelLabel.Text = "100%";
            // 
            // zoomInButton
            // 
            this.zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInButton.Enabled = false;
            this.zoomInButton.Image = global::Necrofy.Properties.Resources.magnifier_zoom_in;
            this.zoomInButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(23, 22);
            this.zoomInButton.Text = "Zoom In";
            this.zoomInButton.Click += new System.EventHandler(this.ZoomIn);
            // 
            // levelToolStripSeparator
            // 
            this.toolStripGrouper.SetItemSet(this.levelToolStripSeparator, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.levelToolStripSeparator.Name = "levelToolStripSeparator";
            this.levelToolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // paintbrushButton
            // 
            this.paintbrushButton.Checked = true;
            this.paintbrushButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.paintbrushButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.paintbrushButton.Image = global::Necrofy.Properties.Resources.paint_brush;
            this.paintbrushButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.paintbrushButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.paintbrushButton, Necrofy.ToolStripGrouper.ItemType.PaintbrushTool);
            this.paintbrushButton.Name = "paintbrushButton";
            this.paintbrushButton.Size = new System.Drawing.Size(23, 22);
            this.paintbrushButton.Text = "Paintbrush";
            // 
            // tileSuggestButton
            // 
            this.tileSuggestButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tileSuggestButton.Image = global::Necrofy.Properties.Resources.light_bulb;
            this.tileSuggestButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.tileSuggestButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.tileSuggestButton, Necrofy.ToolStripGrouper.ItemType.TileSuggestTool);
            this.tileSuggestButton.Name = "tileSuggestButton";
            this.tileSuggestButton.Size = new System.Drawing.Size(23, 22);
            this.tileSuggestButton.Text = "Tile Suggest";
            // 
            // rectangleSelectButton
            // 
            this.rectangleSelectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rectangleSelectButton.Image = global::Necrofy.Properties.Resources.selection_select;
            this.rectangleSelectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.rectangleSelectButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.rectangleSelectButton, Necrofy.ToolStripGrouper.ItemType.RectangleSelectTool);
            this.rectangleSelectButton.Name = "rectangleSelectButton";
            this.rectangleSelectButton.Size = new System.Drawing.Size(23, 22);
            this.rectangleSelectButton.Text = "Rectangle Select";
            // 
            // pencilSelectButton
            // 
            this.pencilSelectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pencilSelectButton.Image = global::Necrofy.Properties.Resources.pencil_select;
            this.pencilSelectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.pencilSelectButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.pencilSelectButton, Necrofy.ToolStripGrouper.ItemType.PencilSelectTool);
            this.pencilSelectButton.Name = "pencilSelectButton";
            this.pencilSelectButton.Size = new System.Drawing.Size(23, 22);
            this.pencilSelectButton.Text = "Pencil Select";
            // 
            // tileSelectButton
            // 
            this.tileSelectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tileSelectButton.Image = global::Necrofy.Properties.Resources.tile_select;
            this.tileSelectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.tileSelectButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.tileSelectButton, Necrofy.ToolStripGrouper.ItemType.TileSelectTool);
            this.tileSelectButton.Name = "tileSelectButton";
            this.tileSelectButton.Size = new System.Drawing.Size(23, 22);
            this.tileSelectButton.Text = "Tile Select";
            // 
            // resizeLevelButton
            // 
            this.resizeLevelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resizeLevelButton.Image = global::Necrofy.Properties.Resources.map_resize;
            this.resizeLevelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.resizeLevelButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.resizeLevelButton, Necrofy.ToolStripGrouper.ItemType.ResizeLevelTool);
            this.resizeLevelButton.Name = "resizeLevelButton";
            this.resizeLevelButton.Size = new System.Drawing.Size(23, 22);
            this.resizeLevelButton.Text = "Resize Level";
            // 
            // spritesButton
            // 
            this.spritesButton.Checked = false;
            this.spritesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.spritesButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spritesItems,
            this.spritesVictims,
            this.spritesOneShotMonsters,
            this.spritesMonsters,
            this.spritesBossMonsters,
            this.spritesPlayers,
            this.spritesSeparator,
            this.spritesAll});
            this.spritesButton.Image = global::Necrofy.Properties.Resources.leaf;
            this.spritesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.spritesButton, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.spritesButton, Necrofy.ToolStripGrouper.ItemType.SpriteTool);
            this.spritesButton.Name = "spritesButton";
            this.spritesButton.Size = new System.Drawing.Size(32, 22);
            this.spritesButton.Text = "Sprites";
            // 
            // spritesItems
            // 
            this.spritesItems.Checked = true;
            this.spritesItems.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesItems.Image = global::Necrofy.Properties.Resources.item;
            this.toolStripGrouper.SetItemType(this.spritesItems, Necrofy.ToolStripGrouper.ItemType.SpritesItems);
            this.spritesItems.Name = "spritesItems";
            this.spritesItems.Size = new System.Drawing.Size(184, 22);
            this.spritesItems.Text = "Items";
            // 
            // spritesVictims
            // 
            this.spritesVictims.Checked = true;
            this.spritesVictims.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesVictims.Image = global::Necrofy.Properties.Resources.victim;
            this.toolStripGrouper.SetItemType(this.spritesVictims, Necrofy.ToolStripGrouper.ItemType.SpritesVictims);
            this.spritesVictims.Name = "spritesVictims";
            this.spritesVictims.Size = new System.Drawing.Size(184, 22);
            this.spritesVictims.Text = "Victims";
            // 
            // spritesOneShotMonsters
            // 
            this.spritesOneShotMonsters.Checked = true;
            this.spritesOneShotMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesOneShotMonsters.Image = global::Necrofy.Properties.Resources.one_shot_monster;
            this.toolStripGrouper.SetItemType(this.spritesOneShotMonsters, Necrofy.ToolStripGrouper.ItemType.SpritesOneShotMonsters);
            this.spritesOneShotMonsters.Name = "spritesOneShotMonsters";
            this.spritesOneShotMonsters.Size = new System.Drawing.Size(184, 22);
            this.spritesOneShotMonsters.Text = "One-shot Monsters";
            // 
            // spritesMonsters
            // 
            this.spritesMonsters.Checked = true;
            this.spritesMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesMonsters.Image = global::Necrofy.Properties.Resources.monster;
            this.toolStripGrouper.SetItemType(this.spritesMonsters, Necrofy.ToolStripGrouper.ItemType.SpritesMonsters);
            this.spritesMonsters.Name = "spritesMonsters";
            this.spritesMonsters.Size = new System.Drawing.Size(184, 22);
            this.spritesMonsters.Text = "Monsters";
            // 
            // spritesBossMonsters
            // 
            this.spritesBossMonsters.Checked = true;
            this.spritesBossMonsters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.spritesBossMonsters.Image = global::Necrofy.Properties.Resources.boss_monster;
            this.toolStripGrouper.SetItemType(this.spritesBossMonsters, Necrofy.ToolStripGrouper.ItemType.SpritesBossMonsters);
            this.spritesBossMonsters.Name = "spritesBossMonsters";
            this.spritesBossMonsters.Size = new System.Drawing.Size(184, 22);
            this.spritesBossMonsters.Text = "Boss Monsters";
            // 
            // spritesPlayers
            // 
            this.spritesPlayers.Checked = true;
            this.spritesPlayers.Image = global::Necrofy.Properties.Resources.zeke;
            this.toolStripGrouper.SetItemType(this.spritesPlayers, Necrofy.ToolStripGrouper.ItemType.SpritesPlayers);
            this.spritesPlayers.Name = "spritesPlayers";
            this.spritesPlayers.Size = new System.Drawing.Size(184, 22);
            this.spritesPlayers.Text = "Player Start Positions";
            // 
            // spritesSeparator
            // 
            this.spritesSeparator.Name = "spritesSeparator";
            this.spritesSeparator.Size = new System.Drawing.Size(181, 6);
            // 
            // spritesAll
            // 
            this.toolStripGrouper.SetItemType(this.spritesAll, Necrofy.ToolStripGrouper.ItemType.SpritesAll);
            this.spritesAll.Name = "spritesAll";
            this.spritesAll.Size = new System.Drawing.Size(184, 22);
            this.spritesAll.Text = "All";
            // 
            // toolStripSeparator11
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator11, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // flipHorizontalButton
            // 
            this.flipHorizontalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipHorizontalButton.Image = global::Necrofy.Properties.Resources.layer_flip;
            this.flipHorizontalButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.flipHorizontalButton, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.flipHorizontalButton, Necrofy.ToolStripGrouper.ItemType.FlipHorizontally);
            this.flipHorizontalButton.Name = "flipHorizontalButton";
            this.flipHorizontalButton.Size = new System.Drawing.Size(23, 22);
            this.flipHorizontalButton.Text = "Flip Horizontally";
            // 
            // flipVerticalButton
            // 
            this.flipVerticalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.flipVerticalButton.Image = global::Necrofy.Properties.Resources.layer_flip_vertical;
            this.flipVerticalButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.flipVerticalButton, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.flipVerticalButton, Necrofy.ToolStripGrouper.ItemType.FlipVertically);
            this.flipVerticalButton.Name = "flipVerticalButton";
            this.flipVerticalButton.Size = new System.Drawing.Size(23, 22);
            this.flipVerticalButton.Text = "Flip Vertical";
            // 
            // levelTitleToolStripSeparator
            // 
            this.toolStripGrouper.SetItemSet(this.levelTitleToolStripSeparator, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.levelTitleToolStripSeparator.Name = "levelTitleToolStripSeparator";
            this.levelTitleToolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // centerHorizontallyButton
            // 
            this.centerHorizontallyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.centerHorizontallyButton.Image = global::Necrofy.Properties.Resources.layers_alignment_center;
            this.centerHorizontallyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.centerHorizontallyButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.centerHorizontallyButton, Necrofy.ToolStripGrouper.ItemType.CenterHorizontally);
            this.centerHorizontallyButton.Name = "centerHorizontallyButton";
            this.centerHorizontallyButton.Size = new System.Drawing.Size(23, 22);
            this.centerHorizontallyButton.Text = "Center Horizontally";
            // 
            // centerVerticallyButton
            // 
            this.centerVerticallyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.centerVerticallyButton.Image = global::Necrofy.Properties.Resources.layers_alignment_middle;
            this.centerVerticallyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.centerVerticallyButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.centerVerticallyButton, Necrofy.ToolStripGrouper.ItemType.CenterVertically);
            this.centerVerticallyButton.Name = "centerVerticallyButton";
            this.centerVerticallyButton.Size = new System.Drawing.Size(23, 22);
            this.centerVerticallyButton.Text = "Center Vertically";
            // 
            // toolStripSeparator4
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator4, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // moveUpButton
            // 
            this.moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpButton.Image = global::Necrofy.Properties.Resources.arrow_090;
            this.moveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.moveUpButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.moveUpButton, Necrofy.ToolStripGrouper.ItemType.MoveUp);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(23, 22);
            this.moveUpButton.Text = "Move Up";
            // 
            // moveDownButton
            // 
            this.moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownButton.Image = global::Necrofy.Properties.Resources.arrow_270;
            this.moveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.moveDownButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.moveDownButton, Necrofy.ToolStripGrouper.ItemType.MoveDown);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(23, 22);
            this.moveDownButton.Text = "Move Down";
            // 
            // moveToFrontButton
            // 
            this.moveToFrontButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveToFrontButton.Image = global::Necrofy.Properties.Resources.arrow_stop_090;
            this.moveToFrontButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.moveToFrontButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.moveToFrontButton, Necrofy.ToolStripGrouper.ItemType.MoveToFront);
            this.moveToFrontButton.Name = "moveToFrontButton";
            this.moveToFrontButton.Size = new System.Drawing.Size(23, 22);
            this.moveToFrontButton.Text = "Move to Front";
            // 
            // moveToBackButton
            // 
            this.moveToBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveToBackButton.Image = global::Necrofy.Properties.Resources.arrow_stop_270;
            this.moveToBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripGrouper.SetItemSet(this.moveToBackButton, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.moveToBackButton, Necrofy.ToolStripGrouper.ItemType.MoveToBack);
            this.moveToBackButton.Name = "moveToBackButton";
            this.moveToBackButton.Size = new System.Drawing.Size(23, 22);
            this.moveToBackButton.Text = "Move to Back";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.buildMenu,
            this.viewMenu,
            this.levelMenu,
            this.toolsMenu,
            this.arrangeMenu,
            this.windowMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1061, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNewProject,
            this.fileOpenProject,
            this.fileSeparator1,
            this.fileSave,
            this.fileSaveAll,
            this.fileClose,
            this.fileSeparator2,
            this.recentProjects,
            this.fileSeparator3,
            this.fileExit});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "&File";
            // 
            // fileNewProject
            // 
            this.fileNewProject.Image = global::Necrofy.Properties.Resources.document__pencil;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.fileNewProject, this.createProjectButton);
            this.fileNewProject.Name = "fileNewProject";
            this.fileNewProject.Size = new System.Drawing.Size(195, 22);
            this.fileNewProject.Text = "&New Project...";
            this.fileNewProject.Click += new System.EventHandler(this.CreateProject);
            // 
            // fileOpenProject
            // 
            this.fileOpenProject.Image = global::Necrofy.Properties.Resources.folder_horizontal_open;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.fileOpenProject, this.openProjectButton);
            this.fileOpenProject.Name = "fileOpenProject";
            this.fileOpenProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.fileOpenProject.Size = new System.Drawing.Size(195, 22);
            this.fileOpenProject.Text = "&Open Project...";
            this.fileOpenProject.Click += new System.EventHandler(this.OpenProject);
            // 
            // fileSeparator1
            // 
            this.fileSeparator1.Name = "fileSeparator1";
            this.fileSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // fileSave
            // 
            this.fileSave.Enabled = false;
            this.fileSave.Image = global::Necrofy.Properties.Resources.disk;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.fileSave, this.saveButton);
            this.fileSave.Name = "fileSave";
            this.fileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileSave.Size = new System.Drawing.Size(195, 22);
            this.fileSave.Text = "&Save";
            this.fileSave.Click += new System.EventHandler(this.Save);
            // 
            // fileSaveAll
            // 
            this.fileSaveAll.Enabled = false;
            this.fileSaveAll.Image = global::Necrofy.Properties.Resources.disks;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.fileSaveAll, this.saveAllButton);
            this.fileSaveAll.Name = "fileSaveAll";
            this.fileSaveAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.fileSaveAll.Size = new System.Drawing.Size(195, 22);
            this.fileSaveAll.Text = "Save &All";
            this.fileSaveAll.Click += new System.EventHandler(this.SaveAll);
            // 
            // fileClose
            // 
            this.fileClose.Enabled = false;
            this.fileClose.Name = "fileClose";
            this.fileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.fileClose.Size = new System.Drawing.Size(195, 22);
            this.fileClose.Text = "&Close";
            this.fileClose.Click += new System.EventHandler(this.CloseFile);
            // 
            // fileSeparator2
            // 
            this.fileSeparator2.Name = "fileSeparator2";
            this.fileSeparator2.Size = new System.Drawing.Size(192, 6);
            // 
            // recentProjects
            // 
            this.recentProjects.Files = ((System.Collections.Generic.IEnumerable<string>)(resources.GetObject("recentProjects.Files")));
            this.recentProjects.MaxItems = 10;
            this.recentProjects.MaxLength = 60;
            this.recentProjects.Name = "recentProjects";
            this.recentProjects.Separator = this.fileSeparator3;
            this.recentProjects.Size = new System.Drawing.Size(195, 22);
            this.recentProjects.Text = "Recent Projects";
            this.recentProjects.FileClicked += new Necrofy.RecentFilesMenu.FileClickedDelegate(this.recentProjects_FileClicked);
            // 
            // fileSeparator3
            // 
            this.fileSeparator3.Name = "fileSeparator3";
            this.fileSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // fileExit
            // 
            this.fileExit.Name = "fileExit";
            this.fileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.fileExit.Size = new System.Drawing.Size(195, 22);
            this.fileExit.Text = "E&xit";
            this.fileExit.Click += new System.EventHandler(this.Exit);
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUndo,
            this.editRedo,
            this.editSeparator1,
            this.editCut,
            this.editCopy,
            this.editPaste,
            this.editDelete,
            this.editSeparator2,
            this.editSelectAll,
            this.editSelectNone});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(39, 20);
            this.editMenu.Text = "&Edit";
            // 
            // editUndo
            // 
            this.editUndo.Enabled = false;
            this.editUndo.Image = global::Necrofy.Properties.Resources.arrow_return_180;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.editUndo, this.undoButton);
            this.editUndo.Name = "editUndo";
            this.editUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.editUndo.Size = new System.Drawing.Size(211, 22);
            this.editUndo.Text = "&Undo";
            this.editUndo.Click += new System.EventHandler(this.Undo);
            // 
            // editRedo
            // 
            this.editRedo.Enabled = false;
            this.editRedo.Image = global::Necrofy.Properties.Resources.arrow_return;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.editRedo, this.redoButton);
            this.editRedo.Name = "editRedo";
            this.editRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.editRedo.Size = new System.Drawing.Size(211, 22);
            this.editRedo.Text = "&Redo";
            this.editRedo.Click += new System.EventHandler(this.Redo);
            // 
            // editSeparator1
            // 
            this.editSeparator1.Name = "editSeparator1";
            this.editSeparator1.Size = new System.Drawing.Size(208, 6);
            // 
            // editCut
            // 
            this.editCut.Enabled = false;
            this.editCut.Image = global::Necrofy.Properties.Resources.scissors;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.editCut, this.cutButton);
            this.editCut.Name = "editCut";
            this.editCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.editCut.Size = new System.Drawing.Size(211, 22);
            this.editCut.Text = "Cu&t";
            this.editCut.Click += new System.EventHandler(this.Cut);
            // 
            // editCopy
            // 
            this.editCopy.Enabled = false;
            this.editCopy.Image = global::Necrofy.Properties.Resources.document_copy;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.editCopy, this.copyButton);
            this.editCopy.Name = "editCopy";
            this.editCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.editCopy.Size = new System.Drawing.Size(211, 22);
            this.editCopy.Text = "&Copy";
            this.editCopy.Click += new System.EventHandler(this.Copy);
            // 
            // editPaste
            // 
            this.editPaste.Enabled = false;
            this.editPaste.Image = global::Necrofy.Properties.Resources.clipboard_paste;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.editPaste, this.pasteButton);
            this.editPaste.Name = "editPaste";
            this.editPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.editPaste.Size = new System.Drawing.Size(211, 22);
            this.editPaste.Text = "&Paste";
            this.editPaste.Click += new System.EventHandler(this.Paste);
            // 
            // editDelete
            // 
            this.editDelete.Enabled = false;
            this.editDelete.Image = global::Necrofy.Properties.Resources.cross_script;
            this.editDelete.Name = "editDelete";
            this.editDelete.ShortcutKeyDisplayString = "";
            this.editDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.editDelete.Size = new System.Drawing.Size(211, 22);
            this.editDelete.Text = "&Delete";
            this.editDelete.Click += new System.EventHandler(this.Delete);
            // 
            // editSeparator2
            // 
            this.editSeparator2.Name = "editSeparator2";
            this.editSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // editSelectAll
            // 
            this.editSelectAll.Enabled = false;
            this.editSelectAll.Name = "editSelectAll";
            this.editSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.editSelectAll.Size = new System.Drawing.Size(211, 22);
            this.editSelectAll.Text = "Select &All";
            this.editSelectAll.Click += new System.EventHandler(this.SelectAll);
            // 
            // editSelectNone
            // 
            this.editSelectNone.Enabled = false;
            this.editSelectNone.Name = "editSelectNone";
            this.editSelectNone.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.editSelectNone.Size = new System.Drawing.Size(211, 22);
            this.editSelectNone.Text = "Select &None";
            this.editSelectNone.Click += new System.EventHandler(this.SelectNone);
            // 
            // buildMenu
            // 
            this.buildMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildBuildProject,
            this.buildRunProject,
            this.buildRunFromLevel,
            this.buildRunSettings,
            this.debugToolStripMenuItem});
            this.buildMenu.Name = "buildMenu";
            this.buildMenu.Size = new System.Drawing.Size(46, 20);
            this.buildMenu.Text = "&Build";
            // 
            // buildBuildProject
            // 
            this.buildBuildProject.Enabled = false;
            this.buildBuildProject.Image = global::Necrofy.Properties.Resources.compile;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.buildBuildProject, this.buildProjectButton);
            this.buildBuildProject.Name = "buildBuildProject";
            this.buildBuildProject.Size = new System.Drawing.Size(210, 22);
            this.buildBuildProject.Text = "&Build Project";
            this.buildBuildProject.Click += new System.EventHandler(this.BuildProject);
            // 
            // buildRunProject
            // 
            this.buildRunProject.Enabled = false;
            this.buildRunProject.Image = global::Necrofy.Properties.Resources.control;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.buildRunProject, this.runProjectButton);
            this.buildRunProject.Name = "buildRunProject";
            this.buildRunProject.Size = new System.Drawing.Size(210, 22);
            this.buildRunProject.Text = "&Run Project";
            this.buildRunProject.Click += new System.EventHandler(this.RunProject);
            // 
            // buildRunFromLevel
            // 
            this.buildRunFromLevel.Enabled = false;
            this.buildRunFromLevel.Image = global::Necrofy.Properties.Resources.control_cursor;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.buildRunFromLevel, this.runFromLevelButton);
            this.buildRunFromLevel.Name = "buildRunFromLevel";
            this.buildRunFromLevel.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.buildRunFromLevel.Size = new System.Drawing.Size(210, 22);
            this.buildRunFromLevel.Text = "Run From &Level";
            this.buildRunFromLevel.Click += new System.EventHandler(this.RunFromLevel);
            // 
            // buildRunSettings
            // 
            this.buildRunSettings.Name = "buildRunSettings";
            this.buildRunSettings.Size = new System.Drawing.Size(210, 22);
            this.buildRunSettings.Text = "Run From Level &Settings...";
            this.buildRunSettings.Click += new System.EventHandler(this.RunFromLevelSettings);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewGrid,
            this.viewSolidTilesOnly,
            this.viewTilePriority,
            this.toolStripSeparator6,
            this.viewRespawnAreas,
            this.viewScreenSizeGuide,
            this.toolStripSeparator7,
            this.viewAnimate,
            this.viewNextFrame,
            this.viewRestartAnimation,
            this.toolStripSeparator10,
            this.viewAxes,
            this.viewTileBorders,
            this.toolStripSeparator13,
            this.viewZoomOut,
            this.viewZoomIn});
            this.toolStripGrouper.SetItemSet(this.viewMenu, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelEditor | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "&View";
            // 
            // viewGrid
            // 
            this.viewGrid.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewGrid, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewGrid, Necrofy.ToolStripGrouper.ItemType.ViewGrid);
            this.viewGrid.Name = "viewGrid";
            this.viewGrid.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.viewGrid.Size = new System.Drawing.Size(208, 22);
            this.viewGrid.Text = "&Grid";
            // 
            // viewSolidTilesOnly
            // 
            this.viewSolidTilesOnly.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewSolidTilesOnly, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewSolidTilesOnly, Necrofy.ToolStripGrouper.ItemType.ViewSolidTilesOnly);
            this.viewSolidTilesOnly.Name = "viewSolidTilesOnly";
            this.viewSolidTilesOnly.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.viewSolidTilesOnly.Size = new System.Drawing.Size(208, 22);
            this.viewSolidTilesOnly.Text = "&Solid Tiles Only";
            // 
            // viewTilePriority
            // 
            this.viewTilePriority.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewTilePriority, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewTilePriority, Necrofy.ToolStripGrouper.ItemType.ViewTilePriority);
            this.viewTilePriority.Name = "viewTilePriority";
            this.viewTilePriority.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.viewTilePriority.Size = new System.Drawing.Size(208, 22);
            this.viewTilePriority.Text = "Tile &Priority";
            // 
            // toolStripSeparator6
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator6, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(205, 6);
            // 
            // viewRespawnAreas
            // 
            this.viewRespawnAreas.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewRespawnAreas, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewRespawnAreas, Necrofy.ToolStripGrouper.ItemType.ViewRespawnAreas);
            this.viewRespawnAreas.Name = "viewRespawnAreas";
            this.viewRespawnAreas.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.viewRespawnAreas.Size = new System.Drawing.Size(208, 22);
            this.viewRespawnAreas.Text = "R&espawn Areas";
            // 
            // viewScreenSizeGuide
            // 
            this.viewScreenSizeGuide.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewScreenSizeGuide, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewScreenSizeGuide, Necrofy.ToolStripGrouper.ItemType.ViewScreenSizeGuide);
            this.viewScreenSizeGuide.Name = "viewScreenSizeGuide";
            this.viewScreenSizeGuide.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.viewScreenSizeGuide.Size = new System.Drawing.Size(208, 22);
            this.viewScreenSizeGuide.Text = "Screen Size G&uide";
            // 
            // toolStripSeparator7
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator7, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(205, 6);
            // 
            // viewAnimate
            // 
            this.viewAnimate.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewAnimate, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewAnimate, Necrofy.ToolStripGrouper.ItemType.ViewAnimate);
            this.viewAnimate.Name = "viewAnimate";
            this.viewAnimate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.viewAnimate.Size = new System.Drawing.Size(208, 22);
            this.viewAnimate.Text = "&Animate";
            // 
            // viewNextFrame
            // 
            this.toolStripGrouper.SetItemSet(this.viewNextFrame, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewNextFrame, Necrofy.ToolStripGrouper.ItemType.ViewNextFrame);
            this.viewNextFrame.Name = "viewNextFrame";
            this.viewNextFrame.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.viewNextFrame.Size = new System.Drawing.Size(208, 22);
            this.viewNextFrame.Text = "&Next Frame";
            // 
            // viewRestartAnimation
            // 
            this.toolStripGrouper.SetItemSet(this.viewRestartAnimation, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripGrouper.SetItemType(this.viewRestartAnimation, Necrofy.ToolStripGrouper.ItemType.ViewRestartAnimation);
            this.viewRestartAnimation.Name = "viewRestartAnimation";
            this.viewRestartAnimation.Size = new System.Drawing.Size(208, 22);
            this.viewRestartAnimation.Text = "&Restart Animation";
            // 
            // toolStripSeparator10
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator10, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(205, 6);
            // 
            // viewAxes
            // 
            this.viewAxes.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewAxes, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.viewAxes, Necrofy.ToolStripGrouper.ItemType.ViewAxes);
            this.viewAxes.Name = "viewAxes";
            this.viewAxes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.viewAxes.Size = new System.Drawing.Size(208, 22);
            this.viewAxes.Text = "&Axes";
            // 
            // viewTileBorders
            // 
            this.viewTileBorders.CheckOnClick = true;
            this.toolStripGrouper.SetItemSet(this.viewTileBorders, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.viewTileBorders, Necrofy.ToolStripGrouper.ItemType.ViewTileBorders);
            this.viewTileBorders.Name = "viewTileBorders";
            this.viewTileBorders.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.viewTileBorders.Size = new System.Drawing.Size(208, 22);
            this.viewTileBorders.Text = "Tile &Borders";
            // 
            // toolStripSeparator13
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator13, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(205, 6);
            // 
            // viewZoomOut
            // 
            this.viewZoomOut.Enabled = false;
            this.viewZoomOut.Image = global::Necrofy.Properties.Resources.magnifier_zoom_out;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.viewZoomOut, this.zoomOutButton);
            this.viewZoomOut.Name = "viewZoomOut";
            this.viewZoomOut.ShortcutKeyDisplayString = "Ctrl+-";
            this.viewZoomOut.Size = new System.Drawing.Size(208, 22);
            this.viewZoomOut.Text = "Zoom &Out";
            this.viewZoomOut.Click += new System.EventHandler(this.ZoomOut);
            // 
            // viewZoomIn
            // 
            this.viewZoomIn.Enabled = false;
            this.viewZoomIn.Image = global::Necrofy.Properties.Resources.magnifier_zoom_in;
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.viewZoomIn, this.zoomInButton);
            this.viewZoomIn.Name = "viewZoomIn";
            this.viewZoomIn.ShortcutKeyDisplayString = "Ctrl++";
            this.viewZoomIn.Size = new System.Drawing.Size(208, 22);
            this.viewZoomIn.Text = "Zoom &In";
            this.viewZoomIn.Click += new System.EventHandler(this.ZoomIn);
            // 
            // levelMenu
            // 
            this.levelMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelEditTitle,
            this.levelSettings,
            this.toolStripSeparator5,
            this.levelClear,
            this.toolStripSeparator8,
            this.levelSaveAsImage});
            this.toolStripGrouper.SetItemSet(this.levelMenu, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.levelMenu.Name = "levelMenu";
            this.levelMenu.Size = new System.Drawing.Size(46, 20);
            this.levelMenu.Text = "&Level";
            // 
            // levelEditTitle
            // 
            this.toolStripGrouper.SetItemType(this.levelEditTitle, Necrofy.ToolStripGrouper.ItemType.LevelEditTitle);
            this.levelEditTitle.Name = "levelEditTitle";
            this.levelEditTitle.Size = new System.Drawing.Size(159, 22);
            this.levelEditTitle.Text = "Edit Title...";
            // 
            // levelSettings
            // 
            this.toolStripGrouper.SetItemType(this.levelSettings, Necrofy.ToolStripGrouper.ItemType.LevelSettings);
            this.levelSettings.Name = "levelSettings";
            this.levelSettings.Size = new System.Drawing.Size(159, 22);
            this.levelSettings.Text = "&Settings...";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(156, 6);
            // 
            // levelClear
            // 
            this.toolStripGrouper.SetItemType(this.levelClear, Necrofy.ToolStripGrouper.ItemType.LevelClear);
            this.levelClear.Name = "levelClear";
            this.levelClear.Size = new System.Drawing.Size(159, 22);
            this.levelClear.Text = "Clear";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(156, 6);
            // 
            // levelSaveAsImage
            // 
            this.toolStripGrouper.SetItemType(this.levelSaveAsImage, Necrofy.ToolStripGrouper.ItemType.LevelSaveAsImage);
            this.levelSaveAsImage.Name = "levelSaveAsImage";
            this.levelSaveAsImage.Size = new System.Drawing.Size(159, 22);
            this.levelSaveAsImage.Text = "Save As Image...";
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsPaintbrush,
            this.toolsTileSuggest,
            this.toolsRectangleSelect,
            this.toolsPencilSelect,
            this.toolsTileSelect,
            this.toolsResizeLevel,
            this.toolsSprites});
            this.toolStripGrouper.SetItemSet(this.toolsMenu, Necrofy.ToolStripGrouper.ItemSet.LevelEditor);
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(46, 20);
            this.toolsMenu.Text = "&Tools";
            // 
            // toolsPaintbrush
            // 
            this.toolsPaintbrush.Checked = true;
            this.toolsPaintbrush.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolsPaintbrush.Image = global::Necrofy.Properties.Resources.paint_brush;
            this.toolStripGrouper.SetItemType(this.toolsPaintbrush, Necrofy.ToolStripGrouper.ItemType.PaintbrushTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsPaintbrush, this.paintbrushButton);
            this.toolsPaintbrush.Name = "toolsPaintbrush";
            this.toolsPaintbrush.ShortcutKeyDisplayString = "P";
            this.toolsPaintbrush.Size = new System.Drawing.Size(174, 22);
            this.toolsPaintbrush.Text = "&Paintbrush";
            // 
            // toolsTileSuggest
            // 
            this.toolsTileSuggest.Image = global::Necrofy.Properties.Resources.light_bulb;
            this.toolStripGrouper.SetItemType(this.toolsTileSuggest, Necrofy.ToolStripGrouper.ItemType.TileSuggestTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsTileSuggest, this.tileSuggestButton);
            this.toolsTileSuggest.Name = "toolsTileSuggest";
            this.toolsTileSuggest.ShortcutKeyDisplayString = "S";
            this.toolsTileSuggest.Size = new System.Drawing.Size(174, 22);
            this.toolsTileSuggest.Text = "Tile &Suggest";
            // 
            // toolsRectangleSelect
            // 
            this.toolsRectangleSelect.Image = global::Necrofy.Properties.Resources.selection_select;
            this.toolStripGrouper.SetItemType(this.toolsRectangleSelect, Necrofy.ToolStripGrouper.ItemType.RectangleSelectTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsRectangleSelect, this.rectangleSelectButton);
            this.toolsRectangleSelect.Name = "toolsRectangleSelect";
            this.toolsRectangleSelect.ShortcutKeyDisplayString = "R";
            this.toolsRectangleSelect.Size = new System.Drawing.Size(174, 22);
            this.toolsRectangleSelect.Text = "&Rectangle Select";
            // 
            // toolsPencilSelect
            // 
            this.toolsPencilSelect.Image = global::Necrofy.Properties.Resources.pencil_select;
            this.toolStripGrouper.SetItemType(this.toolsPencilSelect, Necrofy.ToolStripGrouper.ItemType.PencilSelectTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsPencilSelect, this.pencilSelectButton);
            this.toolsPencilSelect.Name = "toolsPencilSelect";
            this.toolsPencilSelect.ShortcutKeyDisplayString = "C";
            this.toolsPencilSelect.Size = new System.Drawing.Size(174, 22);
            this.toolsPencilSelect.Text = "Pen&cil Select";
            // 
            // toolsTileSelect
            // 
            this.toolsTileSelect.Image = global::Necrofy.Properties.Resources.tile_select;
            this.toolStripGrouper.SetItemType(this.toolsTileSelect, Necrofy.ToolStripGrouper.ItemType.TileSelectTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsTileSelect, this.tileSelectButton);
            this.toolsTileSelect.Name = "toolsTileSelect";
            this.toolsTileSelect.ShortcutKeyDisplayString = "T";
            this.toolsTileSelect.Size = new System.Drawing.Size(174, 22);
            this.toolsTileSelect.Text = "&Tile Select";
            // 
            // toolsResizeLevel
            // 
            this.toolsResizeLevel.Image = global::Necrofy.Properties.Resources.map_resize;
            this.toolStripGrouper.SetItemType(this.toolsResizeLevel, Necrofy.ToolStripGrouper.ItemType.ResizeLevelTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsResizeLevel, this.resizeLevelButton);
            this.toolsResizeLevel.Name = "toolsResizeLevel";
            this.toolsResizeLevel.ShortcutKeyDisplayString = "L";
            this.toolsResizeLevel.Size = new System.Drawing.Size(174, 22);
            this.toolsResizeLevel.Text = "Resize &Level";
            // 
            // toolsSprites
            // 
            this.toolsSprites.Image = global::Necrofy.Properties.Resources.leaf;
            this.toolStripGrouper.SetItemType(this.toolsSprites, Necrofy.ToolStripGrouper.ItemType.SpriteTool);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.toolsSprites, this.spritesButton);
            this.toolsSprites.Name = "toolsSprites";
            this.toolsSprites.ShortcutKeyDisplayString = "I";
            this.toolsSprites.Size = new System.Drawing.Size(174, 22);
            this.toolsSprites.Text = "Spr&ites";
            // 
            // arrangeMenu
            // 
            this.arrangeMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arrangeFlipHorizontal,
            this.arrangeFlipVertical,
            this.toolStripSeparator12,
            this.arrangeCenterHorizontally,
            this.arrangeCenterVertically,
            this.titleSeparator,
            this.arrangeMoveUp,
            this.arrangeMoveDown,
            this.arrangeMoveToFront,
            this.arrangeMoveToBack});
            this.toolStripGrouper.SetItemSet(this.arrangeMenu, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.arrangeMenu.Name = "arrangeMenu";
            this.arrangeMenu.Size = new System.Drawing.Size(61, 20);
            this.arrangeMenu.Text = "&Arrange";
            // 
            // arrangeFlipHorizontal
            // 
            this.arrangeFlipHorizontal.Image = global::Necrofy.Properties.Resources.layer_flip;
            this.toolStripGrouper.SetItemSet(this.arrangeFlipHorizontal, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.arrangeFlipHorizontal, Necrofy.ToolStripGrouper.ItemType.FlipHorizontally);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeFlipHorizontal, this.flipHorizontalButton);
            this.arrangeFlipHorizontal.Name = "arrangeFlipHorizontal";
            this.arrangeFlipHorizontal.Size = new System.Drawing.Size(176, 22);
            this.arrangeFlipHorizontal.Text = "Flip H&orizontally";
            // 
            // arrangeFlipVertical
            // 
            this.arrangeFlipVertical.Image = global::Necrofy.Properties.Resources.layer_flip_vertical;
            this.toolStripGrouper.SetItemSet(this.arrangeFlipVertical, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripGrouper.SetItemType(this.arrangeFlipVertical, Necrofy.ToolStripGrouper.ItemType.FlipVertically);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeFlipVertical, this.flipVerticalButton);
            this.arrangeFlipVertical.Name = "arrangeFlipVertical";
            this.arrangeFlipVertical.Size = new System.Drawing.Size(176, 22);
            this.arrangeFlipVertical.Text = "Flip V&ertically";
            // 
            // toolStripSeparator12
            // 
            this.toolStripGrouper.SetItemSet(this.toolStripSeparator12, Necrofy.ToolStripGrouper.ItemSet.SpriteEditor);
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(173, 6);
            // 
            // arrangeCenterHorizontally
            // 
            this.arrangeCenterHorizontally.Image = global::Necrofy.Properties.Resources.layers_alignment_center;
            this.toolStripGrouper.SetItemSet(this.arrangeCenterHorizontally, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeCenterHorizontally, Necrofy.ToolStripGrouper.ItemType.CenterHorizontally);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeCenterHorizontally, this.centerHorizontallyButton);
            this.arrangeCenterHorizontally.Name = "arrangeCenterHorizontally";
            this.arrangeCenterHorizontally.Size = new System.Drawing.Size(176, 22);
            this.arrangeCenterHorizontally.Text = "Center &Horizontally";
            // 
            // arrangeCenterVertically
            // 
            this.arrangeCenterVertically.Image = global::Necrofy.Properties.Resources.layers_alignment_middle;
            this.toolStripGrouper.SetItemSet(this.arrangeCenterVertically, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeCenterVertically, Necrofy.ToolStripGrouper.ItemType.CenterVertically);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeCenterVertically, this.centerVerticallyButton);
            this.arrangeCenterVertically.Name = "arrangeCenterVertically";
            this.arrangeCenterVertically.Size = new System.Drawing.Size(176, 22);
            this.arrangeCenterVertically.Text = "Center &Vertically";
            // 
            // titleSeparator
            // 
            this.toolStripGrouper.SetItemSet(this.titleSeparator, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.titleSeparator.Name = "titleSeparator";
            this.titleSeparator.Size = new System.Drawing.Size(173, 6);
            // 
            // arrangeMoveUp
            // 
            this.arrangeMoveUp.Image = global::Necrofy.Properties.Resources.arrow_090;
            this.toolStripGrouper.SetItemSet(this.arrangeMoveUp, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeMoveUp, Necrofy.ToolStripGrouper.ItemType.MoveUp);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeMoveUp, this.moveUpButton);
            this.arrangeMoveUp.Name = "arrangeMoveUp";
            this.arrangeMoveUp.Size = new System.Drawing.Size(176, 22);
            this.arrangeMoveUp.Text = "Move &Up";
            // 
            // arrangeMoveDown
            // 
            this.arrangeMoveDown.Image = global::Necrofy.Properties.Resources.arrow_270;
            this.toolStripGrouper.SetItemSet(this.arrangeMoveDown, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeMoveDown, Necrofy.ToolStripGrouper.ItemType.MoveDown);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeMoveDown, this.moveDownButton);
            this.arrangeMoveDown.Name = "arrangeMoveDown";
            this.arrangeMoveDown.Size = new System.Drawing.Size(176, 22);
            this.arrangeMoveDown.Text = "Move &Down";
            // 
            // arrangeMoveToFront
            // 
            this.arrangeMoveToFront.Image = global::Necrofy.Properties.Resources.arrow_stop_090;
            this.toolStripGrouper.SetItemSet(this.arrangeMoveToFront, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeMoveToFront, Necrofy.ToolStripGrouper.ItemType.MoveToFront);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeMoveToFront, this.moveToFrontButton);
            this.arrangeMoveToFront.Name = "arrangeMoveToFront";
            this.arrangeMoveToFront.Size = new System.Drawing.Size(176, 22);
            this.arrangeMoveToFront.Text = "Move to &Front";
            // 
            // arrangeMoveToBack
            // 
            this.arrangeMoveToBack.Image = global::Necrofy.Properties.Resources.arrow_stop_270;
            this.toolStripGrouper.SetItemSet(this.arrangeMoveToBack, ((Necrofy.ToolStripGrouper.ItemSet)((Necrofy.ToolStripGrouper.ItemSet.LevelTitle | Necrofy.ToolStripGrouper.ItemSet.SpriteEditor))));
            this.toolStripGrouper.SetItemType(this.arrangeMoveToBack, Necrofy.ToolStripGrouper.ItemType.MoveToBack);
            this.toolBarMenuLinker.SetLinkedToolBarItem(this.arrangeMoveToBack, this.moveToBackButton);
            this.arrangeMoveToBack.Name = "arrangeMoveToBack";
            this.arrangeMoveToBack.Size = new System.Drawing.Size(176, 22);
            this.arrangeMoveToBack.Text = "Move to &Back";
            // 
            // windowMenu
            // 
            this.windowMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowProject,
            this.windowObjects,
            this.windowProperties,
            this.windowSeparator1,
            this.windowRestore});
            this.windowMenu.Name = "windowMenu";
            this.windowMenu.Size = new System.Drawing.Size(63, 20);
            this.windowMenu.Text = "&Window";
            // 
            // windowProject
            // 
            this.windowProject.Name = "windowProject";
            this.windowProject.Size = new System.Drawing.Size(193, 22);
            this.windowProject.Text = "Pro&ject";
            this.windowProject.Click += new System.EventHandler(this.windowProject_Click);
            // 
            // windowObjects
            // 
            this.windowObjects.Name = "windowObjects";
            this.windowObjects.Size = new System.Drawing.Size(193, 22);
            this.windowObjects.Text = "&Objects";
            this.windowObjects.Click += new System.EventHandler(this.windowObjects_Click);
            // 
            // windowProperties
            // 
            this.windowProperties.Name = "windowProperties";
            this.windowProperties.Size = new System.Drawing.Size(193, 22);
            this.windowProperties.Text = "&Properties";
            this.windowProperties.Click += new System.EventHandler(this.windowProperties_Click);
            // 
            // windowSeparator1
            // 
            this.windowSeparator1.Name = "windowSeparator1";
            this.windowSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // windowRestore
            // 
            this.windowRestore.Name = "windowRestore";
            this.windowRestore.Size = new System.Drawing.Size(193, 22);
            this.windowRestore.Text = "&Restore Default Layout";
            this.windowRestore.Click += new System.EventHandler(this.windowRestore_Click);
            // 
            // openProjectDialog
            // 
            this.openProjectDialog.Filter = "Necrofy project files (*.nfyp)|*.nfyp|All Files (*.*)|*.*";
            this.openProjectDialog.Title = "Open Project";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.buildStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 628);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1061, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(1046, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buildStatusLabel
            // 
            this.buildStatusLabel.Image = global::Necrofy.Properties.Resources.tick_button;
            this.buildStatusLabel.Name = "buildStatusLabel";
            this.buildStatusLabel.Size = new System.Drawing.Size(85, 17);
            this.buildStatusLabel.Text = "Build Status";
            this.buildStatusLabel.Visible = false;
            // 
            // toolStripGrouper
            // 
            this.toolStripGrouper.ItemClick += new System.EventHandler<Necrofy.ToolStripGrouper.ItemEventArgs>(this.toolStripGrouper_ItemClick);
            this.toolStripGrouper.ItemCheckedChanged += new System.EventHandler<Necrofy.ToolStripGrouper.ItemEventArgs>(this.toolStripGrouper_ItemCheckedChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 650);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Necrofy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton createProjectButton;
        private System.Windows.Forms.ToolStripButton buildProjectButton;
        private WeifenLuo.WinFormsUI.Docking.VS2012LightTheme vS2012LightTheme1;
        private System.Windows.Forms.ToolStripButton openProjectButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileNewProject;
        private System.Windows.Forms.ToolStripMenuItem fileOpenProject;
        private System.Windows.Forms.ToolStripSeparator fileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fileSave;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAll;
        private System.Windows.Forms.ToolStripSeparator fileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileExit;
        private System.Windows.Forms.ToolStripMenuItem buildMenu;
        private System.Windows.Forms.ToolStripMenuItem buildBuildProject;
        private System.Windows.Forms.ToolStripMenuItem buildRunProject;
        private System.Windows.Forms.ToolStripMenuItem buildRunFromLevel;
        private System.Windows.Forms.ToolStripMenuItem buildRunSettings;
        private System.Windows.Forms.OpenFileDialog openProjectDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton runProjectButton;
        private System.Windows.Forms.ToolStripButton runFromLevelButton;
        private System.Windows.Forms.ToolStripSeparator fileSeparator3;
        private RecentFilesMenu recentProjects;
        private System.Windows.Forms.ToolStripSeparator levelToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem editUndo;
        private System.Windows.Forms.ToolStripMenuItem editRedo;
        private System.Windows.Forms.ToolStripSeparator editSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editCut;
        private System.Windows.Forms.ToolStripMenuItem editCopy;
        private System.Windows.Forms.ToolStripMenuItem editPaste;
        private System.Windows.Forms.ToolStripMenuItem editDelete;
        private System.Windows.Forms.ToolStripSeparator editSeparator2;
        private System.Windows.Forms.ToolStripMenuItem editSelectAll;
        private System.Windows.Forms.ToolStripMenuItem editSelectNone;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton saveAllButton;
        private System.Windows.Forms.ToolStripButton cutButton;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton undoButton;
        private System.Windows.Forms.ToolStripSplitButton redoButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileClose;
        private System.Windows.Forms.ToolStripMenuItem windowMenu;
        private System.Windows.Forms.ToolStripMenuItem windowProject;
        private System.Windows.Forms.ToolStripMenuItem windowObjects;
        private System.Windows.Forms.ToolStripSeparator windowSeparator1;
        private System.Windows.Forms.ToolStripMenuItem windowRestore;
        private System.Windows.Forms.ToolStripMenuItem windowProperties;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem toolsPaintbrush;
        private System.Windows.Forms.ToolStripMenuItem toolsTileSuggest;
        private System.Windows.Forms.ToolStripMenuItem toolsRectangleSelect;
        private System.Windows.Forms.ToolStripMenuItem toolsPencilSelect;
        private System.Windows.Forms.ToolStripMenuItem toolsTileSelect;
        private System.Windows.Forms.ToolStripMenuItem toolsResizeLevel;
        private System.Windows.Forms.ToolStripMenuItem toolsSprites;
        private System.Windows.Forms.ToolStripButton paintbrushButton;
        private System.Windows.Forms.ToolStripButton tileSuggestButton;
        private System.Windows.Forms.ToolStripButton rectangleSelectButton;
        private System.Windows.Forms.ToolStripButton pencilSelectButton;
        private System.Windows.Forms.ToolStripButton tileSelectButton;
        private System.Windows.Forms.ToolStripButton resizeLevelButton;
        private CheckableToolStripSplitButton spritesButton;
        private SeparateCheckToolStripMenuItem spritesItems;
        private SeparateCheckToolStripMenuItem spritesVictims;
        private SeparateCheckToolStripMenuItem spritesOneShotMonsters;
        private SeparateCheckToolStripMenuItem spritesMonsters;
        private SeparateCheckToolStripMenuItem spritesBossMonsters;
        private SeparateCheckToolStripMenuItem spritesPlayers;
        private System.Windows.Forms.ToolStripSeparator spritesSeparator;
        private System.Windows.Forms.ToolStripMenuItem spritesAll;
        private ToolStripGrouper toolStripGrouper;
        private ToolBarMenuLinker toolBarMenuLinker;
        private System.Windows.Forms.ToolStripMenuItem levelMenu;
        private System.Windows.Forms.ToolStripMenuItem levelSettings;
        private System.Windows.Forms.ToolStripMenuItem levelEditTitle;
        private System.Windows.Forms.ToolStripSeparator levelTitleToolStripSeparator;
        private System.Windows.Forms.ToolStripButton centerHorizontallyButton;
        private System.Windows.Forms.ToolStripButton centerVerticallyButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDownButton;
        private System.Windows.Forms.ToolStripButton moveToFrontButton;
        private System.Windows.Forms.ToolStripButton moveToBackButton;
        private System.Windows.Forms.ToolStripMenuItem arrangeMenu;
        private System.Windows.Forms.ToolStripMenuItem arrangeCenterHorizontally;
        private System.Windows.Forms.ToolStripMenuItem arrangeCenterVertically;
        private System.Windows.Forms.ToolStripSeparator titleSeparator;
        private System.Windows.Forms.ToolStripMenuItem arrangeMoveUp;
        private System.Windows.Forms.ToolStripMenuItem arrangeMoveDown;
        private System.Windows.Forms.ToolStripMenuItem arrangeMoveToFront;
        private System.Windows.Forms.ToolStripMenuItem arrangeMoveToBack;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem levelClear;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem viewGrid;
        private System.Windows.Forms.ToolStripMenuItem viewSolidTilesOnly;
        private System.Windows.Forms.ToolStripMenuItem viewTilePriority;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem viewRespawnAreas;
        private System.Windows.Forms.ToolStripMenuItem viewScreenSizeGuide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem viewAnimate;
        private System.Windows.Forms.ToolStripMenuItem viewNextFrame;
        private System.Windows.Forms.ToolStripMenuItem viewRestartAnimation;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem levelSaveAsImage;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel buildStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton zoomOutButton;
        private System.Windows.Forms.ToolStripButton zoomInButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem viewZoomOut;
        private System.Windows.Forms.ToolStripMenuItem viewZoomIn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton flipHorizontalButton;
        private System.Windows.Forms.ToolStripButton flipVerticalButton;
        private System.Windows.Forms.ToolStripMenuItem arrangeFlipHorizontal;
        private System.Windows.Forms.ToolStripMenuItem arrangeFlipVertical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem viewAxes;
        private System.Windows.Forms.ToolStripMenuItem viewTileBorders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripLabel zoomLevelLabel;
    }
}

