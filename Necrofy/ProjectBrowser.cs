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
        private readonly MainWindow mainWindow;
        private Project project;
        private TreeViewAssetTreePopulator treePopulator = null;

        public ProjectBrowser(MainWindow mainWindow) {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        public void OpenProject(Project project) {
            if (treePopulator != null) {
                treePopulator.Dispose();
            }
            this.project = project;
            tree.Nodes.Clear();

            if (project != null) {
                treePopulator = new TreeViewAssetTreePopulator(project.Assets, tree);
                if (project.userSettings.FolderStates != null) {
                    LoadFolderStates(tree.Nodes, project.userSettings.FolderStates);
                } else {
                    tree.Nodes[LevelAsset.Folder]?.Expand();
                }
                if (tree.Nodes.Count > 0) {
                    tree.SelectedNode = tree.Nodes[0]; // Scroll to the top regardless of what folders were opened
                }
            }
        }

        private class TreeViewAssetTreePopulator : AssetTreePopulator<TreeNodeCollection, TreeNode>
        {
            private readonly TreeView treeView;

            public TreeViewAssetTreePopulator(AssetTree tree, TreeView treeView) : base(tree, treeView.Nodes) {
                this.treeView = treeView;
                Load();
            }

            protected override void SetImageList(ImageList imageList) => treeView.ImageList = imageList;

            protected override TreeNode CreateChild(string text, object tag, bool isFolder, int imageIndex) {
                TreeNode node = new TreeNode(text);
                node.Name = text;
                node.Tag = tag;
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
                return node;
            }

            protected override List<AssetTree.Node> GetAssetTreeNodes(TreeNodeCollection parent) {
                return parent.Cast<TreeNode>().Select(n => n.Tag as AssetTree.Node).ToList();
            }

            public override TreeNode FindByTag(TreeNodeCollection collection, object tag) {
                foreach (TreeNode node in collection) {
                    if (node.Tag == tag) {
                        return node;
                    }
                    TreeNode child = FindByTag(node.Nodes, tag);
                    if (child != null) {
                        return child;
                    }
                }
                return null;
            }

            protected override TreeNodeCollection GetChildren(TreeNode node) => node.Nodes;
            protected override TreeNode GetParent(TreeNode node) => node.Parent;
            protected override bool IsEmpty(TreeNode node) => node.Nodes.Count == 0;
            protected override void Remove(TreeNode node) => node.Remove();
            protected override void Insert(TreeNodeCollection parent, TreeNode node, int index) => parent.Insert(index, node);
            protected override void SetColor(TreeNode node, Color color) => node.ForeColor = color;
            protected override void SetText(TreeNode node, string text) {
                node.Text = text;
                node.Name = text;
            }
            protected override TreeNode SelectedNode { get => treeView.SelectedNode; set => treeView.SelectedNode = value; }
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
            e.Node.ImageIndex = TreeViewAssetTreePopulator.FolderOpenImageIndex;
            e.Node.SelectedImageIndex = TreeViewAssetTreePopulator.FolderOpenImageIndex;
        }

        private void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            e.Node.ImageIndex = TreeViewAssetTreePopulator.FolderImageIndex;
            e.Node.SelectedImageIndex = TreeViewAssetTreePopulator.FolderImageIndex;
        }

        private void tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            tree.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right && e.Node.Tag is AssetTree.Node node) {
                contextOpen.Visible = node is AssetTree.AssetEntry;
                contextOpen.Enabled = node is AssetTree.AssetEntry entry && entry.Asset.Editable;

                bool modifiable = !IsReserved(node);
                contextRename.Enabled = modifiable;
                contextCut.Enabled = modifiable;
                contextCopy.Enabled = modifiable;
                contextDelete.Enabled = modifiable;

                contextMenu.Show(Cursor.Position);
            }
        }

        private bool IsReserved(AssetTree.Node node) {
            if (node is AssetTree.Folder folder) {
                return Asset.IsFolderReserved(folder.GetFilename("").Replace(Path.DirectorySeparatorChar, Asset.FolderSeparator));
            } else if (node is AssetTree.AssetEntry asset) {
                return Asset.IsAssetReserved(asset.Asset);
            }
            return false;
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
#if !DEBUG
                try {
#endif
                    mainWindow.OpenAsset(entry.Asset);
#if !DEBUG
                } catch (Exception ex) {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show($"Could not open file {entry.Asset.GetFilename("")}:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
#endif
            }
        }

        private void contextRename_Click(object sender, EventArgs e) {
            tree.LabelEdit = true;
            AssetTree.Node node = (AssetTree.Node)tree.SelectedNode.Tag;
            if (node is AssetTree.AssetEntry asset) {
                tree.SelectedNode.Text = asset.Asset.Parts.name;
            }
            tree.SelectedNode.BeginEdit();
        }

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            AssetTree.Node node = (AssetTree.Node)e.Node.Tag;
            e.CancelEdit = true;
            if (e.Label != null && e.Label.Length > 0) {
                try {
                    project.Assets.Rename(node, e.Label);
                } catch (Exception ex) {
                    e.CancelEdit = true;
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show($"Error renaming file: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            e.Node.Text = node.DisplayName;
            tree.LabelEdit = false;
        }

        private void contextCut_Click(object sender, EventArgs e) {
            project.Assets.Cut((AssetTree.Node)tree.SelectedNode.Tag);
        }

        private void contextCopy_Click(object sender, EventArgs e) {
            project.Assets.Copy((AssetTree.Node)tree.SelectedNode.Tag);
        }

        private void contextPaste_Click(object sender, EventArgs e) {
            try {
                project.Assets.Paste((AssetTree.Node)tree.SelectedNode.Tag);
            } catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show($"Error pasting: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contextDelete_Click(object sender, EventArgs e) {
            AssetTree.Node node = (AssetTree.Node)tree.SelectedNode.Tag;
            if (MessageBox.Show($"Are you sure you want to permanently delete {node.Name}? This cannot be undone.", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                try {
                    project.Assets.Delete(node);
                } catch (Exception ex) {
                    Console.WriteLine(ex.StackTrace);
                    MessageBox.Show($"Error deleting: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
