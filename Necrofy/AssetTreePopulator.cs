using Necrofy.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    /// <summary>A class that can be used to display an asset tree in a control</summary>
    /// <typeparam name="NC">The type of the node collections</typeparam>
    /// <typeparam name="N">The type of the nodes</typeparam>
    abstract class AssetTreePopulator<NC, N> where N : class
    {
        public const int DocumentImageIndex = 0;
        public const int FolderImageIndex = 1;
        public const int FolderOpenImageIndex = 2;

        private static readonly Dictionary<AssetCategory, Bitmap> displayImages = new Dictionary<AssetCategory, Bitmap> {
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

        private readonly AssetTree tree;
        protected readonly NC root;

        private readonly ImageList treeImages = new ImageList();
        private readonly Dictionary<AssetCategory, int> displayImageIndexes = new Dictionary<AssetCategory, int>();

        public AssetTreePopulator(AssetTree tree, NC root) {
            this.tree = tree;
            this.root = root;
            treeImages.ColorDepth = ColorDepth.Depth32Bit;
            treeImages.Images.Add(Resources.document); // Used as default image
            treeImages.Images.Add(Resources.folder);
            treeImages.Images.Add(Resources.folder_open);
        }

        protected void Load() {
            SetImageList(treeImages);
            PopulateTree(root, tree.Root);
            tree.AssetChanged += AssetChanged;
            tree.AssetAdded += AssetAdded;
            tree.AssetRemoved += AssetRemoved;
            tree.NodeRenamed += NodeRenamed;
        }

        public void Dispose() {
            tree.AssetChanged -= AssetChanged;
            tree.AssetAdded -= AssetAdded;
            tree.AssetRemoved -= AssetRemoved;
            tree.NodeRenamed -= NodeRenamed;
            treeImages.Dispose();
        }

        private void PopulateTree(NC parent, AssetTree.Folder folder) {
            foreach (AssetTree.Folder subFolder in folder.Folders.OrderBy(n => n, AssetTree.NodeComparer.instance)) {
                PopulateFolder(parent, subFolder, true);
            }

            foreach (AssetTree.AssetEntry entry in folder.Assets.OrderBy(n => n, AssetTree.NodeComparer.instance)) {
                PopulateAsset(parent, entry, true);
            }
        }

        private void PopulateFolder(NC parent, AssetTree.Folder subFolder, bool atEnd) {
            N child = CreateChild(parent, subFolder, subFolder, true, FolderImageIndex, atEnd);

            PopulateTree(GetChildren(child), subFolder);
            if (IsEmpty(child)) {
                Remove(child);
            }
        }

        private void PopulateAsset(NC parent, AssetTree.AssetEntry entry, bool atEnd) {
            if (IncludeAsset(entry)) {
                N child = CreateChild(parent, entry, entry, false, GetImageIndex(entry.Asset.Category), atEnd);

                if (!entry.Asset.Editable) {
                    SetColor(child, SystemColors.GrayText);
                } else if (entry.Asset.Parts.skipped) {
                    SetColor(child, SystemColors.Highlight);
                }
            }
        }

        private N CreateChild(NC parent, AssetTree.Node assetTreeNode, object tag, bool isFolder, int imageIndex, bool atEnd) {
            int index;
            if (atEnd) {
                index = GetAssetTreeNodes(parent).Count;
            } else {
                index = GetInsertionPoint(parent, assetTreeNode);
            }
            N child = CreateChild(assetTreeNode.DisplayName, tag, isFolder, imageIndex);
            Insert(parent, child, index);
            return child;
        }

        private int GetInsertionPoint(NC parent, AssetTree.Node assetTreeNode) {
            List<AssetTree.Node> children = GetAssetTreeNodes(parent);

            int index;
            for (index = 0; index < children.Count; index++) {
                if (AssetTree.NodeComparer.instance.Compare(assetTreeNode, children[index]) < 0) {
                    break;
                }
            }
            return index;
        }

        private void AssetChanged(object sender, AssetEventArgs e) {
            N node = FindByTag(root, e.Asset);
            if (node != null) {
                SetText(node, e.Asset.Asset.DisplayName);
            }
        }

        private void AssetAdded(object sender, AssetEventArgs e) {
            if (IncludeAsset(e.Asset) && FindByTag(root, e.Asset) == null) {
                N parent = null;
                AssetTree.Node node = e.Asset;
                while (node.Parent.Parent != null) {
                    parent = FindByTag(root, node.Parent);
                    if (parent != null) {
                        break;
                    }
                    node = node.Parent;
                }
                NC collection = parent == null ? root : GetChildren(parent);
                if (node is AssetTree.Folder folder) {
                    PopulateFolder(collection, folder, false);
                } else if (node is AssetTree.AssetEntry asset) {
                    PopulateAsset(collection, asset, false);
                }
            }
        }

        private void AssetRemoved(object sender, AssetEventArgs e) {
            RemoveNode(FindByTag(root, e.Asset));
        }

        private void NodeRenamed(object sender, NodeEventArgs e) {
            N node = FindByTag(root, e.Node);
            if (node != null) {
                N selectedNode = SelectedNode;

                Remove(node);
                N parentNode = FindByTag(root, e.Node.Parent);
                NC collection = parentNode == null ? root : GetChildren(parentNode);
                Insert(collection, node, GetInsertionPoint(collection, e.Node));
                SetText(node, e.Node.DisplayName);

                SelectedNode = selectedNode;
            }
        }

        private void RemoveNode(N node) {
            if (node != null) {
                N parent = GetParent(node);
                Remove(node);
                if (parent != null && IsEmpty(parent)) {
                    RemoveNode(parent);
                }
            }
        }

        private int GetImageIndex(AssetCategory category) {
            if (!displayImages.ContainsKey(category)) {
                return DocumentImageIndex;
            }
            if (!displayImageIndexes.ContainsKey(category)) {
                displayImageIndexes[category] = treeImages.Images.Count;
                treeImages.Images.Add(displayImages[category]);
            }
            return displayImageIndexes[category];
        }

        protected abstract void SetImageList(ImageList imageList);
        protected abstract N CreateChild(string text, object tag, bool isFolder, int imageIndex);
        protected abstract List<AssetTree.Node> GetAssetTreeNodes(NC parent);
        protected abstract NC GetChildren(N node);
        protected abstract N GetParent(N node);
        protected abstract bool IsEmpty(N node);
        protected abstract void Remove(N node);
        protected abstract void Insert(NC parent, N node, int index);
        protected abstract void SetColor(N node, Color color);
        public abstract N FindByTag(NC collection, object tag);
        protected abstract void SetText(N node, string text);
        protected abstract N SelectedNode { get; set; }
        protected virtual bool IncludeAsset(AssetTree.AssetEntry entry) {
            return true;
        }
    }
}
