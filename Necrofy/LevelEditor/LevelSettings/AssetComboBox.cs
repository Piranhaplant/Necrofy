using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class AssetComboBox : ComboBox
    {
        private readonly List<Item> items = new List<Item>();
        public new IEnumerable<Asset.NameInfo> Items => items.Select(i => i.asset);

        public new Asset.NameInfo SelectedItem {
            get {
                if (base.SelectedItem == null) {
                    return null;
                }
                return ((Item)base.SelectedItem).asset;
            }
            set {
                base.SelectedItem = items.Where(i => i.asset == value).FirstOrDefault();
            }
        }

        public string SelectedName {
            get {
                if (base.SelectedItem == null) {
                    return null;
                }
                return ((Item)base.SelectedItem).asset.Name;
            }
            set {
                base.SelectedItem = items.Where(i => i.asset.Name == value).FirstOrDefault();
            }
        }

        public void Add(IEnumerable<Asset.NameInfo> assets, string defaultName, bool showTilesetName = true) {
            Item[] newItems = assets.Select(a => new Item(a, defaultName, showTilesetName)).ToArray();
            items.AddRange(newItems);
            base.Items.AddRange(newItems);
        }

        public void Clear() {
            items.Clear();
            base.Items.Clear();
        }

        public string GetAnyName() {
            return items.Select(i => i.asset.Name).FirstOrDefault() ?? "";
        }

        private class Item
        {
            public readonly Asset.NameInfo asset;
            private readonly string s;

            public Item(Asset.NameInfo asset, string defaultName, bool showTilesetName) {
                this.asset = asset;
                if (asset.ParsedName.Tileset != null) {
                    if (asset.ParsedName.FinalName == defaultName) {
                        s = asset.ParsedName.Tileset;
                    } else if (showTilesetName) {
                        s = asset.ParsedName.Tileset + Asset.FolderSeparator + asset.ParsedName.FinalName;
                    } else {
                        s = asset.ParsedName.FinalName;
                    }
                } else {
                    s = asset.ParsedName.FinalName;
                }
            }

            public override string ToString() {
                return s;
            }
        }
    }
}
