using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class LevelEditorAction : UndoAction<LevelEditor>
    {
        protected Level level;

        protected override void AfterAction() {
            editor.Repaint();
            editor.RefreshPropertyBrowser();
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            level = editor.level.Level;
        }
    }

    class ClearLevelAction : LevelEditorAction
    {
        private ushort tile;
        private ushort[,] prevTiles;
        private readonly List<WrappedLevelObject> removedObjs = new List<WrappedLevelObject>();
        private readonly List<WrappedLevelObject> movedObjs = new List<WrappedLevelObject>();
        private readonly List<ushort> prevX = new List<ushort>();
        private readonly List<ushort> prevY = new List<ushort>();

        public ClearLevelAction(ushort tile) {
            this.tile = tile;
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            prevTiles = (ushort[,])level.background.Clone();
            foreach (WrappedLevelObject obj in editor.level.GetAllObjects()) {
                if (obj.Removable) {
                    removedObjs.Add(obj);
                } else {
                    movedObjs.Add(obj);
                    prevX.Add(obj.X);
                    prevY.Add(obj.Y);
                }
            }
        }

        protected override void Undo() {
            level.background = (ushort[,])prevTiles.Clone();
            foreach (WrappedLevelObject obj in removedObjs) {
                obj.Add(level);
            }
            for (int i = 0; i < movedObjs.Count; i++) {
                movedObjs[i].X = prevX[i];
                movedObjs[i].Y = prevY[i];
            }
        }

        protected override void Redo() {
            for (int y = 0; y < level.height; y++) {
                for (int x = 0; x < level.width; x++) {
                    level.background[x, y] = tile;
                }
            }
            foreach (WrappedLevelObject obj in removedObjs) {
                obj.Remove(level);
            }
            int xPos = 0;
            foreach (WrappedLevelObject obj in movedObjs) {
                obj.X = (ushort)(xPos + obj.X - obj.Bounds.X);
                obj.Y = 0;
                xPos += obj.Bounds.Width;
            }
            editor.UpdateSpriteSelection();
        }

        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is ClearLevelAction clearLevelAction) {
                tile = clearLevelAction.tile;
                return true;
            }
            return false;
        }

        public override string ToString() {
            return "Clear level";
        }
    }
}
