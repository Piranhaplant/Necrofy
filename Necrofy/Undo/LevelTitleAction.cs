using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class LevelTitleAction : UndoAction<TitleEditor>
    {
        protected override void AfterAction() {
            editor.Repaint();
            editor.RefreshPropertyBrowser();
        }
    }

    class MoveWordAction : LevelTitleAction
    {
        public readonly List<WrappedTitleWord> words;

        private readonly List<byte> prevX = new List<byte>();
        private readonly List<byte> prevY = new List<byte>();
        private readonly List<byte> newX = new List<byte>();
        private readonly List<byte> newY = new List<byte>();

        public MoveWordAction(IEnumerable<WrappedTitleWord> words, int dx, int dy) {
            this.words = new List<WrappedTitleWord>(words);
            foreach (WrappedTitleWord word in words) {
                prevX.Add(word.X);
                prevY.Add(word.Y);
                newX.Add((byte)(word.X + dx));
                newY.Add((byte)(word.Y + dy));
            }
        }

        public MoveWordAction(IEnumerable<WrappedTitleWord> words, byte? x, byte? y) {
            this.words = new List<WrappedTitleWord>(words);
            foreach (WrappedTitleWord word in words) {
                prevX.Add(word.X);
                prevY.Add(word.Y);
                newX.Add(x == null ? word.X : (byte)x);
                newY.Add(y == null ? word.Y : (byte)y);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < words.Count; i++) {
                words[i].SetPosition(prevX[i], prevY[i]);
            }
        }

        protected override void Redo() {
            for (int i = 0; i < words.Count; i++) {
                words[i].SetPosition(newX[i], newY[i]);
            }
        }

        public override bool Merge(UndoAction<TitleEditor> action) {
            if (action is MoveWordAction moveWordAction) {
                if (moveWordAction.words.SequenceEqual(words)) {
                    for (int i = 0; i < words.Count; i++) {
                        newX[i] = moveWordAction.newX[i];
                        newY[i] = moveWordAction.newY[i];
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Move text";
        }
    }

    class AddWordAdtion : LevelTitleAction
    {
        private readonly PageEditor pageEditor;
        private readonly List<WrappedTitleWord> words;

        public AddWordAdtion(PageEditor pageEditor, IEnumerable<WrappedTitleWord> words) {
            this.pageEditor = pageEditor;
            this.words = new List<WrappedTitleWord>(words);
        }

        protected override void Undo() {
            pageEditor.RemoveWords(words);
        }

        protected override void Redo() {
            pageEditor.AddWords(words);
        }

        public override bool Merge(UndoAction<TitleEditor> action) {
            if (action is ChangeWordTextAction changeWordTextAction) {
                if (words.Count == 1 && words[0] == changeWordTextAction.word) {
                    return true;
                }
            }
            if (action is MoveWordAction moveWordAction) {
                if (words.SequenceEqual(moveWordAction.words)) {
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Add text";
        }
    }

    class RemoveWordAction : LevelTitleAction
    {
        private readonly PageEditor pageEditor;
        private readonly List<WrappedTitleWord> words;
        private readonly List<int> zIndexes;

        public RemoveWordAction(PageEditor pageEditor, IEnumerable<WrappedTitleWord> words) {
            this.pageEditor = pageEditor;
            this.words = new List<WrappedTitleWord>(words);
            zIndexes = pageEditor.SortAndGetZIndexes(this.words);
        }

        protected override void Undo() {
            pageEditor.AddWords(words, zIndexes);
        }

        protected override void Redo() {
            pageEditor.RemoveWords(words);
        }

        public override string ToString() {
            return "Remove text";
        }
    }

    class ChangeWordPaletteAction : LevelTitleAction
    {
        private readonly List<WrappedTitleWord> words;

        private readonly List<byte> prevPalette = new List<byte>();
        private readonly byte newPalette;

        public ChangeWordPaletteAction(IEnumerable<WrappedTitleWord> words, byte palette) {
            this.words = new List<WrappedTitleWord>(words);
            newPalette = palette;
            foreach (WrappedTitleWord word in words) {
                prevPalette.Add(word.Palette);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < words.Count; i++) {
                words[i].Palette = prevPalette[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < words.Count; i++) {
                words[i].Palette = newPalette;
            }
        }

        protected override void AfterAction() {
            base.AfterAction();
            editor.UpdateSelectedPalette();
        }

        public override string ToString() {
            return "Change palette";
        }
    }

    class ChangeWordTextAction : LevelTitleAction
    {
        private readonly PageEditor pageEditor;
        public readonly WrappedTitleWord word;

        private readonly List<byte> oldChars;
        private List<byte> newChars;
        private readonly int oldCaretPosition;
        private int newCaretPosition;

        public ChangeWordTextAction(PageEditor pageEditor, WrappedTitleWord word, List<byte> newChars, int oldCaretPosition) {
            this.pageEditor = pageEditor;
            this.word = word;
            oldChars = new List<byte>(word.Chars);
            this.newChars = newChars;
            this.oldCaretPosition = oldCaretPosition;
            newCaretPosition = pageEditor.CaretPosition;
        }

        protected override void Undo() {
            word.Chars = oldChars;
            pageEditor.SetCaretPosition(word, oldCaretPosition);
        }

        protected override void Redo() {
            word.Chars = newChars;
            pageEditor.SetCaretPosition(word, newCaretPosition);
        }
        
        public override bool Merge(UndoAction<TitleEditor> action) {
            if (action is ChangeWordTextAction changeWordTextAction && changeWordTextAction.word == word) {
                newChars = changeWordTextAction.newChars;
                newCaretPosition = changeWordTextAction.newCaretPosition;
                return true;
            }
            return false;
        }

        public override string ToString() {
            return "Change text";
        }
    }

    class ChangeDisplayNameAction : UndoAction<TitleEditor>
    {
        private readonly string prevValue;
        private string newValue;

        public ChangeDisplayNameAction(string prevValue, string value) {
            this.prevValue = prevValue;
            newValue = value;
        }
        
        protected override void Undo() {
            editor.DisplayName = prevValue;
        }

        protected override void Redo() {
            editor.DisplayName = newValue;
        }

        public override bool Merge(UndoAction<TitleEditor> action) {
            if (action is ChangeDisplayNameAction changeDisplayNameAction) {
                newValue = changeDisplayNameAction.newValue;
                return true;
            }
            return false;
        }

        public override string ToString() {
            return "Change display name";
        }
    }
}
