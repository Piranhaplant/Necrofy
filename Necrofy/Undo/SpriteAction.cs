using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class MoveSpriteAction : LevelEditorAction
    {
        private readonly List<WrappedLevelObject> objs;
        private readonly List<ushort> prevX = new List<ushort>();
        private readonly List<ushort> prevY = new List<ushort>();
        private readonly List<ushort> newX = new List<ushort>();
        private readonly List<ushort> newY = new List<ushort>();

        public MoveSpriteAction(IEnumerable<WrappedLevelObject> objs, int dx, int dy, int snap) {
            this.objs = new List<WrappedLevelObject>(objs);
            foreach (WrappedLevelObject obj in objs) {
                prevX.Add(obj.x);
                prevY.Add(obj.y);
                newX.Add((ushort)((obj.x + dx) / snap * snap));
                newY.Add((ushort)((obj.y + dy) / snap * snap));
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].x = prevX[i];
                objs[i].y = prevY[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].x = newX[i];
                objs[i].y = newY[i];
            }
        }

        public override bool CanMerge => true;

        public override void Merge(UndoAction<LevelEditor> action) {
            MoveSpriteAction moveSpriteAction = (MoveSpriteAction)action;
            for (int i = 0; i < objs.Count; i++) {
                newX[i] = moveSpriteAction.newX[i];
                newY[i] = moveSpriteAction.newY[i];
            }
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Move sprite";
            } else {
                return "Move " + objs.Count.ToString() + " sprites";
            }
        }
    }
}
