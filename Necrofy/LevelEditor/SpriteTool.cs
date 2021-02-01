using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    class SpriteTool : Tool, ObjectSelector<WrappedLevelObject>.IHost
    {
        private const string DefaultStatus = "Click to select or move. Hold shift to add to the selection. Hold alt to remove from the selection. Double click in the objects panel or hold ctrl and click on the level to create a new sprite.";
        private const string DragStatus = "Move: {0}, {1}.";

        private static readonly SolidBrush selectionFillBrush = new SolidBrush(Color.FromArgb(96, 255, 255, 255));

        private readonly ObjectSelector<WrappedLevelObject> objectSelector;

        public SpriteTool(LevelEditor editor) : base(editor) {
            objectSelector = new ObjectSelector<WrappedLevelObject>(this);
            UpdateStatus();
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

        private HashSet<WrappedLevelObject> selectedObjects = new HashSet<WrappedLevelObject>();

        public void SelectionChanged() {
            editor.Repaint();
            if (!selectedObjects.SetEquals(objectSelector.GetSelectedObjects())) {
                selectedObjects = new HashSet<WrappedLevelObject>(objectSelector.GetSelectedObjects());
                editor.NonTileSelectionChanged();
                editor.spriteObjectBrowserContents.SetHighlightedCategories(selectedObjects.Select(o => o.Category));
                PropertyBrowserObjects = selectedObjects.ToArray();
            }
        }

        public void MoveSelectedObjects(int dx, int dy) {
            editor.undoManager.Do(new MoveSpriteAction(selectedObjects, dx, dy));
            UpdateStatus();
        }

        public void SetSelectedObjectsPosition(int? x, int? y) {
            editor.undoManager.Do(new MoveSpriteAction(selectedObjects, (ushort?)x, (ushort?)y));
        }

        private WrappedLevelObject GetCreationObject(int x, int y) {
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
            return newObject;
        }
        
        public WrappedLevelObject CreateObject(int x, int y) {
            WrappedLevelObject newObject = GetCreationObject(x, y);
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
            UpdateStatus();
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            objectSelector.MouseMove(e.X, e.Y);
        }

        public override void MouseUp(LevelMouseEventArgs e) {
            objectSelector.MouseUp();
            editor.undoManager.ForceNoMerge();
            UpdateStatus();
        }

        public override void KeyDown(KeyEventArgs e) {
            objectSelector.KeyDown(e.KeyData);
        }

        ChangeSpriteTypeAction prevChangeSpriteAction1 = null;
        ChangeSpriteTypeAction prevChangeSpriteAction2 = null;

        public override void SpriteChanged() {
            prevChangeSpriteAction2 = prevChangeSpriteAction1;
            if (editor.spriteObjectBrowserContents.SelectedSprite != null) {
                prevChangeSpriteAction1 = new ChangeSpriteTypeAction(selectedObjects, (SpriteDisplay.Category)editor.spriteObjectBrowserContents.SelectedCategory, editor.spriteObjectBrowserContents.SelectedSprite.value);
                editor.undoManager.Do(prevChangeSpriteAction1);
            } else {
                prevChangeSpriteAction1 = null;
            }
        }
        
        public override void SpriteDoubleClicked() {
            Point center = editor.GetViewCenter();
            WrappedLevelObject newObject = GetCreationObject(Math.Max(0, center.X), Math.Max(0, center.Y));
            if (newObject != null) {
                editor.undoManager.Revert(prevChangeSpriteAction1);
                editor.undoManager.Revert(prevChangeSpriteAction2);
                editor.undoManager.Do(new AddSpriteAction(new[] { newObject }));

                objectSelector.SelectObjects(new[] { newObject });
                editor.Activate();
            }
        }
        
        public override void PropertyBrowserPropertyChanged(PropertyValueChangedEventArgs e) {
            string value = e.ChangedItem.Value as string;
            foreach (WrappedLevelObject o in selectedObjects) {
                o.ClearBrowsableProperties();
            }
            if (value != null) {
                value = value.Trim();
                switch (e.ChangedItem.Label) {
                    case WrappedLevelObject.XProperty:
                        objectSelector.ParsePositionChange(value, isX: true);
                        break;
                    case WrappedLevelObject.YProperty:
                        objectSelector.ParsePositionChange(value, isX: false);
                        break;
                    case WrappedLevelObject.PointerProperty:
                        if (ROMPointers.ParsePointer(value, out int pointer)) {
                            editor.undoManager.Do(new ChangeSpriteTypeAction(selectedObjects.Where(o => o.Category != SpriteDisplay.Category.Item && o.Category != SpriteDisplay.Category.Player), pointer));
                        }
                        break;
                    case WrappedItem.TypeProperty:
                        if (byte.TryParse(value, out byte type)) {
                            editor.undoManager.Do(new ChangeSpriteTypeAction(selectedObjects, SpriteDisplay.Category.Item, type));
                        }
                        break;
                    case WrappedMonster.RadiusProperty:
                        if (byte.TryParse(value, out byte radius)) {
                            editor.undoManager.Do(new ChangeMonsterRadiusAction(selectedObjects, radius));
                        }
                        break;
                    case WrappedMonster.DelayProperty:
                        if (byte.TryParse(value, out byte delay)) {
                            editor.undoManager.Do(new ChangeMonsterDelayAction(selectedObjects, delay));
                        }
                        break;
                    case WrappedOneShotMonster.ExtraProperty:
                        if (ushort.TryParse(value, out ushort extra)) {
                            editor.undoManager.Do(new ChangeOneShotMonsterExtraAction(selectedObjects, extra));
                        }
                        break;
                }
            }
        }

        public override void Paint(Graphics g) {
            objectSelector.DrawSelectionRectangle(g, zoom: editor.Zoom);

            using (Pen p = new Pen(Color.White, 1 / editor.Zoom)) {
                foreach (WrappedLevelObject obj in selectedObjects) {
                    Rectangle bounds = obj.Bounds;
                    g.FillRectangle(selectionFillBrush, bounds);
                    g.DrawRectangle(p, bounds);
                }
            }
        }

        public override bool CanCopy => selectedObjects.Any(o => o.Removable);
        public override bool CanPaste => true;
        public override bool CanDelete => selectedObjects.Any(o => o.Removable);
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
            foreach (WrappedLevelObject obj in selectedObjects) {
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
            editor.undoManager.Do(new DeleteSpriteAction(selectedObjects));
        }

        private void UpdateStatus() {
            if (objectSelector.MovingObjects) {
                Status = string.Format(DragStatus, objectSelector.TotalMoveX, objectSelector.TotalMoveY);
            } else {
                Status = DefaultStatus;
            }
        }
    }
}
