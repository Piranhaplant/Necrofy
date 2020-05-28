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
        }
    }

    class MoveWordAction : LevelTitleAction
    {
        private readonly List<WrappedTitleWord> words;

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
}
