﻿using System;
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
        private readonly Point start;
        private readonly Point end;
        private Color[,] oldColors;
        private Color newColor;

        public ChangeColorAction(Color newColor, Point start, Point end) {
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
        private readonly Point start;
        private Color[,] oldColors;
        private Color[,] newColors;

        public PasteColorsAction(Color[,] newColors, Point start) {
            this.newColors = newColors;
            this.start = start;
        }

        public override void SetEditor(PaletteEditor editor) {
            base.SetEditor(editor);
            oldColors = editor.GetColors(start, new Point(start.X + newColors.GetWidth() - 1, start.Y + newColors.GetHeight() - 1));
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
