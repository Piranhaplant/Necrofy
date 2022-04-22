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

            protected override TreeNode CreateChild(TreeNodeCollection parent, string text, object tag, bool isFolder, int imageIndex) {
                TreeNode node = parent.Add(text);
                node.Name = text;
                node.Tag = tag;
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
                return node;
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
            protected override void SetColor(TreeNode node, Color color) => node.ForeColor = color;
            protected override void SetText(TreeNode node, string text) {
                node.Text = text;
                node.Name = text;
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
            e.Node.ImageIndex += 1;
            e.Node.SelectedImageIndex += 1;
        }

        private void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            e.Node.ImageIndex -= 1;
            e.Node.SelectedImageIndex -= 1;
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
    }
}
