﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TileSelection
    {
        public int width { get; private set; }
        public int height { get; private set; }
        private readonly int scale;
        private bool[,] points;
        private bool[,] curPoints;
        private bool useCurPoints = false;

        // Selection rectangle
        private int startX;
        private int startY;
        private int curX;
        private int curY;
        private bool adding;
        private int snap;
        
        public Rectangle CurrentRectangle {
            get {
                Rectangle r = GetDrawRectangle(scale: 1);
                r.Intersect(new Rectangle(0, 0, width, height));
                return r;
            }
        }
        
        public event EventHandler Changed;

        public TileSelection(int width, int height, int scale) {
            this.width = width;
            this.height = height;
            this.scale = scale;
            points = new bool[width, height];
            curPoints = new bool[width, height];
        }

        public void Clear() {
            useCurPoints = false;
            Array.Clear(points, 0, points.Length);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetPoint(int x, int y, bool selected) {
            if (x >= 0 && x < width && y >= 0 && y < height) {
                points[x, y] = selected;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetAllPoints(Func<int, int, bool> setter) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    points[x, y] = setter(x, y);
                }
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool GetPoint(int x, int y) {
            return points[x, y];
        }

        public void Resize(int startX, int startY, int endX, int endY) {
            bool[,] oldPoints = points;

            width = endX - startX;
            height = endY - startY;
            points = new bool[width, height];
            curPoints = new bool[width, height];

            for (int y = Math.Max(startY, 0); y < Math.Min(endY, oldPoints.GetHeight()); y++) {
                for (int x = Math.Max(startX, 0); x < Math.Min(endX, oldPoints.GetWidth()); x++) {
                    points[x - startX, y - startY] = oldPoints[x, y];
                }
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void StartRect(int x, int y, bool selected, int snap = 1) {
            startX = x / snap;
            startY = y / snap;
            curX = int.MinValue;
            curY = int.MinValue;
            adding = selected;
            this.snap = snap;
        }

        public void MoveRect(int newX, int newY) {
            newX = newX / snap;
            newY = newY / snap;
            if (newX != curX || newY != curY) {
                useCurPoints = true;
                curX = newX;
                curY = newY;

                Array.Copy(points, curPoints, points.Length);
                int firstX = Math.Max(0, Math.Min(startX, newX) * snap);
                int firstY = Math.Max(0, Math.Min(startY, newY) * snap);
                int lastX = Math.Min(width - 1, Math.Max(startX, newX) * snap + snap - 1);
                int lastY = Math.Min(height - 1, Math.Max(startY, newY) * snap + snap - 1);
                for (int y = firstY; y <= lastY; y++) {
                    for (int x = firstX; x <= lastX; x++) {
                        curPoints[x, y] = adding;
                    }
                }

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void EndRect() {
            if (useCurPoints) {
                Array.Copy(curPoints, points, points.Length);
            }
            useCurPoints = false;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public Rectangle GetDrawRectangle() {
            return GetDrawRectangle(scale);
        }

        private Rectangle GetDrawRectangle(int scale) {
            if (useCurPoints) {
                int firstX = Math.Max(0, Math.Min(startX, curX) * snap);
                int firstY = Math.Max(0, Math.Min(startY, curY) * snap);
                int lastX = Math.Min(width - 1, Math.Max(startX, curX) * snap + snap - 1);
                int lastY = Math.Min(height - 1, Math.Max(startY, curY) * snap + snap - 1);
                return new Rectangle(firstX * scale, firstY * scale, (lastX - firstX + 1) * scale, (lastY - firstY + 1) * scale);
            }
            return Rectangle.Empty;
        }
        
        public Rectangle GetEraserRectangle() {
            if (!adding) {
                return GetDrawRectangle();
            }
            return Rectangle.Empty;
        }

        public GraphicsPath GetGraphicsPath() {
            if (useCurPoints) {
                return GetGraphicsPath(curPoints);
            } else {
                return GetGraphicsPath(points);
            }
        }

        private GraphicsPath GetGraphicsPath(bool[,] p) {
            Dictionary<Point, Edge> edges = new Dictionary<Point, Edge>();
            void AddEdge(int x1, int y1, int x2, int y2) {
                Edge e = new Edge(x1, y1, x2, y2);
                edges[e.p1] = e;
            }

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (p[x, y]) {
                        if (x == 0 || !p[x - 1, y]) AddEdge(x, y + 1, x, y);
                        if (y == 0 || !p[x, y - 1]) AddEdge(x, y, x + 1, y);
                        if (x == width - 1 || !p[x + 1, y]) AddEdge(x + 1, y, x + 1, y + 1);
                        if (y == height - 1 || !p[x, y + 1]) AddEdge(x + 1, y + 1, x, y + 1);
                    }
                }
            }

            if (edges.Count == 0) {
                return null;
            }

            GraphicsPath gp = new GraphicsPath();
            while (edges.Count > 0) {
                List<Point> polygon = new List<Point>();
                KeyValuePair<Point, Edge> start = edges.First();
                Point startPoint = start.Value.p1;
                Point nextPoint = start.Value.p2;
                edges.Remove(start.Key);

                polygon.Add(startPoint);
                while (nextPoint != startPoint) {
                    polygon.Add(nextPoint);
                    Edge nextEdge = edges[nextPoint];
                    edges.Remove(nextPoint);
                    nextPoint = nextEdge.p2;
                }
                gp.AddPolygon(polygon.ToArray());
            }

            Matrix scaleMatrix = new Matrix();
            scaleMatrix.Scale(scale, scale);
            gp.Transform(scaleMatrix);

            return gp;
        }

        private class Edge
        {
            public readonly Point p1;
            public readonly Point p2;

            public Edge(int x1, int y1, int x2, int y2) {
                p1 = new Point(x1, y1);
                p2 = new Point(x2, y2);
            }
        }
    }
}
