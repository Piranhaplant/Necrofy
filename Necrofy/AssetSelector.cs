﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.IO;

namespace Necrofy
{
    partial class AssetSelector : NoAutoScalePanel
    {
        public string SelectedItem => comboTree.SelectedNode?.GetFullPath(Asset.FolderSeparator.ToString(), false);
        public Asset.NameInfo SelectedNameInfo => (comboTree.SelectedNode?.Tag as AssetTree.AssetEntry)?.Asset;
        public event EventHandler SelectedItemChanged;

        private DropDownAssetTreePopulator populator;

        public AssetSelector() {
            InitializeComponent();
            Disposed += AssetSelector_Disposed;
        }

        private void AssetSelector_Disposed(object sender, EventArgs e) {
            populator?.Dispose();
        }

        private void comboTreeBox1_SelectedNodeChanged(object sender, EventArgs e) {
            SelectedItemChanged?.Invoke(this, e);
        }

        public void LoadProject(Project project, AssetCategory category, string startingAssetName, Func<AssetTree.AssetEntry, bool> filter = null) {
            populator = new DropDownAssetTreePopulator(project.Assets, comboTree, category, filter);

            if (!string.IsNullOrEmpty(startingAssetName)) {
                if (project.Assets.Root.FindAsset(startingAssetName, out AssetTree.AssetEntry entry)) {
                    ComboTreeNode assetNode = populator.FindByTag(comboTree.Nodes, entry);
                    if (assetNode != null) {
                        comboTree.SelectedNode = assetNode;
                        return;
                    }
                }
                if (project.Assets.Root.FindFolder(Path.GetDirectoryName(startingAssetName), out AssetTree.Folder folder)) {
                    ComboTreeNode folderNode = populator.FindByTag(comboTree.Nodes, folder);
                    if (folderNode != null && folderNode.Nodes.Count > 0) {
                        string assetFileName = Path.GetFileName(startingAssetName);
                        foreach (ComboTreeNode node in folderNode.Nodes) {
                            if (assetFileName.StartsWith(node.Text)) {
                                comboTree.SelectedNode = node;
                                return;
                            }
                        }
                        comboTree.SelectedNode = folderNode.Nodes[0];
                    }
                }
            }
        }

        public void Deselect() {
            comboTree.SelectedNode = null;
        }

        private class DropDownAssetTreePopulator : AssetTreePopulator<ComboTreeNodeCollection, ComboTreeNode> {
            private readonly AssetCategory category;
            private readonly ComboTreeBox comboBox;
            private readonly Func<AssetTree.AssetEntry, bool> filter;

            public DropDownAssetTreePopulator(AssetTree tree, ComboTreeBox comboBox, AssetCategory category, Func<AssetTree.AssetEntry, bool> filter) : base(tree, comboBox.Nodes) {
                this.category = category;
                this.comboBox = comboBox;
                this.filter = filter ?? (a => true);
                Load();
            }

            protected override void SetImageList(ImageList imageList) => comboBox.Images = imageList;

            protected override ComboTreeNode CreateNode(string text, object tag, bool isFolder, int imageIndex) {
                ComboTreeNode node = new ComboTreeNode(text);
                node.Name = text;
                node.Tag = tag;
                node.Selectable = !isFolder;
                node.ImageIndex = imageIndex;
                if (isFolder) {
                    node.ExpandedImageIndex = imageIndex + 1;
                }
                return node;
            }

            protected override List<AssetTree.Node> GetAssetTreeNodes(ComboTreeNodeCollection parent) {
                return parent.Select(n => n.Tag as AssetTree.Node).ToList();
            }

            public override ComboTreeNode FindByTag(ComboTreeNodeCollection collection, object tag) {
                foreach (ComboTreeNode node in collection) {
                    if (node.Tag == tag) {
                        return node;
                    }
                    ComboTreeNode child = FindByTag(node.Nodes, tag);
                    if (child != null) {
                        return child;
                    }
                }
                return null;
            }

            protected override ComboTreeNodeCollection GetChildren(ComboTreeNode node) => node.Nodes;
            protected override ComboTreeNode GetParent(ComboTreeNode node) => node.Parent;
            protected override bool IsEmpty(ComboTreeNode node) => node.Nodes.Count == 0;
            protected override void Remove(ComboTreeNode node) {
                if (node.Parent == null) {
                    root.Remove(node);
                } else {
                    node.Parent.Nodes.Remove(node);
                }
            }
            protected override void Insert(ComboTreeNodeCollection parent, ComboTreeNode node, int index) => parent.Insert(index, node);
            protected override void SetColor(ComboTreeNode node, Color color) => node.ForeColor = color;
            protected override void SetText(ComboTreeNode node, string text) {
                node.Text = text;
                node.Name = text;
            }
            protected override ComboTreeNode SelectedNode { get => comboBox.SelectedNode; set => comboBox.SelectedNode = value; }
            protected override bool IncludeAsset(AssetTree.AssetEntry entry) => entry.Asset.Category == category && filter(entry);
        }
    }
}
