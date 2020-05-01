using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    public class ObjectBrowserObject
    {
        public Size Size { get; private set; }
        public string Description { get; private set; }
        public bool Enabled { get; private set; }
        public Rectangle DisplayBounds { get; set; }

        public ObjectBrowserObject(Size size, string description = null, bool enabled = true) {
            Size = size;
            Description = description;
            Enabled = enabled;
        }
    }
}
