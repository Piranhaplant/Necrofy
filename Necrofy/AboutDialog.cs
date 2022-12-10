using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    public partial class AboutDialog : Form
    {
        public AboutDialog() {
            InitializeComponent();

            title.Text = MainWindow.DefaultStatusText;
            Size = new Size(Width, Height - tabControl1.Height);
        }

        private void librariesLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Size = new Size(Width, Height + tabControl1.Height);
            librariesLink.Enabled = false;
        }
    }
}
