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
    partial class TitleEditor : EditorWindow
    {
        public readonly LoadedLevel level;

        public TitleEditor(LoadedLevel level, Project project) {
            InitializeComponent();

            this.level = level;
            Title = "[Title] " + level.Level.displayName;

            LoadedLevelTitleCharacters characters = new LoadedLevelTitleCharacters(project);
            pageEditor1.LoadPage(level.Level.title1, characters);
            pageEditor2.LoadPage(level.Level.title2, characters);
        }

        public override int? LevelNumber => level.levelAsset.LevelNumber;
    }
}
