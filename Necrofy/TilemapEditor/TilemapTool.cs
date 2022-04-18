using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class TilemapTool : MapTool
    {
        protected readonly TilemapEditor editor;

        public TilemapTool(TilemapEditor editor) : base(editor) {
            this.editor = editor;
        }
    }
}
