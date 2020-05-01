﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class SpriteTool : Tool, ObjectSelector<WrappedLevelObject>.IHost
    {
        private static readonly Pen selectionBorderDashPen = new Pen(Color.Black) {
            DashOffset = 0,
            DashPattern = new float[] { 4f, 4f },
        };
        private static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 255, 255, 255));

        private readonly ObjectSelector<WrappedLevelObject> objectSelector;

        public SpriteTool(LevelEditor editor) : base(editor) {
            objectSelector = new ObjectSelector<WrappedLevelObject>(this);
        }

        public override ObjectType objectType => ObjectType.Sprites;

        public IEnumerable<WrappedLevelObject> GetObjects() {
            return editor.level.GetAllObjects(
                items: editor.ItemsEnabled,
                victims: editor.VictimsEnabled,
                oneShotMonsters: editor.OneShotMonstersEnabled,
                monsters: editor.MonstersEnabled,
                bossMonsters: editor.BossMonstersEnabled,
                players: editor.PlayersEnabled);
        }

        public void SelectionChanged() {
            editor.Repaint();
            editor.NonTileSelectionChanged();
            editor.spriteObjectBrowserContents.SetHighlightedCategories(objectSelector.GetSelectedObjects().Select(o => o.Category));
        }

        public void MoveSelectedObjects(int dx, int dy, int snap) {
            editor.undoManager.Do(new MoveSpriteAction(objectSelector.GetSelectedObjects(), dx, dy, snap));
        }
        
        public WrappedLevelObject CreateObject(int x, int y) {
            SpriteDisplay.Key selectedSprite = editor.spriteObjectBrowserContents.SelectedSprite;

            WrappedLevelObject newObject = null;
            if (x >= ushort.MinValue && y >= ushort.MinValue && x <= ushort.MaxValue && y <= ushort.MaxValue) {
                switch (editor.spriteObjectBrowserContents.SelectedCategory) {
                    case SpriteDisplay.Category.Item:
                        newObject = new WrappedItem(new Item((ushort)x, (ushort)y, (byte)selectedSprite.value), editor.level.spriteGraphics);
                        break;
                    case SpriteDisplay.Category.Monster:
                        newObject = new WrappedMonster(new Monster((ushort)x, (ushort)y, 0, 0, selectedSprite.value), editor.level.spriteGraphics);
                        break;
                    case SpriteDisplay.Category.OneShotMonster:
                        newObject = new WrappedOneShotMonster(new OneShotMonster((ushort)x, (ushort)y, 0, 0, selectedSprite.value), editor.level.spriteGraphics);
                        break;
                    case SpriteDisplay.Category.LevelMonster:
                        newObject = new WrappedPositionLevelMonster(new PositionLevelMonster((ushort)x, (ushort)y, selectedSprite.value), editor.level.spriteGraphics);
                        break;
                }
            }
            if (newObject != null) {
                editor.undoManager.Do(new AddSpriteAction(new[] { newObject }));
            }
            return newObject;
        }

        public IEnumerable<WrappedLevelObject> CloneSelection() {
            string serialized = JsonConvert.SerializeObject(SelectionToClipboard());
            List<WrappedLevelObject> clone = ClipboardToList(JsonConvert.DeserializeObject<SpriteClipboardContents>(serialized));
            editor.undoManager.Do(new AddSpriteAction(clone));
            return clone;
        }

        public void UpdateSelection() {
            objectSelector.UpdateSelection();
        }

        public override void MouseDown(LevelMouseEventArgs e) {
            objectSelector.MouseDown(e.X, e.Y);
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            objectSelector.MouseMove(e.X, e.Y);
        }

        public override void MouseUp(LevelMouseEventArgs e) {
            objectSelector.MouseUp();
            editor.undoManager.ForceNoMerge();
        }

        public override void SpriteChanged() {
            if (editor.spriteObjectBrowserContents.SelectedSprite != null) {
                editor.undoManager.Do(new ChangeSpriteTypeAction(objectSelector.GetSelectedObjects(), (SpriteDisplay.Category)editor.spriteObjectBrowserContents.SelectedCategory, editor.spriteObjectBrowserContents.SelectedSprite));
            }
        }

        public override void Paint(Graphics g) {
            Rectangle selectionRectangle = objectSelector.GetSelectionRectangle();
            if (selectionRectangle != Rectangle.Empty) {
                g.DrawRectangle(Pens.White, selectionRectangle);
                g.DrawRectangle(selectionBorderDashPen, selectionRectangle);
            }

            foreach (WrappedLevelObject obj in objectSelector.GetSelectedObjects()) {
                Rectangle bounds = obj.Bounds;
                g.FillRectangle(selectionFillBrush, bounds);
                g.DrawRectangle(Pens.White, bounds);
            }
        }

        public override bool CanCopy => objectSelector.GetSelectedObjects().Any(o => o.Removable);
        public override bool CanPaste => true;
        public override bool CanDelete => objectSelector.GetSelectedObjects().Any(o => o.Removable);
        public override bool HasSelection => true;

        public override void Copy() {
            SpriteClipboardContents contents = SelectionToClipboard();
            Clipboard.SetText(JsonConvert.SerializeObject(contents));
        }

        public override void Paste() {
            try {
                SpriteClipboardContents contents = JsonConvert.DeserializeObject<SpriteClipboardContents>(Clipboard.GetText());
                List<WrappedLevelObject> objs = ClipboardToList(contents);

                int minx = ushort.MaxValue;
                int miny = ushort.MaxValue;
                int maxx = 0;
                int maxy = 0;
                foreach (WrappedLevelObject obj in objs) {
                    minx = Math.Min(minx, obj.x);
                    miny = Math.Min(miny, obj.y);
                    maxx = Math.Max(maxx, obj.x);
                    maxy = Math.Max(maxy, obj.y);
                }

                Point center = editor.GetViewCenter();
                int dx = Math.Max(-minx, center.X - (maxx + minx) / 2);
                int dy = Math.Max(-miny, center.Y - (maxy + miny) / 2);
                foreach (WrappedLevelObject obj in objs) {
                    obj.x = (ushort)(obj.x + dx);
                    obj.y = (ushort)(obj.y + dy);
                }

                editor.undoManager.Do(new AddSpriteAction(objs));
                objectSelector.SelectObjects(objs);
            } catch (Exception) { }
        }

        private SpriteClipboardContents SelectionToClipboard() {
            SpriteClipboardContents contents = new SpriteClipboardContents();
            foreach (WrappedLevelObject obj in objectSelector.GetSelectedObjects()) {
                obj.AddToClipboard(contents);
            }
            return contents;
        }

        private List<WrappedLevelObject> ClipboardToList(SpriteClipboardContents contents) {
            List<WrappedLevelObject> objs = new List<WrappedLevelObject>();
            foreach (Item i in contents.items) {
                objs.Add(new WrappedItem(i, editor.level.spriteGraphics));
            }
            foreach (Monster m in contents.monsters) {
                objs.Add(new WrappedMonster(m, editor.level.spriteGraphics));
            }
            foreach (OneShotMonster m in contents.oneShotMonsters) {
                objs.Add(new WrappedOneShotMonster(m, editor.level.spriteGraphics));
            }
            foreach (PositionLevelMonster m in contents.bossMonsters) {
                objs.Add(new WrappedPositionLevelMonster(m, editor.level.spriteGraphics));
            }
            return objs;
        }

        public override void SelectAll() {
            objectSelector.SelectAll();
        }

        public override void SelectNone() {
            objectSelector.SelectNone();
        }

        public override void Delete() {
            editor.undoManager.Do(new DeleteSpriteAction(objectSelector.GetSelectedObjects()));
        }
    }
}
