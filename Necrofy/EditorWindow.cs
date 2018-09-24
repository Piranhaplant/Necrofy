using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    class EditorWindow : DockContent
    {
        public MenuStrip EditorMenuStrip { get; set; }
        public ToolStrip EditorToolStrip { get; set; }

        public EditorWindow() {
            this.DockAreas = DockAreas.Document;
        }
    }
}
