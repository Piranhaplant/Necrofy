﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class SpriteEditorAction : UndoAction<SpriteEditor>
    {
        public readonly Sprite sprite;
        public readonly List<WrappedSpriteTile> objs;

        public SpriteEditorAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs) {
            this.sprite = sprite;
            this.objs = new List<WrappedSpriteTile>(objs);
            if (this.objs.Count == 0) {
                cancel = true;
            }
        }

        protected override void AfterAction() {
            editor.SetCurrentSprite(sprite);
            editor.Repaint();
            editor.RefreshPropertyBrowser();
        }
    }

    class MoveSpriteTileAction : SpriteEditorAction
    {
        private readonly List<short> prevX = new List<short>();
        private readonly List<short> prevY = new List<short>();
        private readonly List<short> newX = new List<short>();
        private readonly List<short> newY = new List<short>();

        public MoveSpriteTileAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, int dx, int dy) : base(sprite, objs) {
            foreach (WrappedSpriteTile obj in objs) {
                prevX.Add(obj.X);
                prevY.Add(obj.Y);
                newX.Add((short)(obj.X + dx));
                newY.Add((short)(obj.Y + dy));
            }
        }

        public MoveSpriteTileAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, ushort? x, ushort? y) : base(sprite, objs) {
            foreach (WrappedSpriteTile obj in objs) {
                prevX.Add(obj.X);
                prevY.Add(obj.Y);
                newX.Add(x == null ? obj.X : (short)x);
                newY.Add(y == null ? obj.Y : (short)y);
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

        public override bool Merge(UndoAction<SpriteEditor> action) {
            if (action is MoveSpriteTileAction moveSpriteTileAction) {
                if (moveSpriteTileAction.objs.SequenceEqual(objs)) {
                    for (int i = 0; i < objs.Count; i++) {
                        newX[i] = moveSpriteTileAction.newX[i];
                        newY[i] = moveSpriteTileAction.newY[i];
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Move tile";
            } else {
                return "Move " + objs.Count.ToString() + " tiles";
            }
        }
    }

    class DeleteSpriteTileAction : SpriteEditorAction
    {
        private readonly List<int> zIndexes;

        public DeleteSpriteTileAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, List<int> zIndexes) : base(sprite, objs) {
            this.zIndexes = zIndexes;
        }
        
        protected override void Undo() {
            editor.AddTiles(sprite, objs, zIndexes);
        }

        protected override void Redo() {
            editor.RemoveTiles(sprite, objs);
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Delete tile";
            } else {
                return "Delete " + objs.Count.ToString() + " tiles";
            }
        }
    }

    class AddSpriteTileAction : SpriteEditorAction
    {
        public AddSpriteTileAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs) : base(sprite, objs) { }

        protected override void Undo() {
            editor.RemoveTiles(sprite, objs);
        }

        protected override void Redo() {
            editor.AddTiles(sprite, objs);
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Add tile";
            } else {
                return "Add " + objs.Count.ToString() + " sprites";
            }
        }

        public override bool Merge(UndoAction<SpriteEditor> action) {
            if (action is MoveSpriteTileAction moveSpriteTileAction) {
                if (moveSpriteTileAction.objs.SequenceEqual(objs)) {
                    return true;
                }
            }
            return false;
        }
    }

    class ChangeSpriteTileNumAction : SpriteEditorAction
    {
        private ushort newType;
        private readonly List<ushort> prevType = new List<ushort>();

        public ChangeSpriteTileNumAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, ushort newType) : base(sprite, objs) {
            this.newType = newType;
            foreach (WrappedSpriteTile obj in base.objs) {
                prevType.Add(obj.TileNum);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].TileNum = prevType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].TileNum = newType;
            }
        }

        public override bool Merge(UndoAction<SpriteEditor> action) {
            if (action is ChangeSpriteTileNumAction changeSpriteTileNumAction) {
                if (changeSpriteTileNumAction.objs.SequenceEqual(objs)) {
                    newType = changeSpriteTileNumAction.newType;
                    return true;
                }
            }
            return false;
        }

        public override bool Unmerge(UndoAction<SpriteEditor> action) {
            if (action is ChangeSpriteTileNumAction changeSpriteTileNumAction) {
                if (changeSpriteTileNumAction.objs.SequenceEqual(objs)) {
                    newType = changeSpriteTileNumAction.prevType[0];
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change tile";
            } else {
                return "Change " + objs.Count.ToString() + " tiles";
            }
        }
    }

    class ChangeSpriteTilePaletteAction : SpriteEditorAction
    {
        private readonly List<int> prevPalette = new List<int>();
        private readonly int newPalette;

        public ChangeSpriteTilePaletteAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, int palette) : base(sprite, objs) {
            newPalette = palette;
            foreach (WrappedSpriteTile obj in objs) {
                prevPalette.Add(obj.Palette);
            }
        }

        protected override void Undo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].Palette = prevPalette[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i < objs.Count; i++) {
                objs[i].Palette = newPalette;
            }
        }

        protected override void AfterAction() {
            base.AfterAction();
            editor.UpdateSelectedPalette();
        }

        public override string ToString() {
            if (objs.Count == 1) {
                return "Change tile palette";
            } else {
                return "Change " + objs.Count.ToString() + " tile palettes";
            }
        }
    }

    class ChangeSpriteTileZIndexAction : SpriteEditorAction
    {
        private readonly List<int> oldZIndexes;
        private List<int> newZIndexes;

        public ChangeSpriteTileZIndexAction(Sprite sprite, IEnumerable<WrappedSpriteTile> objs, List<int> oldZIndexes, List<int> newZIndexes) : base(sprite, objs) {
            this.oldZIndexes = oldZIndexes;
            this.newZIndexes = newZIndexes;
            if (newZIndexes.SequenceEqual(oldZIndexes)) {
                cancel = true;
            }
        }

        protected override void Undo() {
            editor.RemoveTiles(sprite, objs, updateSelection: false);
            editor.AddTiles(sprite, objs, oldZIndexes);
        }

        protected override void Redo() {
            editor.RemoveTiles(sprite, objs, updateSelection: false);
            editor.AddTiles(sprite, objs, newZIndexes);
        }

        public override bool Merge(UndoAction<SpriteEditor> action) {
            if (action is ChangeSpriteTileZIndexAction changeSpriteTileZIndexAction) {
                if (changeSpriteTileZIndexAction.objs.SequenceEqual(objs)) {
                    newZIndexes = changeSpriteTileZIndexAction.newZIndexes;
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Change tile order";
        }
    }
}
