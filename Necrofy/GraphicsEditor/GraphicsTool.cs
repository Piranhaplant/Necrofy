using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class GraphicsTool
    {
        protected readonly GraphicsEditor editor;

        public GraphicsTool(GraphicsEditor editor) {
            this.editor = editor;
        }

        public virtual void MouseDown(MouseEventArgs e) { }
        public virtual void MouseUp(MouseEventArgs e) { }
        public virtual void MouseMove(MouseEventArgs e) { }

        public virtual void DoneBeingUsed() { }

        private string status = "";
        public string Status {
            get {
                return status;
            }
            protected set {
                status = value;
                editor.SetStatus(status);
            }
        }
    }
}
