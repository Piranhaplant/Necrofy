using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        private HashSet<WrappedLevelObject> prevSelectedObjects = new HashSet<WrappedLevelObject>();

        public void SelectionChanged() {
            editor.Repaint();
            if (!prevSelectedObjects.SetEquals(objectSelector.GetSelectedObjects())) {
                prevSelectedObjects = new HashSet<WrappedLevelObject>(objectSelector.GetSelectedObjects());
                editor.NonTileSelectionChanged();
                editor.spriteObjectBrowserContents.SetHighlightedCategories(objectSelector.GetSelectedObjects().Select(o => o.Category));
                PropertyBrowserObjects = objectSelector.GetSelectedObjects().ToArray();
            }
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
            List<WrappedLevelObject> clone = ClipboardToList(SelectionToClipboard().JsonClone());
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
                editor.undoManager.Do(new ChangeSpriteTypeAction(objectSelector.GetSelectedObjects(), (SpriteDisplay.Category)editor.spriteObjectBrowserContents.SelectedCategory, editor.spriteObjectBrowserContents.SelectedSprite.value));
            }
        }

        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            string value = e.ChangedItem.Value as string;
            foreach (WrappedLevelObject o in objectSelector.GetSelectedObjects()) {
                o.ClearBrowsableProperties();
            }
            if (value != null) {
                value = value.Trim();
                switch (e.ChangedItem.Label) {
                    case WrappedLevelObject.XProperty:
                        ParsePositionProperty(value, o => o.X,
                            dx => new MoveSpriteAction(objectSelector.GetSelectedObjects(), dx, 0, 1),
                            x => new MoveSpriteAction(objectSelector.GetSelectedObjects(), x, null));
                        break;
                    case WrappedLevelObject.YProperty:
                        ParsePositionProperty(value, o => o.Y,
                            dy => new MoveSpriteAction(objectSelector.GetSelectedObjects(), 0, dy, 1),
                            y => new MoveSpriteAction(objectSelector.GetSelectedObjects(), null, y));
                        break;
                    case WrappedLevelObject.PointerProperty:
                        if (ROMPointers.ParsePointer(value, out int pointer)) {
                            editor.undoManager.Do(new ChangeSpriteTypeAction(objectSelector.GetSelectedObjects().Where(o => o.Category != SpriteDisplay.Category.Item && o.Category != SpriteDisplay.Category.Player), pointer));
                        }
                        break;
                    case WrappedItem.TypeProperty:
                        if (byte.TryParse(value, out byte type)) {
                            editor.undoManager.Do(new ChangeSpriteTypeAction(objectSelector.GetSelectedObjects(), SpriteDisplay.Category.Item, type));
                        }
                        break;
                    case WrappedMonster.RadiusProperty:
                        if (byte.TryParse(value, out byte radius)) {
                            editor.undoManager.Do(new ChangeMonsterRadiusAction(objectSelector.GetSelectedObjects(), radius));
                        }
                        break;
                    case WrappedMonster.DelayProperty:
                        if (byte.TryParse(value, out byte delay)) {
                            editor.undoManager.Do(new ChangeMonsterDelayAction(objectSelector.GetSelectedObjects(), delay));
                        }
                        break;
                    case WrappedOneShotMonster.ExtraProperty:
                        if (ushort.TryParse(value, out ushort extra)) {
                            editor.undoManager.Do(new ChangeOneShotMonsterExtraAction(objectSelector.GetSelectedObjects(), extra));
                        }
                        break;
                }
            }
        }

        private void ParsePositionProperty(string value, Func<WrappedLevelObject, ushort> getter, Func<int, MoveSpriteAction> deltaMoveGetter, Func<ushort, MoveSpriteAction> absoluteMoveGetter) {
            if (int.TryParse(value, out int intValue)) {
                if (value.StartsWith("+") || value.StartsWith("-")) {
                    int min = objectSelector.GetSelectedObjects().Min(getter);
                    int max = objectSelector.GetSelectedObjects().Max(getter);
                    editor.undoManager.Do(deltaMoveGetter(Math.Max(-min, Math.Min(ushort.MaxValue - max, intValue))));
                } else if (intValue >= ushort.MinValue && intValue <= ushort.MaxValue) {
                    editor.undoManager.Do(absoluteMoveGetter((ushort)intValue));
                }
            }
        }

        public override void Paint(Graphics g) {
            objectSelector.DrawSelectionRectangle(g);

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

                int minx = objs.Min(o => o.X);
                int miny = objs.Min(o => o.Y);
                int maxx = objs.Max(o => o.X);
                int maxy = objs.Max(o => o.Y);

                Point center = editor.GetViewCenter();
                int dx = Math.Max(-minx, center.X - (maxx + minx) / 2);
                int dy = Math.Max(-miny, center.Y - (maxy + miny) / 2);
                foreach (WrappedLevelObject obj in objs) {
                    obj.X = (ushort)(obj.X + dx);
                    obj.Y = (ushort)(obj.Y + dy);
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
