using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    public partial class ObjectBrowserForm : DockContent
    {
        public ObjectBrowserControl Browser {
            get {
                return objectBrowser;
            }
        }

        public ObjectBrowserForm() {
            InitializeComponent();
        }
    }
}
