using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    [ProvideProperty("ItemType", typeof(ToolStripItem))]
    [ProvideProperty("ItemSet", typeof(ToolStripItem))]
    public partial class ToolStripGrouper : Component, IExtenderProvider
    {
        private readonly Dictionary<ItemSet, HashSet<ToolStripItem>> itemsForSet = new Dictionary<ItemSet, HashSet<ToolStripItem>>();
        private readonly Dictionary<ToolStripItem, ItemSet> setOfItem = new Dictionary<ToolStripItem, ItemSet>();
        private readonly Dictionary<ItemType, HashSet<ToolStripItem>> itemsForType = new Dictionary<ItemType, HashSet<ToolStripItem>>();
        private readonly Dictionary<ToolStripItem, ItemType> typeOfItem = new Dictionary<ToolStripItem, ItemType>();
        private readonly Dictionary<ItemType, ItemProxy> itemProxies = new Dictionary<ItemType, ItemProxy>();

        public event EventHandler<ItemEventArgs> ItemClick;
        public event EventHandler<ItemEventArgs> ItemCheckedChanged;

        public ToolStripGrouper() {
            InitializeComponent();
        }

        public ToolStripGrouper(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        public bool CanExtend(object extendee) {
            return extendee is ToolStripItem;
        }

        [DefaultValue(ItemType.None)]
        public ItemType GetItemType(ToolStripItem item) {
            if (typeOfItem.TryGetValue(item, out ItemType type)) {
                return type;
            }
            return ItemType.None;
        }

        public void SetItemType(ToolStripItem item, ItemType type) {
            if (!DesignMode && !typeOfItem.ContainsKey(item)) {
                itemProxies[type] = GetItemProxy(item, Item_Click, Item_CheckedChanged);
            }
            typeOfItem[item] = type;
            if (!itemsForType.ContainsKey(type)) {
                itemsForType[type] = new HashSet<ToolStripItem>();
            }
            itemsForType[type].Add(item);
        }

        [DefaultValue(ItemSet.None)]
        public ItemSet GetItemSet(ToolStripItem item) {
            if (setOfItem.TryGetValue(item, out ItemSet set)) {
                return set;
            }
            return ItemSet.None;
        }

        public void SetItemSet(ToolStripItem item, ItemSet set) {
            setOfItem[item] = set;
            if (!itemsForSet.ContainsKey(set)) {
                itemsForSet[set] = new HashSet<ToolStripItem>();
            }
            itemsForSet[set].Add(item);
        }

        public IEnumerable<ToolStripItem> GetItemsInSet(ItemSet set) {
            if (set != ItemSet.None && itemsForSet.TryGetValue(set, out HashSet<ToolStripItem> items)) {
                return items;
            }
            return Enumerable.Empty<ToolStripItem>();
        }

        public IEnumerable<ToolStripItem> GetAllSetItems() {
            return setOfItem.Keys;
        }

        public ItemProxy GetItem(ItemType type) {
            if (itemProxies.TryGetValue(type, out ItemProxy item)) {
                return item;
            }
            throw new Exception($"No item registered for type '{type}'");
        }

        private void Item_Click(object sender, EventArgs e) {
            ItemType type = GetSenderItemType(sender);
            if (type != ItemType.None) {
                ItemClick?.Invoke(this, new ItemEventArgs(type));
            }
        }

        private void Item_CheckedChanged(object sender, EventArgs e) {
            ItemType type = GetSenderItemType(sender);
            if (type != ItemType.None) {
                ItemCheckedChanged?.Invoke(this, new ItemEventArgs(type));
            }
        }

        private ItemType GetSenderItemType(object sender) {
            if (sender is ToolStripItem item) {
                if (typeOfItem.TryGetValue(item, out ItemType type)) {
                    return type;
                }
            }
            return ItemType.None;
        }

        public class ItemEventArgs : EventArgs
        {
            public ItemType Type { get; private set; }

            public ItemEventArgs(ItemType type) {
                Type = type;
            }
        }

        private ItemProxy GetItemProxy(ToolStripItem item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
            if (item is SeparateCheckToolStripMenuItem separateCheckItem) {
                return new SeparateCheckMenuItemProxy(separateCheckItem, clickEventHandler, checkedChangedEventHandler);
            } else if (item is CheckableToolStripSplitButton checkableSplitButton) {
                return new CheckableSplitButtonItemProxy(checkableSplitButton, clickEventHandler, checkedChangedEventHandler);
            } else if (item is ToolStripMenuItem menuItem) {
                return new MenuItemProxy(menuItem, clickEventHandler, checkedChangedEventHandler);
            } else {
                return new BaseItemProxy(item, clickEventHandler);
            }
        }

        public abstract class ItemProxy
        {
            public abstract bool Checked { get; set; }
        }

        private class BaseItemProxy : ItemProxy
        {
            public override bool Checked { get => false; set { } }

            public BaseItemProxy(ToolStripItem item, EventHandler clickEventHandler) {
                item.Click += clickEventHandler;
            }
        }

        private class MenuItemProxy : ItemProxy
        {
            private readonly ToolStripMenuItem item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }

            public MenuItemProxy(ToolStripMenuItem item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        private class CheckableSplitButtonItemProxy : ItemProxy
        {
            private readonly CheckableToolStripSplitButton item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }

            public CheckableSplitButtonItemProxy(CheckableToolStripSplitButton item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.ButtonClick += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        private class SeparateCheckMenuItemProxy : ItemProxy
        {
            private readonly SeparateCheckToolStripMenuItem item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }

            public SeparateCheckMenuItemProxy(SeparateCheckToolStripMenuItem item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        public enum ItemType
        {
            None,
            LevelEditTitle,
            LevelSettings,
            PaintbrushTool,
            TileSuggestTool,
            RectangleSelectTool,
            PencilSelectTool,
            TileSelectTool,
            ResizeLevelTool,
            SpriteTool,
            SpritesItems,
            SpritesVictims,
            SpritesOneShotMonsters,
            SpritesMonsters,
            SpritesBossMonsters,
            SpritesPlayers,
            SpritesAll,
        }

        public enum ItemSet
        {
            None,
            LevelEditor,
        }
    }
}
