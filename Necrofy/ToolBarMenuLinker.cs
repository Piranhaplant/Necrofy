using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    [ProvideProperty("LinkedToolBarItem", typeof(ToolStripMenuItem))]
    public partial class ToolBarMenuLinker : Component, IExtenderProvider
    {
        private readonly Dictionary<ToolStripMenuItem, ToolStripItem> links = new Dictionary<ToolStripMenuItem, ToolStripItem>();

        public ToolBarMenuLinker() {
            InitializeComponent();
        }

        public ToolBarMenuLinker(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        public bool CanExtend(object extendee) {
            return extendee is ToolStripMenuItem;
        }

        [DefaultValue(null)]
        public ToolStripItem GetLinkedToolBarItem(ToolStripMenuItem item) {
            if (links.TryGetValue(item, out ToolStripItem linkedItem)) {
                return linkedItem;
            }
            return null;
        }

        public void SetLinkedToolBarItem(ToolStripMenuItem item, ToolStripItem linkedItem) {
            links[item] = linkedItem;
            if (!DesignMode) {
                Link(linkedItem, item);
            }
        }

        private static void Link(ToolStripItem toolBarItem, ToolStripMenuItem menuItem) {
            Link(() => toolBarItem.Enabled, v => toolBarItem.Enabled = v, a => toolBarItem.EnabledChanged += a,
                 () => menuItem.Enabled, v => menuItem.Enabled = v, a => menuItem.EnabledChanged += a);
            if (toolBarItem is ToolStripButton button) {
                Link(() => button.Checked, v => button.Checked = v, a => button.CheckedChanged += a,
                     () => menuItem.Checked, v => menuItem.Checked = v, a => menuItem.CheckedChanged += a);
            } else if (toolBarItem is CheckableToolStripSplitButton checkable) {
                Link(() => checkable.Checked, v => checkable.Checked = v, a => checkable.CheckedChanged += a,
                     () => menuItem.Checked, v => menuItem.Checked = v, a => menuItem.CheckedChanged += a);
            }
        }

        private static void Link<T>(Func<T> getter1, Action<T> setter1, Action<EventHandler> event1, Func<T> getter2, Action<T> setter2, Action<EventHandler> event2) {
            bool updating = false;
            event1((sender, e) => {
                if (!updating) {
                    updating = true;
                    setter2(getter1());
                    updating = false;
                }
            });
            event2((sender, e) => {
                if (!updating) {
                    updating = true;
                    setter1(getter2());
                    updating = false;
                }
            });
        }
    }
}
