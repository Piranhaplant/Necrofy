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
        private readonly List<SpriteDisplay.Category> categoryForSprites = new List<SpriteDisplay.Category>();
        private HashSet<SpriteDisplay.Category> highlighedCategories;
        
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

        public void SetHighlightedCategories(IEnumerable<SpriteDisplay.Category> c) {
            highlighedCategories = new HashSet<SpriteDisplay.Category>(c);
            if (highlighedCategories.Count == 0) {
                highlighedCategories = null;
            }
            RaiseObjectsChangedEvent(scrollToTop: false);
        }

        private void UpdateSpriteList() {
            // TODO: update selected index
            sprites.Clear();
            categoryForSprites.Clear();
            foreach (SpriteDisplay.Category category in categories.OrderBy(c => c)) {
                List<LoadedSpriteGraphics.LoadedSprite> s = spriteGrahpics.spritesByCategory[category];
                sprites.AddRange(s);
                categoryForSprites.AddRange(s.Select(o => category));
            }
            RaiseObjectsChangedEvent(scrollToTop: true);
        }

        public SpriteDisplay.Key SelectedSprite => SelectedIndex < 0 ? null : sprites[SelectedIndex].Key;
        public SpriteDisplay.Category? SelectedCategory => SelectedIndex < 0 ? (SpriteDisplay.Category?)null : categoryForSprites[SelectedIndex];

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                for (int i = 0; i < sprites.Count; i++) {
                    bool enabled = highlighedCategories == null || highlighedCategories.Contains(categoryForSprites[i]);
                    yield return new ObjectBrowserObject(sprites[i].Size, sprites[i].Description, enabled);
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            sprites[i].RenderFromTopCorner(g, x, y);
        }
    }
}
