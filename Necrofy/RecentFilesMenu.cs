using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Necrofy
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class RecentFilesMenu : ToolStripMenuItem
    {
        private List<string> _files = new List<string>();
        public IEnumerable<String> Files {
            get {
                return _files.AsReadOnly();
            }
            set {
                _files = new List<string>(value);
                createItems();
            }
        }
        private int _maxItems = 10;
        public int MaxItems {
            get {
                return _maxItems;
            }
            set {
                if (value >= 1 && value != _maxItems) {
                    _maxItems = value;
                    createItems();
                }
            }
        }
        private int _maxLength = 60;
        public int MaxLength {
            get {
                return _maxLength;
            }
            set {
                if (value >= 1 && value != _maxLength) {
                    _maxLength = value;
                    createItems();
                }
            }
        }
        private ToolStripSeparator _separator;
        public ToolStripSeparator Separator {
            get {
                return _separator;
            }
            set {
                _separator = value;
                createItems();
            }
        }

        public delegate void FileClickedDelegate(string file);
        public event FileClickedDelegate FileClicked;

        private List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

        private void createItems() {
            foreach (ToolStripMenuItem item in items) {
                this.Parent.Items.Remove(item);
            }
            items.Clear();

            for (int i = 0; i < _files.Count; i++) {
                ToolStripMenuItem item;
                if (i == 0) {
                    item = this;
                } else {
                    item = new ToolStripMenuItem();
                    this.Parent.Items.Insert(this.Parent.Items.IndexOf(this) + i, item);
                    items.Add(item);
                }
                item.Text = (i + 1).ToString() + " " + trimFile(_files[i]);
                item.ShortcutKeys = getShortcutKeys(i);
                int iClosure = i;
                item.Click += new EventHandler((sender, e) => item_Click(sender, iClosure));
            }

            if (!DesignMode) {
                this.Visible = _files.Count > 0;
                if (_separator != null) {
                    _separator.Visible = _files.Count > 0;
                }
            }
        }

        public void Add(string file) {
            if (_files.Contains(file)) {
                _files.Remove(file);
            }
            _files.Insert(0, file);
            if (_files.Count > _maxItems) {
                _files.RemoveAt(_files.Count - 1);
            }
            createItems();
        }

        void item_Click(object sender, int i) {
            if (FileClicked != null) {
                FileClicked(_files[i]);
            }
        }

        private string trimFile(string file) {
            // TODO
            return file;
        }

        private Keys getShortcutKeys(int i) {
            Keys k = Keys.Control | Keys.Shift;
            switch (i) {
                case 0:
                    return k | Keys.D1;
                case 1:
                    return k | Keys.D2;
                case 2:
                    return k | Keys.D3;
                case 3:
                    return k | Keys.D4;
                case 4:
                    return k | Keys.D5;
                case 5:
                    return k | Keys.D6;
                case 6:
                    return k | Keys.D7;
                case 7:
                    return k | Keys.D8;
                case 8:
                    return k | Keys.D9;
                case 9:
                    return k | Keys.D0;
                default:
                    return Keys.None;
            }
        }
    }
}
