using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Necrofy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "SNES ROMs (*.sfc; *.smc)|*.sfc;*.smc|All Files (*.*)|*.*";
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
            ROMInfo info = new ROMInfo(fs);
            info.Freespace.Sort();
            Clipboard.SetText(info.Freespace.ToString());
            fs.Close();
        }
    }
}
