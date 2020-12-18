using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class SpriteEditor : EditorWindow
    {
        private readonly LoadedSprites loadedSprites;
        private readonly SpriteEditorObjectBrowserContents browserContents;

        public SpriteEditor(LoadedSprites loadedSprites) {
            this.loadedSprites = loadedSprites;
            Title = "Sprites";

            browserContents = new SpriteEditorObjectBrowserContents(loadedSprites);
            BrowserContents = browserContents;
        }
    }
}
