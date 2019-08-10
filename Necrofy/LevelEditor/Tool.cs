using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class Tool
    {
        protected readonly LevelEditor editor;

        public Tool(LevelEditor editor) {
            this.editor = editor;
        }

        public abstract ObjectType objectType { get; }
        public virtual void Paint(Graphics g) { }
        public virtual void MouseDown(MouseEventArgs e) { }
        public virtual void MouseUp(MouseEventArgs e) { }
        public virtual void MouseMove(MouseEventArgs e) { }
        public virtual void KeyDown(KeyEventArgs e) { }
        public virtual void KeyUp(KeyEventArgs e) { }

        public enum ObjectType
        {
            Tiles,
            Sprites,
        }
    }
}
