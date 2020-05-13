using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

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
    }
}
