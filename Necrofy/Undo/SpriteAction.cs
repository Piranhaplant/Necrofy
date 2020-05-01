using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class SpriteAction : LevelEditorAction
    {
        public readonly List<WrappedLevelObject> objs;

        public SpriteAction(IEnumerable<WrappedLevelObject> objs) {
            this.objs = new List<WrappedLevelObject>(objs);
            if (this.objs.Count == 0) {
                cancel = true;
            }
        }
    }

    class MoveSpriteAction : SpriteAction
    {
        private readonly List<ushort> prevX = new List<ushort>();
        private readonly List<ushort> prevY = new List<ushort>();
        private readonly List<ushort> newX = new List<ushort>();
        private readonly List<ushort> newY = new List<ushort>();

        public MoveSpriteAction(IEnumerable<WrappedLevelObject> objs, int dx, int dy, int snap) : base(objs) {
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
        
        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is MoveSpriteAction moveSpriteAction) {
                if (moveSpriteAction.objs.SequenceEqual(objs)) {
                    for (int i = 0; i < objs.Count; i++) {
                        newX[i] = moveSpriteAction.newX[i];
                        newY[i] = moveSpriteAction.newY[i];
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Move sprite";
            } else {
                return "Move " + objs.Count.ToString() + " sprites";
            }
        }
    }

    class DeleteSpriteAction : SpriteAction
    {
        public DeleteSpriteAction(IEnumerable<WrappedLevelObject> objs) : base(objs.Where(o => o.Removable)) { }

        protected override void Undo() {
            foreach (WrappedLevelObject obj in objs) {
                obj.Add(level);
            }
        }

        protected override void Redo() {
            foreach (WrappedLevelObject obj in objs) {
                obj.Remove(level);
            }
            editor.UpdateSpriteSelection();
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Delete sprite";
            } else {
                return "Delete " + objs.Count.ToString() + " sprites";
            }
        }
    }

    class AddSpriteAction : SpriteAction
    {
        public AddSpriteAction(IEnumerable<WrappedLevelObject> objs) : base(objs.Where(o => o.Removable)) { }

        protected override void Undo() {
            foreach (WrappedLevelObject obj in objs) {
                obj.Remove(level);
            }
            editor.UpdateSpriteSelection();
        }

        protected override void Redo() {
            foreach (WrappedLevelObject obj in objs) {
                obj.Add(level);
            }
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Add sprite";
            } else {
                return "Add " + objs.Count.ToString() + " sprites";
            }
        }

        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is MoveSpriteAction moveSpriteAction) {
                if (moveSpriteAction.objs.SequenceEqual(objs)) {
                    return true;
                }
            }
            return false;
        }
    }

    class ChangeSpriteTypeAction : SpriteAction
    {
        private readonly int newType;
        private readonly List<int> prevType = new List<int>();

        public ChangeSpriteTypeAction(IEnumerable<WrappedLevelObject> objs, SpriteDisplay.Category category, SpriteDisplay.Key newType) : base(objs.Where(o => o.Category == category)) {
            this.newType = newType.value;
            foreach (WrappedLevelObject obj in objs) {
                prevType.Add(obj.type);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].type = prevType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].type = newType;
            }
        }
        
        // Not allowing merging because it gets complicated when there are multiple types of sprites selected at the same time

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change sprite";
            } else {
                return "Change " + objs.Count.ToString() + " sprites";
            }
        }
    }
}
