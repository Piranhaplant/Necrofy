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
            { AssetCategory.Data, Resources.document_binary },
            { AssetCategory.Editor, Resources.gear },
            { AssetCategory.Graphics, Resources.image },
            { AssetCategory.Level, Resources.map },
            { AssetCategory.Palette, Resources.color },
            { AssetCategory.Sprites, Resources.leaf },
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
                PopulateTree(tree.Nodes, project.Assets.Root);
                if (project.settings.FolderStates != null) {
                    LoadFolderStates(tree.Nodes, project.settings.FolderStates);
                } else {
                    tree.Nodes[LevelAsset.Folder]?.Expand();
                }
                if (tree.Nodes.Count > 0) {
                    tree.SelectedNode = tree.Nodes[0]; // Scroll to the top regardless of what folders were opened
                }
                project.Assets.AssetChanged += AssetChanged;
                project.Assets.AssetAdded += AssetAdded;
                project.Assets.AssetRemoved += AssetRemoved;
                project.Assets.FolderAdded += FolderAdded;
                project.Assets.FolderRemoved += FolderRemoved;
            }
        }

        private void PopulateTree(TreeNodeCollection parent, AssetTree.Folder folder) {
            foreach (AssetTree.Folder subFolder in folder.Folders) {
                PopulateFolder(parent, subFolder);
            }

            foreach (AssetTree.AssetEntry entry in folder.Assets) {
                PopulateAsset(parent, entry);
            }
        }

        private void PopulateFolder(TreeNodeCollection parent, AssetTree.Folder subFolder) {
            TreeNode child = parent.Add(subFolder.Name);
            child.Name = child.Text;
            child.ImageIndex = FolderImageIndex;
            child.SelectedImageIndex = FolderImageIndex;
            child.Tag = subFolder;

            PopulateTree(child.Nodes, subFolder);
        }

        private void PopulateAsset(TreeNodeCollection parent, AssetTree.AssetEntry entry) {
            TreeNode child = parent.Add(entry.Asset.DisplayName);
            child.Name = child.Text;
            SetImage(child, entry.Asset.Category);
            child.Tag = entry;
        }

        private void AssetChanged(object sender, AssetEventArgs e) {
            Invoke((MethodInvoker)delegate {
                TreeNode node = tree.Nodes.FindNodeByTag(e.Asset);
                node.Text = e.Asset.Asset.DisplayName;
                node.Name = node.Text;
            });
        }

        private void AssetAdded(object sender, AssetEventArgs e) {
            Invoke((MethodInvoker)delegate {
                TreeNode parent = tree.Nodes.FindNodeByTag(e.Asset.Parent);
                PopulateAsset(parent.Nodes, e.Asset);
            });
        }

        private void AssetRemoved(object sender, AssetEventArgs e) {
            Invoke((MethodInvoker)delegate {
                tree.Nodes.FindNodeByTag(e.Asset).Remove();
            });
        }

        private void FolderAdded(object sender, FolderEventArgs e) {
            Invoke((MethodInvoker)delegate {
                TreeNode parent = tree.Nodes.FindNodeByTag(e.Folder.Parent);
                PopulateFolder(parent.Nodes, e.Folder);
            });
        }

        private void FolderRemoved(object sender, FolderEventArgs e) {
            Invoke((MethodInvoker)delegate {
                tree.Nodes.FindNodeByTag(e.Folder).Remove();
            });
        }

        private void LoadFolderStates(TreeNodeCollection parent, List<ProjectSettings.FolderState> folderStates) {
            foreach (ProjectSettings.FolderState folderState in folderStates) {
                TreeNode node = parent[folderState.Name];
                if (node != null) {
                    if (folderState.Expanded) {
                        node.Expand();
                    }
                    if (folderState.Children != null) {
                        LoadFolderStates(node.Nodes, folderState.Children);
                    }
                }
            }
        }

        public void SaveFolderStates() {
            project.settings.FolderStates = new List<ProjectSettings.FolderState>();
            SaveFolderStates(tree.Nodes, project.settings.FolderStates);
        }

        private void SaveFolderStates(TreeNodeCollection parent, List<ProjectSettings.FolderState> folderStates) {
            foreach (TreeNode n in parent) {
                if (n.Nodes.Count > 0) {
                    ProjectSettings.FolderState folderState = new ProjectSettings.FolderState();
                    folderState.Name = n.Name;
                    folderState.Expanded = n.IsExpanded;
                    folderState.Children = new List<ProjectSettings.FolderState>();
                    folderStates.Add(folderState);
                    SaveFolderStates(n.Nodes, folderState.Children);
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
            if (e.Node.Tag is AssetTree.AssetEntry entry) {
                try {
                    mainWindow.OpenAsset(entry.Asset);
                } catch (Exception ex) {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show($"Could not open file {entry.Asset.GetFilename("")}:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public Icon GetEditorIcon(AssetCategory assetCategory) {
            if (!displayIcons.ContainsKey(assetCategory)) {
                displayIcons[assetCategory] = Icon.FromHandle(displayImages[assetCategory].GetHicon());
            }
            return displayIcons[assetCategory];
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
