using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

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
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string projectPath = Path.Combine(Path.GetDirectoryName(ofd.FileName), "zamn_project");
            if (Directory.Exists(projectPath)) {
                Directory.Delete(projectPath, true);
            }
            Directory.CreateDirectory(projectPath);
            new Project(ofd.FileName, projectPath);
        }

        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string projectPath = Path.Combine(Path.GetDirectoryName(ofd.FileName), "zamn_project");
            new Project(projectPath).Build(ofd.FileName, ofd.FileName + ".new.sfc");
        }
    }
}
