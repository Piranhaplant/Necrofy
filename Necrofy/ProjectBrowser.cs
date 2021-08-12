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
            { AssetCategory.Demo, Resources.film },
            { AssetCategory.Editor, Resources.gear },
            { AssetCategory.Graphics, Resources.image },
            { AssetCategory.Level, Resources.map },
            { AssetCategory.Palette, Resources.color },
            { AssetCategory.Passwords, Resources.document_binary },
            { AssetCategory.Sprites, Resources.ghost },
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
            if (this.project != null) {
                this.project.Assets.AssetChanged -= AssetChanged;
                this.project.Assets.AssetAdded -= AssetAdded;
                this.project.Assets.AssetRemoved -= AssetRemoved;
            }
            this.project = project;
            tree.Nodes.Clear();

            if (project != null) {
                PopulateTree(tree.Nodes, project.Assets.Root);
                if (project.userSettings.FolderStates != null) {
                    LoadFolderStates(tree.Nodes, project.userSettings.FolderStates);
                } else {
                    tree.Nodes[LevelAsset.Folder]?.Expand();
                }
                if (tree.Nodes.Count > 0) {
                    tree.SelectedNode = tree.Nodes[0]; // Scroll to the top regardless of what folders were opened
                }
                project.Assets.AssetChanged += AssetChanged;
                project.Assets.AssetAdded += AssetAdded;
                project.Assets.AssetRemoved += AssetRemoved;
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
            if (child.Nodes.Count == 0) {
                child.Remove();
            }
        }

        private void PopulateAsset(TreeNodeCollection parent, AssetTree.AssetEntry entry) {
            TreeNode child = parent.Add(entry.Asset.DisplayName);
            child.Name = child.Text;
            SetImage(child, entry.Asset.Category);
            child.ForeColor = entry.Asset.Editable ? SystemColors.ControlText : SystemColors.GrayText;
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
                TreeNode parent = null;
                AssetTree.Node node = e.Asset;
                while (node.Parent.Parent != null) {
                    parent = tree.Nodes.FindNodeByTag(node.Parent);
                    if (parent != null) {
                        break;
                    }
                    node = node.Parent;
                }
                TreeNodeCollection collection = parent == null ? tree.Nodes : parent.Nodes;
                if (node is AssetTree.Folder folder) {
                    PopulateFolder(collection, folder);
                } else if (node is AssetTree.AssetEntry asset) {
                    PopulateAsset(collection, asset);
                }
            });
        }

        private void AssetRemoved(object sender, AssetEventArgs e) {
            Invoke((MethodInvoker)delegate {
                RemoveNode(tree.Nodes.FindNodeByTag(e.Asset));
            });
        }
        
        private void RemoveNode(TreeNode node) {
            TreeNode parent = node.Parent;
            node.Remove();
            if (parent != null && parent.Nodes.Count == 0) {
                RemoveNode(parent);
            }
        }

        private void LoadFolderStates(TreeNodeCollection parent, List<ProjectUserSettings.FolderState> folderStates) {
            foreach (ProjectUserSettings.FolderState folderState in folderStates) {
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
            project.userSettings.FolderStates = new List<ProjectUserSettings.FolderState>();
            SaveFolderStates(tree.Nodes, project.userSettings.FolderStates);
        }

        private void SaveFolderStates(TreeNodeCollection parent, List<ProjectUserSettings.FolderState> folderStates) {
            foreach (TreeNode n in parent) {
                if (n.Nodes.Count > 0) {
                    ProjectUserSettings.FolderState folderState = new ProjectUserSettings.FolderState();
                    folderState.Name = n.Name;
                    folderState.Expanded = n.IsExpanded;
                    folderState.Children = new List<ProjectUserSettings.FolderState>();
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

        private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            tree.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right && e.Node.Tag is AssetTree.AssetEntry entry) {
                contextOpen.Enabled = entry.Asset.Editable;
                contextMenu.Show(Cursor.Position);
            }
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            // Fix bug where double clicking a folder can cause this event to happen on a child node
            if (!e.Node.IsSelected) {
                return;
            }
            OpenNode(e.Node);
        }

        private void contextOpen_Click(object sender, EventArgs e) {
            OpenNode(tree.SelectedNode);
        }

        private void OpenNode(TreeNode node) {
            if (node.Tag is AssetTree.AssetEntry entry) {
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
