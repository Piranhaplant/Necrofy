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
        
        protected MainWindow mainWindow;

        public EditorWindow() {
            DockAreas = DockAreas.Document;
            HideOnClose = false;
        }

        public void Display(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
            Displayed();
        }

        protected virtual void Displayed() { }
    }
}
