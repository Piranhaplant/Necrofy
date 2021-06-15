using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class PasswordEditorAction : UndoAction<PasswordEditor>
    {
        protected override void BeforeAction() {
            editor.CancelCellEdit();
        }
    }

    class ChangePasswordAction : PasswordEditorAction
    {
        private readonly int rowIndex;
        private readonly int columnIndex;
        private readonly string oldValue;
        private readonly string newValue;

        public ChangePasswordAction(int rowIndex, int columnIndex, string oldValue, string newValue) {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.oldValue = oldValue;
            this.newValue = newValue;
            if (newValue == oldValue) {
                cancel = true;
            }
        }

        protected override void Redo() {
            editor.SetCell(rowIndex, columnIndex, newValue);
        }

        protected override void Undo() {
            editor.SetCell(rowIndex, columnIndex, oldValue);
        }

        public override string ToString() {
            return "Change password";
        }
    }

    abstract class GeneratePasswordsAction : PasswordEditorAction
    {
        private readonly string availableChars;
        private string[][] prevValues;
        private string[][] newValues;

        public GeneratePasswordsAction(string availableChars) {
            this.availableChars = availableChars;
        }

        public override void SetEditor(PasswordEditor editor) {
            base.SetEditor(editor);
            prevValues = editor.GetAllCells();

            Random rand = new Random();
            HashSet<string> usedValues = new HashSet<string>();

            newValues = new string[prevValues.Length][];
            for (int r = 0; r < newValues.Length; r++) {
                newValues[r] = new string[prevValues[r].Length];
                for (int c = 0; c < newValues[r].Length; c++) {
                    if (prevValues[r][c] != null) {
                        string newValue;
                        do {
                            newValue = GeneratePassword(rand, r, c);
                        } while (usedValues.Contains(newValue));
                        newValues[r][c] = newValue;
                        usedValues.Add(newValue);
                    }
                }
            }
        }

        protected abstract string GeneratePassword(Random rand, int row, int column);

        protected string GetRandomChar(Random rand) {
            return availableChars[rand.Next(availableChars.Length)].ToString();
        }

        protected override void Redo() {
            for (int r = 0; r < newValues.Length; r++) {
                for (int c = 0; c < newValues[r].Length; c++) {
                    editor.SetCell(r, c, newValues[r][c]);
                }
            }
        }

        protected override void Undo() {
            for (int r = 0; r < prevValues.Length; r++) {
                for (int c = 0; c < prevValues[r].Length; c++) {
                    editor.SetCell(r, c, prevValues[r][c]);
                }
            }
        }

        public override string ToString() {
            return "Generate passwords";
        }
    }

    class GeneratePasswordsDefaultStyleAction : GeneratePasswordsAction
    {
        private Dictionary<int, string> middleCharsForRow = new Dictionary<int, string>();

        public GeneratePasswordsDefaultStyleAction(string availableChars) : base(availableChars) { }

        protected override string GeneratePassword(Random rand, int row, int column) {
            if (!middleCharsForRow.ContainsKey(row)) {
                string middleChars;
                do {
                    middleChars = GetRandomChar(rand) + GetRandomChar(rand);
                } while (middleCharsForRow.ContainsValue(middleChars));
                middleCharsForRow[row] = middleChars;
            }
            return GetRandomChar(rand) + middleCharsForRow[row] + GetRandomChar(rand);
        }
    }

    class GeneratePasswordsAnyCharAction : GeneratePasswordsAction
    {
        public GeneratePasswordsAnyCharAction(string availableChars) : base(availableChars) { }

        protected override string GeneratePassword(Random rand, int row, int column) {
            return GetRandomChar(rand) + GetRandomChar(rand) + GetRandomChar(rand) + GetRandomChar(rand);
        }
    }

    class AddPasswordRowAction : PasswordEditorAction
    {
        private readonly int rowIndex;

        public AddPasswordRowAction(int rowIndex) {
            this.rowIndex = rowIndex;
        }

        protected override void Redo() {
            editor.AddRow(rowIndex);
        }

        protected override void Undo() {
            editor.RemoveRow(rowIndex);
        }

        public override string ToString() {
            return "Add row";
        }
    }

    class RemovePasswordRowAction : PasswordEditorAction
    {
        private readonly int rowIndex;
        private string[] prevValues;

        public RemovePasswordRowAction(int rowIndex) {
            this.rowIndex = rowIndex;
        }

        public override void SetEditor(PasswordEditor editor) {
            base.SetEditor(editor);
            prevValues = editor.GetAllCells()[rowIndex];
        }

        protected override void Redo() {
            editor.RemoveRow(rowIndex);
        }

        protected override void Undo() {
            editor.AddRow(rowIndex);
            for (int c = 0; c < prevValues.Length; c++) {
                editor.SetCell(rowIndex, c, prevValues[c]);
            }
        }

        public override string ToString() {
            return "Remove row";
        }
    }
}
