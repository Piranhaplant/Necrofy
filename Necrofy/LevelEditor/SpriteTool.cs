using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteTool : Tool, ObjectSelector<WrappedLevelObject>.IHost
    {
        private readonly ObjectSelector<WrappedLevelObject> objectSelector;

        public SpriteTool(LevelEditor editor) : base(editor) {
            objectSelector = new ObjectSelector<WrappedLevelObject>(this);
        }

        public override ObjectType objectType => ObjectType.Sprites;

        public IEnumerable<WrappedLevelObject> GetObjects() {
            return editor.level.GetAllObjects(items: editor.ItemsEnabled, victims: editor.VictimsEnabled, oneShotMonsters: editor.OneShotMonstersEnabled, monsters: editor.MonstersEnabled, bossMonsters: editor.BossMonstersEnabled, players: editor.PlayersEnabled);
        }

        public void SelectionChanged() {
            editor.Repaint();
            editor.NonTileSelectionChanged();
        }

        public override void MouseDown(LevelMouseEventArgs e) {
            objectSelector.MouseDown(e.X, e.Y);
        }

        public override void MouseMove(LevelMouseEventArgs e) {
            objectSelector.MouseMove(e.X, e.Y);
        }

        public override void MouseUp(LevelMouseEventArgs e) {
            objectSelector.MouseUp();
        }

        public override void Paint(Graphics g) {
            Rectangle selectionRectangle = objectSelector.GetSelectionRectangle();
            if (selectionRectangle != Rectangle.Empty) {
                g.DrawRectangle(Pens.Blue, selectionRectangle);
            }

            foreach (WrappedLevelObject obj in objectSelector.GetSelectedObjects()) {
                g.FillRectangle(Brushes.Orange, obj.Bounds);
                g.DrawRectangle(Pens.White, obj.Bounds);
            }
        }

        public override bool CanCopy => objectSelector.GetSelectedObjects().Count > 0;
        public override bool CanPaste => true;
        public override bool CanDelete => objectSelector.GetSelectedObjects().Count > 0;
        public override bool HasSelection => true;

        public override void Copy() {
            // TODO
        }

        public override void Paste() {
            // TODO
        }

        public override void SelectAll() {
            objectSelector.SelectAll();
        }

        public override void SelectNone() {
            objectSelector.SelectNone();
        }
    }
}
