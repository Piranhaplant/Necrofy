using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class HexEditor : EditorWindow
    {
        private WpfHexaEditor.HexEditor hexEditorControl;

        public HexEditor(Project project) {
            InitializeComponent();

            string romPath = Path.Combine(project.path, Project.baseROMFilename);
            ROMInfo romInfo;
            using (NStream s = new NStream(new FileStream(romPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))) {
                romInfo = new ROMInfo(s);
                Project.AddEndOfBankFreespace(s, romInfo.Freespace);
            }

            hexEditorControl = new WpfHexaEditor.HexEditor();
            elementHost1.Child = hexEditorControl;

            hexEditorControl.FileName = romPath;
            hexEditorControl.ReadOnlyMode = true;
            hexEditorControl.ShowByteToolTip = true;

            foreach (Freespace.Block block in romInfo.Freespace.GetBlocks()) {
                hexEditorControl.CustomBackgroundBlockItems.Add(new WpfHexaEditor.Core.CustomBackgroundBlock(block.Start, block.Size, System.Windows.Media.Brushes.Aqua, "Freespace"));
            }
        }

        private void HexEditor_FormClosed(object sender, FormClosedEventArgs e) {
            hexEditorControl.CloseProvider();
        }
    }
}
