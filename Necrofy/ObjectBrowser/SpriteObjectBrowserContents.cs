using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteObjectBrowserContents : ObjectBrowserContents
    {
        private readonly LoadedLevel level;
        private readonly HashSet<SpriteDisplay.Category> categories = new HashSet<SpriteDisplay.Category>();
        private readonly List<LoadedSpriteGraphics.LoadedSprite> sprites = new List<LoadedSpriteGraphics.LoadedSprite>();
        private readonly List<SpriteDisplay.Category> categoryForSprites = new List<SpriteDisplay.Category>();
        private HashSet<SpriteDisplay.Category> highlighedCategories = new HashSet<SpriteDisplay.Category>();
        
        public SpriteObjectBrowserContents(LoadedLevel level) {
            this.level = level;
            level.SpritesChanged += Level_SpritesChanged;
        }

        private void Level_SpritesChanged(object sender, EventArgs e) {
            UpdateSpriteList();
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

        public void SetHighlightedCategories(IEnumerable<SpriteDisplay.Category> categories) {
            if (!highlighedCategories.SetEquals(categories)) {
                highlighedCategories = new HashSet<SpriteDisplay.Category>(categories);
                RaiseObjectsChangedEvent();
            }
        }

        private void UpdateSpriteList() {
            // TODO: update selected index
            sprites.Clear();
            categoryForSprites.Clear();
            foreach (SpriteDisplay.Category category in categories.OrderBy(c => c)) {
                List<LoadedSpriteGraphics.LoadedSprite> s = level.spriteGraphics.spritesByCategory[category];
                sprites.AddRange(s);
                categoryForSprites.AddRange(s.Select(o => category));
            }
            RaiseObjectsChangedEvent();
        }

        public SpriteDisplay.Key SelectedSprite => SelectedIndex < 0 ? null : sprites[SelectedIndex].Key;
        public SpriteDisplay.Category? SelectedCategory => SelectedIndex < 0 ? (SpriteDisplay.Category?)null : categoryForSprites[SelectedIndex];

        public override IEnumerable<ObjectBrowserObject> Objects {
            get {
                for (int i = 0; i < sprites.Count; i++) {
                    bool enabled = highlighedCategories.Count == 0 || highlighedCategories.Contains(categoryForSprites[i]);
                    yield return new ObjectBrowserObject(sprites[i].Size, sprites[i].Description, enabled);
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            sprites[i].RenderFromTopCorner(g, x, y);
        }
    }
}
