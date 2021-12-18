using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    abstract class LevelEditorTool : MapTool
    {
        protected readonly LevelEditor editor;

        public LevelEditorTool(LevelEditor editor) : base(editor) {
            this.editor = editor;
        }

        public abstract ObjectType objectType { get; }
        
        public virtual void TileChanged() { }
        public virtual void SpriteChanged() { }
        public virtual void SpriteDoubleClicked() { }
        public virtual void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) { }
        
        private object[] propertyBrowserObjects = null;
        public object[] PropertyBrowserObjects {
            get {
                return propertyBrowserObjects;
            }
            protected set {
                propertyBrowserObjects = value;
                editor.SetPropertyBrowserObjects(propertyBrowserObjects);
            }
        }
        
        public enum ObjectType
        {
            Tiles,
            Sprites,
        }
    }
}
