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
    partial class PasswordEditor : EditorWindow
    {
        private const string AllowedCharacters = "0123456789BCDFGHJKLMNPQRSTVWXYZ! ";
        private readonly string AllowedLetters = new string(AllowedCharacters.Where(c => char.IsLetter(c)).ToArray());

        public override ToolStripGrouper.ItemSet ToolStripItemSet => ToolStripGrouper.ItemSet.Passwords;

        private readonly PasswordsAsset asset;
        private readonly PasswordData data;
        private new UndoManager<PasswordEditor> undoManager;
        
        private string cellPreviousValue;

        public PasswordEditor(PasswordsAsset asset) {
            InitializeComponent();

            this.asset = asset;
            data = asset.data;
            Title = "Passwords";
            Status = "Characters usable in passwords: " + AllowedCharacters;

            dataGrid.DefaultCellStyle.Font = new Font(FontFamily.GenericMonospace, 10);

            dataGrid.ColumnCount = data.normalPasswords.GetWidth();
            for (int i = 0; i < dataGrid.Columns.Count; i++) {
                dataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGrid.Columns[i].HeaderText = $"{i + 1} Victim" + (i > 0 ? "s" : "");
            }

            DataGridViewRow level0Row = new DataGridViewRow();
            for (int i = 0; i < 9; i++) {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                level0Row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = SystemColors.ControlDark;
            }
            level0Row.Cells.Add(new DataGridViewTextBoxCell() { Value = data.level0Password });
            dataGrid.Rows.Add(level0Row);

            for (int levelIndex = 0; levelIndex < data.normalPasswords.GetHeight(); levelIndex++) {
                DataGridViewRow row = new DataGridViewRow();
                for (int victims = 0; victims < data.normalPasswords.GetWidth(); victims++) {
                    row.Cells.Add(new DataGridViewTextBoxCell() { Value = data.normalPasswords[victims, levelIndex] });
                }
                dataGrid.Rows.Add(row);
            }

            GenerateRowHeaders();
        }
        
        protected override UndoManager Setup() {
            undoManager = new UndoManager<PasswordEditor>(mainWindow.UndoButton, mainWindow.RedoButton, this);
            return undoManager;
        }

        public override void ToolStripItemClicked(ToolStripGrouper.ItemType item) {
            base.ToolStripItemClicked(item);
            switch (item) {
                case ToolStripGrouper.ItemType.GeneratePasswordsDefault:
                    undoManager.Do(new GeneratePasswordsDefaultStyleAction(AllowedLetters));
                    break;
                case ToolStripGrouper.ItemType.GeneratePasswordsLetters:
                    undoManager.Do(new GeneratePasswordsAnyCharAction(AllowedLetters));
                    break;
                case ToolStripGrouper.ItemType.GeneratePasswordsAllChars:
                    undoManager.Do(new GeneratePasswordsAnyCharAction(AllowedCharacters.Substring(0, AllowedCharacters.Length - 1)));
                    break;
                case ToolStripGrouper.ItemType.AddPasswordRow:
                    if (dataGrid.RowCount < 400) { // Add row cap so it is always possible to generate enough unique passwords
                        undoManager.Do(new AddPasswordRowAction(dataGrid.RowCount));
                    }
                    break;
            }
        }

        public void CancelCellEdit() {
            cellPreviousValue = null;
            dataGrid.CancelEdit();
            dataGrid.EndEdit();
        }

        protected override void DoSave(Project project) {
            data.normalPasswords = new string[dataGrid.ColumnCount, dataGrid.RowCount - 1];
            for (int levelIndex = 0; levelIndex < data.normalPasswords.GetLength(1); levelIndex++) {
                for (int victims = 0; victims < data.normalPasswords.GetLength(0); victims++) {
                    data.normalPasswords[victims, levelIndex] = (string)dataGrid.Rows[levelIndex + 1].Cells[victims].Value;
                }
            }
            data.level0Password = (string)dataGrid.Rows[0].Cells[9].Value;
            asset.Save(project);
        }

        public override void Undo() {
            if (dataGrid.IsCurrentCellInEditMode) {
                CancelCellEdit();
            } else {
                base.Undo();
            }
        }

        public void SetCell(int rowIndex, int columnIndex, string value) {
            dataGrid.Rows[rowIndex].Cells[columnIndex].Value = value;
        }

        public string[][] GetAllCells() {
            string[][] cells = new string[dataGrid.RowCount][];
            for (int r = 0; r < cells.Length; r++) {
                cells[r] = new string[dataGrid.Rows[r].Cells.Count];
                for (int c = 0; c < cells[r].Length; c++) {
                    cells[r][c] = (string)dataGrid.Rows[r].Cells[c].Value;
                }
            }
            return cells;
        }

        public void AddRow(int index) {
            string fillChar = AllowedLetters[0].ToString();
            string fillString = fillChar + fillChar + fillChar + fillChar;

            DataGridViewRow row = new DataGridViewRow();
            for (int i = 0; i < dataGrid.ColumnCount; i++) {
                row.Cells.Add(new DataGridViewTextBoxCell() { Value = fillString });
            }
            dataGrid.Rows.Insert(index, row);
        }

        public void RemoveRow(int index) {
            dataGrid.Rows.RemoveAt(index);
        }

        private void GenerateRowHeaders(int startIndex = 0) {
            if (startIndex == 0) {
                dataGrid.Rows[0].HeaderCell.Value = "Level 0";
                startIndex = 1;
            }
            for (int i = startIndex; i < dataGrid.Rows.Count; i++) {
                dataGrid.Rows[i].HeaderCell.Value = $"Level {i * 4 + 1}";
            }
        }

        private readonly HashSet<Control> hookedControls = new HashSet<Control>();

        private void dataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            if (e.Control is DataGridViewTextBoxEditingControl textBox && !hookedControls.Contains(e.Control)) {
                hookedControls.Add(e.Control);
                textBox.KeyDown += TextBox_KeyDown;
                textBox.KeyPress += Control_KeyPress;
                textBox.ShortcutsEnabled = false;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                e.SuppressKeyPress = true;
            }
        }

        private void Control_KeyPress(object sender, KeyPressEventArgs e) {
            if (sender is DataGridViewTextBoxEditingControl textBox) {
                char upper = char.ToUpperInvariant(e.KeyChar);
                if (AllowedCharacters.Contains(upper) && textBox.SelectionStart < textBox.Text.Length) {
                    int prevSelectionStart = textBox.SelectionStart;
                    StringBuilder newString = new StringBuilder(textBox.Text);
                    newString[prevSelectionStart] = upper;
                    textBox.Text = newString.ToString();
                    textBox.SelectionStart = prevSelectionStart + 1;
                } else if (e.KeyChar == (char)Keys.Back) {
                    textBox.SelectionStart = Math.Max(0, textBox.SelectionStart - 1);
                }
                e.Handled = true;
            }
        }

        private void dataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            GenerateRowHeaders(e.RowIndex);
        }

        private void dataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            GenerateRowHeaders(e.RowIndex);
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e) {
            dataGrid.AllowUserToDeleteRows = !dataGrid.SelectedRows.Contains(dataGrid.Rows[0]);
        }

        private void dataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
            cellPreviousValue = (string)dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && cellPreviousValue != null) {
                string newValue = (string)dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (newValue != cellPreviousValue) {
                    undoManager.Do(new ChangePasswordAction(e.RowIndex, e.ColumnIndex, cellPreviousValue, newValue));
                }
            }
            cellPreviousValue = null;
        }

        private void dataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            e.Cancel = true;
            undoManager.Do(new RemovePasswordRowAction(e.Row.Index));
        }
    }
}
