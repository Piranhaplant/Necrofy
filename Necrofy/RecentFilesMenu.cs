using System;
using System.Collections.Generic;
using System.IO;
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
        public IEnumerable<string> Files {
            get {
                return _files.AsReadOnly();
            }
            set {
                _files = new List<string>(value);
                TrimFiles();
                CreateItems();
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
                    TrimFiles();
                    CreateItems();
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
                    CreateItems();
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
                CreateItems();
            }
        }

        public delegate void FileClickedDelegate(string file);
        public event FileClickedDelegate FileClicked;

        private List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

        private void CreateItems() {
            foreach (ToolStripMenuItem item in items) {
                Parent.Items.Remove(item);
            }
            items.Clear();

            for (int i = 0; i < _files.Count; i++) {
                ToolStripMenuItem item;
                if (i == 0) {
                    item = this;
                } else {
                    item = new ToolStripMenuItem();
                    Parent.Items.Insert(Parent.Items.IndexOf(this) + i, item);
                    items.Add(item);
                }
                item.Text = (i + 1).ToString() + " " + TrimFile(_files[i]);
                item.ShortcutKeys = GetShortcutKeys(i);
                int iClosure = i;
                item.Click += (sender, e) => Item_Click(sender, iClosure);
            }

            if (!DesignMode) {
                Visible = _files.Count > 0;
                if (_separator != null) {
                    _separator.Visible = Visible;
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
            CreateItems();
        }

        private void Item_Click(object sender, int i) {
            FileClicked?.Invoke(_files[i]);
        }

        private string TrimFile(string file) {
            if (file.Length > _maxLength) {
                string start = file.Substring(0, file.IndexOf(Path.DirectorySeparatorChar) + 1);
                string end = file.Substring(file.Length - (_maxLength - start.Length));
                if (end.Contains(Path.DirectorySeparatorChar)) {
                    end = end.Substring(end.IndexOf(Path.DirectorySeparatorChar));
                }
                return start + "..." + end;
            }
            return file;
        }

        private void TrimFiles() {
            if (_files.Count > _maxItems) {
                _files.RemoveRange(_maxItems, _files.Count - _maxItems);
            }
        }

        private Keys GetShortcutKeys(int i) {
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
