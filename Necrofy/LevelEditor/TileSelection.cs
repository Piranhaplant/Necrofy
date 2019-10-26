using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class TileSelection
    {
        private int width;
        private int height;
        private bool[,] points;
        private bool[,] curPoints;
        private bool useCurPoints = false;

        // Selection rectangle
        private int startX;
        private int startY;
        private int curX;
        private int curY;
        private bool adding;

        public event EventHandler Changed;

        public TileSelection(int width, int height) {
            this.width = width;
            this.height = height;
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

        public void StartRect(int x, int y, bool selected) {
            startX = x;
            startY = y;
            curX = int.MinValue;
            curY = int.MinValue;
            adding = selected;
        }

        public void MoveRect(int newX, int newY) {
            if (newX != curX || newY != curY) {
                useCurPoints = true;
                curX = newX;
                curY = newY;

                Array.Copy(points, curPoints, points.Length);
                int firstX = Math.Max(0, Math.Min(startX, newX));
                int firstY = Math.Max(0, Math.Min(startY, newY));
                int lastX = Math.Min(width - 1, Math.Max(startX, newX));
                int lastY = Math.Min(height - 1, Math.Max(startY, newY));
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

        public Rectangle GetEraserRectangle() {
            if (useCurPoints && !adding) {
                int firstX = Math.Max(0, Math.Min(startX, curX));
                int firstY = Math.Max(0, Math.Min(startY, curY));
                int lastX = Math.Min(width - 1, Math.Max(startX, curX));
                int lastY = Math.Min(height - 1, Math.Max(startY, curY));
                return new Rectangle(firstX * 64, firstY * 64, (lastX - firstX + 1) * 64, (lastY - firstY + 1) * 64);
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
            List<Edge> edges = new List<Edge>();
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (p[x, y]) {
                        if (x == 0 || !p[x - 1, y]) edges.Add(new Edge(x, y, x, y + 1));
                        if (y == 0 || !p[x, y - 1]) edges.Add(new Edge(x, y, x + 1, y));
                        if (x == width - 1 || !p[x + 1, y]) edges.Add(new Edge(x + 1, y, x + 1, y + 1));
                        if (y == height - 1 || !p[x, y + 1]) edges.Add(new Edge(x, y + 1, x + 1, y + 1));
                    }
                }
            }

            if (edges.Count == 0) {
                return null;
            }

            GraphicsPath gp = new GraphicsPath();
            while (edges.Count > 0) {
                List<Point> polygon = new List<Point>();
                Point startPoint = edges[edges.Count - 1].p1;
                Point nextPoint = edges[edges.Count - 1].p2;
                edges.RemoveAt(edges.Count - 1);

                polygon.Add(startPoint);
                while (nextPoint != startPoint) {
                    polygon.Add(nextPoint);
                    for (int i = 0; i < edges.Count; i++) {
                        Edge e = edges[i];
                        if (e.p1 == nextPoint) {
                            edges.RemoveAt(i);
                            nextPoint = e.p2;
                            break;
                        } else if (e.p2 == nextPoint) {
                            edges.RemoveAt(i);
                            nextPoint = e.p1;
                            break;
                        }
                    }
                }
                gp.AddPolygon(polygon.ToArray());
            }

            Matrix scaleMatrix = new Matrix();
            scaleMatrix.Scale(64, 64);
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
