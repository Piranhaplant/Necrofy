using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class ToolBarMenuLinker
    {
        public static void Link(ToolStripItem toolBarItem, ToolStripMenuItem menuItem) {
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
