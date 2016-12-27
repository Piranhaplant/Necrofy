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
            new Project(ofd.FileName, Path.GetDirectoryName(ofd.FileName), true);
        }

        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            Level l = JsonConvert.DeserializeObject<Level>(File.ReadAllText(ofd.FileName), new LevelJsonConverter());
            foreach (LevelMonster m in l.levelMonsters) {
                Console.Out.WriteLine(m.type.ToString());
            }
        }
    }
}
