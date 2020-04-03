using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteObjectBrowserContents : ObjectBrowserContents
    {
        private static readonly Brush disabledOverlayBrush = new SolidBrush(Color.FromArgb(200, SystemColors.Control));
        private static readonly Brush selectedDisabledOverlayBrush = new SolidBrush(Color.FromArgb(200, ObjectBrowserControl.selectedObjectColor));

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
            RaiseObjectsChangedEvent();
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
            RaiseObjectsChangedEvent();
        }

        public SpriteDisplay.Key SelectedSprite => SelectedIndex < 0 ? null : sprites[SelectedIndex].Key;
        public SpriteDisplay.Category? SelectedCategory => SelectedIndex < 0 ? (SpriteDisplay.Category?)null : categoryForSprites[SelectedIndex];

        public override IEnumerable<Size> Objects {
            get {
                foreach (LoadedSpriteGraphics.LoadedSprite sprite in sprites) {
                    yield return sprite.Size;
                }
            }
        }

        public override void PaintObject(int i, Graphics g, int x, int y) {
            sprites[i].RenderFromTopCorner(g, x, y);
            if (highlighedCategories != null && !highlighedCategories.Contains(categoryForSprites[i])) {
                Brush b = i == SelectedIndex ? selectedDisabledOverlayBrush : disabledOverlayBrush;
                g.FillRectangle(b, new Rectangle(new Point(x, y), sprites[i].Size));
            }
        }
    }
}
