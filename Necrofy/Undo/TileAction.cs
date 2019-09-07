using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class PaintTileAction : LevelEditorAction
    {
        private readonly List<Point> points = new List<Point>();
        private readonly List<ushort> prevTileType = new List<ushort>();
        private readonly ushort tileType;

        public PaintTileAction(int x, int y, ushort tileType) {
            points.Add(new Point(x, y));
            this.tileType = tileType;
        }
        
        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            prevTileType.Add(level.background[points[0].X, points[0].Y]);
        }

        protected override void Undo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = prevTileType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = tileType;
            }
        }

        public override bool CanMerge => true;

        public override void Merge(UndoAction<LevelEditor> action) {
            PaintTileAction paintTileAction = (PaintTileAction)action;
            if (!points.Contains(paintTileAction.points[0])) {
                points.Add(paintTileAction.points[0]);
                prevTileType.Add(paintTileAction.prevTileType[0]);
            }
        }

        public override string ToString() {
            return "Paint tiles";
        }
    }

    class FillSelectionAction : LevelEditorAction
    {
        private readonly List<Point> points = new List<Point>();
        private readonly List<ushort> prevTileType = new List<ushort>();
        private ushort tileType;

        public FillSelectionAction(ushort tileType) {
            this.tileType = tileType;
        }
        
        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            for (int y = 0; y < editor.level.Level.height; y++) {
                for (int x = 0; x < editor.level.Level.width; x++) {
                    if (editor.tileSelection.GetPoint(x, y)) {
                        points.Add(new Point(x, y));
                        prevTileType.Add(editor.level.Level.background[x, y]);
                    }
                }
            }
        }

        protected override void Undo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = prevTileType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = tileType;
            }
        }

        public override bool CanMerge => true;

        public override void Merge(UndoAction<LevelEditor> action) {
            tileType = ((FillSelectionAction)action).tileType;
        }

        public override string ToString() {
            return "Fill Selection";
        }
    }

    class PasteTilesAction : LevelEditorAction
    {
        private readonly int pasteX;
        private readonly int pasteY;
        private ushort?[,] tiles;

        private readonly List<Point> points = new List<Point>();
        private readonly List<ushort> prevTileType = new List<ushort>();
        private readonly List<ushort> tileType = new List<ushort>();

        public PasteTilesAction(int pasteX, int pasteY, ushort?[,] tiles) {
            this.pasteX = pasteX;
            this.pasteY = pasteY;
            this.tiles = tiles;
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            for (int y = pasteY; y < pasteY + tiles.GetHeight(); y++) {
                for (int x = pasteX; x < pasteX + tiles.GetWidth(); x++) {
                    ushort? tile = tiles[x - pasteX, y - pasteY];
                    if (x >= 0 && x < level.width && y >= 0 && y < level.height && tile != null) {
                        points.Add(new Point(x, y));
                        tileType.Add((ushort)tile);
                        prevTileType.Add(level.background[x, y]);
                    }
                }
            }
            cancel = points.Count == 0;
            tiles = null;
        }

        protected override void Undo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = prevTileType[i];
            }
        }

        protected override void Redo() {
            for (int i = 0; i <= points.Count - 1; i++) {
                level.background[points[i].X, points[i].Y] = tileType[i];
            }
        }

        public override string ToString() {
            return "Paste Tiles";
        }
    }
}
