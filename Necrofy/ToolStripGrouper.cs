﻿using System;
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
        private readonly Dictionary<ToolStripMenuItem, Keys> shortcutKeys = new Dictionary<ToolStripMenuItem, Keys>();

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
            foreach (ItemSet s in set.GetFlags()) {
                if (!itemsForSet.ContainsKey(s)) {
                    itemsForSet[s] = new HashSet<ToolStripItem>();
                }
                itemsForSet[s].Add(item);
            }
        }

        private IEnumerable<ToolStripItem> GetItemsInSet(ItemSet set) {
            if (set != ItemSet.None && itemsForSet.TryGetValue(set, out HashSet<ToolStripItem> items)) {
                return items;
            }
            return Enumerable.Empty<ToolStripItem>();
        }
        
        public void HideAllSetItems() {
            if (shortcutKeys.Count == 0) {
                foreach (ToolStripItem item in setOfItem.Keys) {
                    if (item is ToolStripMenuItem toolStripItem) {
                        shortcutKeys[toolStripItem] = toolStripItem.ShortcutKeys;
                    }
                }
            }
            foreach (ToolStripItem item in setOfItem.Keys) {
                item.Visible = false;
                if (item is ToolStripMenuItem toolStripItem) {
                    toolStripItem.ShortcutKeys = Keys.None;
                }
            }
        }

        public void ShowItemSet(ItemSet set) {
            foreach (ToolStripItem item in GetItemsInSet(set)) {
                item.Visible = true;
                if (item is ToolStripMenuItem toolStripItem && shortcutKeys.TryGetValue(toolStripItem, out Keys keys)) {
                    toolStripItem.ShortcutKeys = keys;
                }
            }
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
            } else if (item is ToolStripButton button) {
                return new ButtonItemProxy(button, clickEventHandler, checkedChangedEventHandler);
            } else {
                return new BaseItemProxy(item, clickEventHandler);
            }
        }

        public abstract class ItemProxy
        {
            public abstract bool Checked { get; set; }
            public abstract bool Enabled { get; set; }
            public abstract string Text { get; set; }
        }

        private class BaseItemProxy : ItemProxy
        {
            private readonly ToolStripItem item;

            public override bool Checked { get => false; set { } }
            public override bool Enabled { get => item.Enabled; set => item.Enabled = value; }
            public override string Text { get => item.Text; set => item.Text = value; }

            public BaseItemProxy(ToolStripItem item, EventHandler clickEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
            }
        }

        private class MenuItemProxy : ItemProxy
        {
            private readonly ToolStripMenuItem item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }
            public override bool Enabled { get => item.Enabled; set => item.Enabled = value; }
            public override string Text { get => item.Text; set => item.Text = value; }

            public MenuItemProxy(ToolStripMenuItem item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        private class ButtonItemProxy : ItemProxy
        {
            private readonly ToolStripButton item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }
            public override bool Enabled { get => item.Enabled; set => item.Enabled = value; }
            public override string Text { get => item.Text; set => item.Text = value; }

            public ButtonItemProxy(ToolStripButton item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        private class CheckableSplitButtonItemProxy : ItemProxy
        {
            private readonly CheckableToolStripSplitButton item;

            public override bool Checked { get => item.Checked; set => item.Checked = value; }
            public override bool Enabled { get => item.Enabled; set => item.Enabled = value; }
            public override string Text { get => item.Text; set => item.Text = value; }

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
            public override bool Enabled { get => item.Enabled; set => item.Enabled = value; }
            public override string Text { get => item.Text; set => item.Text = value; }

            public SeparateCheckMenuItemProxy(SeparateCheckToolStripMenuItem item, EventHandler clickEventHandler, EventHandler checkedChangedEventHandler) {
                this.item = item;
                item.Click += clickEventHandler;
                item.CheckedChanged += checkedChangedEventHandler;
            }
        }

        public enum ItemType
        {
            None,

            EditMoveSelection,
            ViewGrid,
            ViewSolidTilesOnly,
            ViewTilePriority,
            ViewRespawnAreas,
            ViewScreenSizeGuide,
            ViewAnimate,
            ViewNextFrame,
            ViewRestartAnimation,
            ViewAxes,
            ViewTileBorders,
            ViewSpriteGrid,
            ViewDecreaseWidth,
            WidthLabel,
            ViewIncreaseWidth,
            ViewLargeTileMode,
            ViewTransparency,
            ViewGraphicsGrid,
            ViewTilemapGrid,
            LevelEditTitle,
            LevelSettings,
            LevelClear,
            LevelSaveAsImage,
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

            SpriteSaveAsImage,
            TilemapSaveAsImage,

            FlipHorizontally,
            FlipVertically,
            CenterHorizontally,
            CenterVertically,
            MoveUp,
            MoveDown,
            MoveToFront,
            MoveToBack,

            GeneratePasswordsDefault,
            GeneratePasswordsLetters,
            GeneratePasswordsAllChars,
            AddPasswordRow,

            GraphicsPaintbrush,
            GraphicsRectangleSelect,
            GraphicsBucketFill,
            GraphicsSelectByColor,

            TilemapPaintBrush,
            TilemapRectangleSelect,
            TilemapPencilSelect,
            TilemapSelectByTile,
            TilemapSelectByProperties,

            ViewHintingNone,
            ViewHintingLevelTitle,
            ViewHintingTileset,
        }

        [Flags]
        public enum ItemSet
        {
            None = 0,
            LevelEditor = 1,
            LevelTitle = 2,
            SpriteEditor = 4,
            Passwords = 8,
            Graphics = 16,
            Tilemap = 32,
        }
    }
}
