using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    /// <summary>Provides information for displaying a particular type of object in an <see cref="ObjectBrowser"/></summary>
    public abstract class ObjectBrowserContents
    {
        /// <summary>Enumerates all objects to be displayed</summary>
        public abstract IEnumerable<Size> Objects { get; }
        /// <summary>Paints the object at the given index</summary>
        /// <param name="i">The index of the object to paint</param>
        /// <param name="g">The graphics used to paint the object</param>
        /// <param name="x">The x position at which to paint</param>
        /// <param name="y">The y position at which to paint</param>
        /// <returns>A boolean indicating whether the contents need to be laid out again</returns>
        public abstract bool PaintObject(int i, Graphics g, int x, int y);
        /// <summary>Gets whether objects will be displayed as a list instead of stacking them horizontally.</summary>
        public virtual bool ListLayout {
            get {
                return false;
            }
        }
        /// <summary>Invoked when there is a change to the contents</summary>
        public event EventHandler Changed;
        protected void RaiseChangedEvent() {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private int selectedIndex = -1;
        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                if (selectedIndex != value) {
                    selectedIndex = value;
                    ObjectSelected(selectedIndex);
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>Callback for when an object is selected</summary>
        /// <param name="i">The index of the selected object</param>
        protected abstract void ObjectSelected(int i);
    }
}
