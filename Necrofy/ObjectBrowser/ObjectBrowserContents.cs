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
        // TODO: Store the browser scroll position here
        /// <summary>Enumerates all objects to be displayed</summary>
        public abstract IEnumerable<ObjectBrowserObject> Objects { get; }
        /// <summary>Paints the object at the given index</summary>
        /// <param name="i">The index of the object to paint</param>
        /// <param name="g">The graphics used to paint the object</param>
        /// <param name="x">The x position at which to paint</param>
        /// <param name="y">The y position at which to paint</param>
        public abstract void PaintObject(int i, Graphics g, int x, int y);
        /// <summary>Invoked when there is a change to the list of objects</summary>
        public event EventHandler<ObjectsChangedEventArgs> ObjectsChanged;
        protected void RaiseObjectsChangedEvent(bool scrollToTop) {
            ObjectsChanged?.Invoke(this, new ObjectsChangedEventArgs(scrollToTop));
        }

        /// <summary>Invoked when there is a change to the selected object</summary>
        public event EventHandler SelectedIndexChanged;

        private int selectedIndex = -1;
        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                selectedIndex = value;
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int ScrollPosition { get; set; }
    }

    public class ObjectsChangedEventArgs : EventArgs
    {
        public bool ScrollToTop { get; private set; }

        public ObjectsChangedEventArgs(bool scrollToTop) {
            ScrollToTop = scrollToTop;
        }
    }
}
