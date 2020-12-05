using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    }
}
