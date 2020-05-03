using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    abstract class LevelEditorAction : UndoAction<LevelEditor>
    {
        protected Level level;

        protected override void AfterAction() {
            editor.Repaint();
            editor.RefreshPropertyBrowser();
        }

        public override void SetEditor(LevelEditor editor) {
            base.SetEditor(editor);
            level = editor.level.Level;
        }
    }
}
