using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteObjectBrowserContents : ObjectBrowserContents
    {
        private readonly LoadedSpriteGraphics spriteGrahpics;
        private readonly HashSet<SpriteDisplay.Category> categories = new HashSet<SpriteDisplay.Category>();
        private readonly List<LoadedSpriteGraphics.LoadedSprite> sprites = new List<LoadedSpriteGraphics.LoadedSprite>();
        
        public SpriteObjectBrowserContents(LoadedSpriteGraphics spriteGrahpics) {
            this.spriteGrahpics = spriteGrahpics;
        }

        public void AddCategory(SpriteDisplay.Category category) {
            if (categories.Add(category)) {
                UpdateSpriteList();
            }
        }

        public void RemoveCategory(SpriteDisplay.Category category) {
            if (categories.Remove(category)) {
                UpdateSpriteList();
            }
        }

        private void UpdateSpriteList() {
            // TODO: update selected index
            sprites.Clear();
            foreach (SpriteDisplay.Category category in categories.OrderBy(c => c)) {
                sprites.AddRange(spriteGrahpics.spritesByCategory[category]);
            }
            RaiseObjectsChangedEvent();
        }

        public override IEnumerable<Size> Objects {
            get {
                foreach (LoadedSpriteGraphics.LoadedSprite sprite in sprites) {
                    yield return sprite.Size;
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            sprites[i].RenderFromTopCorner(g, x, y);
        }
    }
}
