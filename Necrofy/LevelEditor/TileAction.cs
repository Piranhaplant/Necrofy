﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class PaintTileAction : LevelEditorAction
    {
        private readonly int x1;
        private readonly int y1;
        private readonly int x2;
        private readonly int y2;

        private readonly List<Point> points = new List<Point>();
        private readonly List<ushort> prevTileType = new List<ushort>();
        private readonly ushort tileType;

        public PaintTileAction(int x1, int y1, int x2, int y2, ushort tileType) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.tileType = tileType;
        }
        
        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            MapEditor.DrawLine(x1, y1, x2, y2, (x, y) => {
                if (x >= 0 && y >= 0 && x < level.width && y < level.height) {
                    points.Add(new Point(x, y));
                    prevTileType.Add(level.background[x, y]);
                }
            });

            if (points.Count == 0) {
                cancel = true;
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
        
        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is PaintTileAction paintTileAction) {
                if (paintTileAction.tileType == tileType) {
                    for (int i = 0; i < paintTileAction.points.Count; i++) {
                        if (!points.Contains(paintTileAction.points[i])) {
                            points.Add(paintTileAction.points[i]);
                            prevTileType.Add(paintTileAction.prevTileType[i]);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Paint tiles";
        }
    }

    class TileSuggestAction : LevelEditorAction
    {
        private readonly int x;
        private readonly int y;
        private ushort prevTileType;
        private ushort tileType;

        public TileSuggestAction(int x, int y, ushort tileType) {
            this.x = x;
            this.y = y;
            this.tileType = tileType;
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            prevTileType = level.background[x, y];
        }

        protected override void Undo() {
            level.background[x, y] = prevTileType;
        }

        protected override void Redo() {
            level.background[x, y] = tileType;
        }
        
        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is TileSuggestAction tileSuggestAction) {
                if (tileSuggestAction.x == x && tileSuggestAction.y == y) {
                    tileType = tileSuggestAction.tileType;
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            return "Change Tile";
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
                    if (editor.Selection.GetPoint(x, y)) {
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
        
        public override bool Merge(UndoAction<LevelEditor> action) {
            if (action is FillSelectionAction fillSelectionAction) {
                if (fillSelectionAction.points.SequenceEqual(points)) {
                    tileType = fillSelectionAction.tileType;
                    return true;
                }
            }
            return false;
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

    class ResizeLevelAction : LevelEditorAction
    {
        private readonly int startX;
        private readonly int startY;
        private readonly int endX;
        private readonly int endY;
        private readonly ushort tileType;

        private ushort[,] oldTiles;
        private readonly Dictionary<WrappedLevelObject, Point> oldPositions = new Dictionary<WrappedLevelObject, Point>();

        public ResizeLevelAction(int startX, int startY, int endX, int endY, ushort tileType) {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.tileType = tileType;
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            oldTiles = level.background.Clone() as ushort[,];
            foreach (WrappedLevelObject o in editor.level.GetAllObjects()) {
                if (o.X < startX * 64 || o.X > endX * 64 || o.Y < startY * 64 || o.Y > endY * 64) {
                    oldPositions[o] = new Point(o.X, o.Y);
                }
            }
        }

        protected override void Undo() {
            level.background = oldTiles;
            foreach (WrappedLevelObject o in editor.level.GetAllObjects()) {
                o.X = (ushort)Math.Max(0, o.X + startX * 64);
                o.Y = (ushort)Math.Max(0, o.Y + startY * 64);
            }
            foreach (KeyValuePair<WrappedLevelObject, Point> pair in oldPositions) {
                pair.Key.X = (ushort)pair.Value.X;
                pair.Key.Y = (ushort)pair.Value.Y;
            }
            editor.ResizeMap(-startX, -startY, oldTiles.GetWidth() - startX, oldTiles.GetHeight() - startY);
        }

        protected override void Redo() {
            level.background = new ushort[endX - startX, endY - startY];
            for (int y = 0; y < level.height; y++) {
                for (int x = 0; x < level.width; x++) {
                    level.background[x, y] = tileType;
                }
            }
            for (int y = Math.Max(startY, 0); y < Math.Min(endY, oldTiles.GetHeight()); y++) {
                for (int x = Math.Max(startX, 0); x < Math.Min(endX, oldTiles.GetWidth()); x++) {
                    level.background[x - startX, y - startY] = oldTiles[x, y];
                }
            }
            foreach (WrappedLevelObject o in editor.level.GetAllObjects()) {
                o.X = (ushort)Math.Max(0, Math.Min(level.width * 64, o.X - startX * 64));
                o.Y = (ushort)Math.Max(0, Math.Min(level.height * 64, o.Y - startY * 64));
            }
            editor.ResizeMap(startX, startY, endX, endY);
        }
        
        public override string ToString() {
            return "Resize Level";
        }
    }
}
