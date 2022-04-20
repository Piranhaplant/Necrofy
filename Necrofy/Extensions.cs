using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    static class Extensions
    {
        public static int GetWidth<T>(this T[,] array) {
            return array.GetLength(0);
        }

        public static int GetHeight<T>(this T[,] array) {
            return array.GetLength(1);
        }

        public static IEnumerable<T> GetFlags<T>(this T e) where T : Enum {
            foreach (Enum value in Enum.GetValues(typeof(T))) {
                if (!Equals(Convert.ChangeType(value, value.GetTypeCode()), 0) && e.HasFlag(value)) {
                    yield return (T)value;
                }
            }
        }

        public static long Seek(this Stream s, long position) {
            return s.Seek(position, SeekOrigin.Begin);
        }
        
        public static void SetSplitPosition(this PropertyGrid propertyGrid, int position) {
            try {
                object gridView = propertyGrid.GetType().GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(propertyGrid, new object[] { });
                gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(gridView, new object[] { position });
            } catch { }
        }

        public static int GetMaximumValue(this ScrollBar scrollBar) {
            return scrollBar.Maximum - scrollBar.LargeChange + 1;
        }

        public static T JsonClone<T>(this T obj, params JsonConverter[] converters) {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json, converters);
        }

        public static int DistanceFrom(this Point point, Rectangle rect) {
            int xDist = Math.Max(0, Math.Max(rect.X - point.X, point.X - rect.Right));
            int yDist = Math.Max(0, Math.Max(rect.Y - point.Y, point.Y - rect.Bottom));
            return xDist + yDist;
        }

        public static Point GetCenter(this Rectangle rect) {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static void DrawRectangleProper(this Graphics g, Pen pen, Rectangle rect) {
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        public static void UnAutoHide(this DockContent dockContent) {
            switch (dockContent.DockState) {
                case DockState.DockBottomAutoHide:
                    dockContent.DockState = DockState.DockBottom;
                    break;
                case DockState.DockLeftAutoHide:
                    dockContent.DockState = DockState.DockLeft;
                    break;
                case DockState.DockRightAutoHide:
                    dockContent.DockState = DockState.DockRight;
                    break;
                case DockState.DockTopAutoHide:
                    dockContent.DockState = DockState.DockTop;
                    break;
            }
        }

        public delegate void SwapDelegate(int x1, int y1, int x2, int y2);

        public static void Flip(bool horizontal, int startX, int startY, int width, int height, SwapDelegate swap) {
            if (width <= 0 || height <= 0) {
                return;
            }
            int endX = startX + (width - 1) / (horizontal ? 2 : 1) + 1;
            int endY = startY + (height - 1) / (horizontal ? 1 : 2) + 1;
            for (int y = startY; y < endY; y++) {
                for (int x = startX; x < endX; x++) {
                    int otherX = horizontal ? width - (x - startX) + startX - 1 : x;
                    int otherY = horizontal ? y : height - (y - startY) + startY - 1;
                    swap(x, y, otherX, otherY);
                }
            }
        }

        public static void Flip<T>(this T[,] array, bool horizontal, int startX, int startY, int width, int height, Func<T, T> transform) {
            Flip(horizontal, startX, startY, width, height, (x1, y1, x2, y2) => {
                T element = array[x1, y1];
                T other = array[x2, y2];
                array[x1, y1] = transform(other);
                array[x2, y2] = transform(element);
            });
        }
    }
}
