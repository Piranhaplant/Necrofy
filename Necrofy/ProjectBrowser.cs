using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Necrofy.Properties;

namespace Necrofy
{
    partial class ProjectBrowser : DockContent
    {
        private const int DocumentImageIndex = 0;
        private const int FolderImageIndex = 1;
        private const int FolderOpenImageIndex = 2;

        private readonly Dictionary<AssetCategory, Bitmap> displayImages = new Dictionary<AssetCategory, Bitmap> {
            { AssetCategory.Collision, Resources.block },
            { AssetCategory.Editor, Resources.gear },
            { AssetCategory.Graphics, Resources.image },
            { AssetCategory.Level, Resources.map },
            { AssetCategory.Palette, Resources.color },
            { AssetCategory.Tilemap, Resources.layout_4 },
        };
        private readonly Dictionary<AssetCategory, int> displayImageIndexes = new Dictionary<AssetCategory, int>();
        private readonly Dictionary<AssetCategory, Icon> displayIcons = new Dictionary<AssetCategory, Icon>();

        private readonly MainWindow mainWindow;
        private Project project;

        public ProjectBrowser(MainWindow mainWindow) {
            InitializeComponent();
            this.mainWindow = mainWindow;
            treeImages.Images.Add(Resources.document); // Used as default image
            treeImages.Images.Add(Resources.folder);
            treeImages.Images.Add(Resources.folder_open);
        }

        public void OpenProject(Project project) {
            this.project = project;
            tree.Nodes.Clear();
            if (project != null) {
                PopulateTree(tree.Nodes, project.path);
            }
        }

        private void PopulateTree(TreeNodeCollection parent, string path) {
            string[] dirs = Directory.GetDirectories(path);
            Array.Sort(dirs, NumericStringComparer.instance);
            foreach (string dir in dirs) {
                TreeNode child = parent.Add(Path.GetFileName(dir));
                child.ImageIndex = FolderImageIndex;
                child.SelectedImageIndex = FolderImageIndex;

                PopulateTree(child.Nodes, dir);
                if (child.Nodes.Count == 0) {
                    child.Remove();
                }
            }

            string[] files = Directory.GetFiles(path);
            Array.Sort(files, NumericStringComparer.instance);
            foreach (string file in files) {
                Asset.NameInfo info = Asset.GetInfo(project, project.GetRelativePath(file));
                if (info != null) {
                    TreeNode child = parent.Add(info.DisplayName);
                    SetImage(child, info.Category);
                    child.Tag = info;
                }
            }
        }

        private void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            e.Node.ImageIndex = FolderOpenImageIndex;
            e.Node.SelectedImageIndex = FolderOpenImageIndex;
        }

        private void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            e.Node.ImageIndex = FolderImageIndex;
            e.Node.SelectedImageIndex = FolderImageIndex;
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            // Fix bug where double clicking a folder can cause this event to happen on a child node
            if (!e.Node.IsSelected) {
                return;
            }
            Asset.NameInfo info = e.Node.Tag as Asset.NameInfo;
            EditorWindow editor = info?.GetEditor(project);
            if (editor != null) {
                if (!displayIcons.ContainsKey(info.Category)) {
                    displayIcons[info.Category] = Icon.FromHandle(displayImages[info.Category].GetHicon());
                }
                editor.Icon = displayIcons[info.Category];
                mainWindow.ShowEditor(editor);
            }
        }

        private void SetImage(TreeNode node, AssetCategory category) {
            if (!displayImageIndexes.ContainsKey(category)) {
                displayImageIndexes[category] = treeImages.Images.Count;
                treeImages.Images.Add(displayImages[category]);
            }
            node.ImageIndex = displayImageIndexes[category];
            node.SelectedImageIndex = node.ImageIndex;
        }
    }
}
