using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class PaletteEditorAction : UndoAction<PaletteEditor>
    {
    }

    class ChangeColorAction : PaletteEditorAction
    {
        private readonly int start;
        private readonly int end;
        private Color[] oldColors;
        private Color newColor;

        public ChangeColorAction(Color newColor, int start, int end) {
            this.newColor = newColor;
            this.start = start;
            this.end = end;
        }

        public override void SetEditor(PaletteEditor editor) {
            base.SetEditor(editor);
            oldColors = editor.GetColors(start, end);
        }

        protected override void Undo() {
            editor.SetColors(oldColors, start);
        }

        protected override void Redo() {
            editor.SetColor(newColor, start, end);
        }

        public override bool Merge(UndoAction<PaletteEditor> action) {
            if (action is ChangeColorAction changeColorAction) {
                if (changeColorAction.start == start && changeColorAction.end == end) {
                    newColor = changeColorAction.newColor;
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Change color";
        }
    }

    class PasteColorsAction : PaletteEditorAction
    {
        private readonly int start;
        private Color[] oldColors;
        private Color[] newColors;

        public PasteColorsAction(Color[] newColors, int start) {
            this.newColors = newColors;
            this.start = start;
        }

        public override void SetEditor(PaletteEditor editor) {
            base.SetEditor(editor);
            oldColors = editor.GetColors(start, start + newColors.Length - 1);
        }

        protected override void Undo() {
            editor.SetColors(oldColors, start);
        }

        protected override void Redo() {
            editor.SetColors(newColors, start);
        }

        public override string ToString() {
            return "Paste colors";
        }
    }
}
