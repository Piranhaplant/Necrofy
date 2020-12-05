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
    public partial class TextDisplayForm : Form
    {
        public TextDisplayForm(string title, string text) {
            InitializeComponent();
            Text = title;
            textBox1.Text = text;
        }

        private void TextDisplayForm_Shown(object sender, EventArgs e) {
            textBox1.DeselectAll();
        }
    }
}
