﻿using System;
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
        private readonly List<LoadedLevelSprites.LoadedSprite> sprites = new List<LoadedLevelSprites.LoadedSprite>();
        private readonly List<SpriteDisplay.Category> categoryForSprites = new List<SpriteDisplay.Category>();
        private HashSet<SpriteDisplay.Category> highlighedCategories = new HashSet<SpriteDisplay.Category>();

        public SpriteObjectBrowserContents(LoadedLevel level) {
            this.level = level;
            level.SpritesChanged += Level_SpritesChanged;
        }

        private void Level_SpritesChanged(object sender, EventArgs e) {
            UpdateSpriteList(scrollToTop: false);
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
                RaiseObjectsChangedEvent(scrollToTop: false);
            }
        }

        public void SetSelectedSprite(SpriteDisplay.Category category, int type) {
            for (int i = 0; i < sprites.Count; i++) {
                if (categoryForSprites[i] == category && sprites[i].Key.value == type) {
                    SelectedIndex = i;
                    break;
                }
            }
        }

        private void UpdateSpriteList(bool scrollToTop = true) {
            SpriteDisplay.Key oldSelectedSprite = SelectedSprite;
            sprites.Clear();
            categoryForSprites.Clear();

            foreach (SpriteDisplay.Category category in categories.OrderBy(c => c)) {
                List<LoadedLevelSprites.LoadedSprite> s = level.spriteGraphics.spritesByCategory[category];
                sprites.AddRange(s);
                categoryForSprites.AddRange(s.Select(o => category));
            }

            if (oldSelectedSprite != null) {
                SelectedIndex = sprites.FindIndex(s => s.Key == oldSelectedSprite);
            }
            RaiseObjectsChangedEvent(scrollToTop);
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
