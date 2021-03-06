﻿using System;
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
                prevX.Add(obj.X);
                prevY.Add(obj.Y);
                newX.Add((ushort)((obj.X + dx) / snap * snap));
                newY.Add((ushort)((obj.Y + dy) / snap * snap));
            }
        }

        public MoveSpriteAction(IEnumerable<WrappedLevelObject> objs, ushort? x, ushort? y) : base(objs) {
            foreach (WrappedLevelObject obj in objs) {
                prevX.Add(obj.X);
                prevY.Add(obj.Y);
                newX.Add(x == null ? obj.X : (ushort)x);
                newY.Add(y == null ? obj.Y : (ushort)y);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].X = prevX[i];
                objs[i].Y = prevY[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].X = newX[i];
                objs[i].Y = newY[i];
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
        private int newType;
        private readonly List<int> prevType = new List<int>();

        public ChangeSpriteTypeAction(IEnumerable<WrappedLevelObject> objs, SpriteDisplay.Category category, int newType) : base(objs.Where(o => o.Category == category)) {
            this.newType = newType;
            foreach (WrappedLevelObject obj in base.objs) {
                prevType.Add(obj.Type);
            }
            if (prevType.All(type => type == newType)) {
                cancel = true;
            }
        }

        public ChangeSpriteTypeAction(IEnumerable<WrappedLevelObject> objs, int newType) : base(objs) {
            this.newType = newType;
            foreach (WrappedLevelObject obj in base.objs) {
                prevType.Add(obj.Type);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].Type = prevType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].Type = newType;
            }
        }

        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is ChangeSpriteTypeAction changeSpriteTypeAction) {
                if (changeSpriteTypeAction.objs.SequenceEqual(objs)) {
                    newType = changeSpriteTypeAction.newType;
                    return true;
                }
            }
            return false;
        }

        public override bool Unmerge(UndoAction<LevelEditor> action) {
            if (action is ChangeSpriteTypeAction changeSpriteTypeAction) {
                if (changeSpriteTypeAction.objs.SequenceEqual(objs)) {
                    newType = changeSpriteTypeAction.prevType[0];
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change sprite";
            } else {
                return "Change " + objs.Count.ToString() + " sprites";
            }
        }
    }

    abstract class ChangeSpritePropertyAction<ObjectType, PropertyType> : LevelEditorAction where ObjectType : WrappedLevelObject
    {
        protected readonly List<ObjectType> objs;
        private PropertyType newValue;
        private readonly List<PropertyType> oldValues = new List<PropertyType>();

        public ChangeSpritePropertyAction(IEnumerable<WrappedLevelObject> objs, PropertyType newValue) {
            this.objs = new List<ObjectType>(objs.Select(o => o as ObjectType).Where(o => o != null));
            this.newValue = newValue;
            foreach (ObjectType o in this.objs) {
                oldValues.Add(GetProperty(o));
            }
        }

        protected abstract PropertyType GetProperty(ObjectType obj);
        protected abstract void SetProperty(ObjectType obj, PropertyType value);

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                SetProperty(objs[i], newValue);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                SetProperty(objs[i], oldValues[i]);
            }
        }

        public override bool Merge(UndoAction<LevelEditor> action) {
            if (this.GetType() == action.GetType()) {
                ChangeSpritePropertyAction<ObjectType, PropertyType> changeSpritePropertyAction = action as ChangeSpritePropertyAction<ObjectType, PropertyType>;
                if (changeSpritePropertyAction.objs.SequenceEqual(objs)) {
                    newValue = changeSpritePropertyAction.newValue;
                    return true;
                }
            }
            return false;
        }
    }

    class ChangeMonsterAreaSizeAction : ChangeSpritePropertyAction<WrappedMonster, byte>
    {
        public ChangeMonsterAreaSizeAction(IEnumerable<WrappedLevelObject> objs, byte newValue) : base(objs, newValue) { }

        protected override byte GetProperty(WrappedMonster obj) => obj.AreaSize;
        protected override void SetProperty(WrappedMonster obj, byte value) => obj.AreaSize = value;

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change monster area size";
            } else {
                return "Change " + objs.Count.ToString() + " monsters' area sizes";
            }
        }
    }

    class ChangeMonsterDelayAction : ChangeSpritePropertyAction<WrappedMonster, byte>
    {
        public ChangeMonsterDelayAction(IEnumerable<WrappedLevelObject> objs, byte newValue) : base(objs, newValue) { }

        protected override byte GetProperty(WrappedMonster obj) => obj.Delay;
        protected override void SetProperty(WrappedMonster obj, byte value) => obj.Delay = value;

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change monster delay";
            } else {
                return "Change " + objs.Count.ToString() + " monsters' delays";
            }
        }
    }

    class ChangeOneShotMonsterExtraAction : ChangeSpritePropertyAction<WrappedOneShotMonster, ushort>
    {
        public ChangeOneShotMonsterExtraAction(IEnumerable<WrappedLevelObject> objs, ushort newValue) : base(objs, newValue) { }

        protected override ushort GetProperty(WrappedOneShotMonster obj) => obj.Extra;
        protected override void SetProperty(WrappedOneShotMonster obj, ushort value) => obj.Extra = value;

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change one-shot monster extra";
            } else {
                return "Change " + objs.Count.ToString() + " one-shot monsters' extras";
            }
        }
    }
}
