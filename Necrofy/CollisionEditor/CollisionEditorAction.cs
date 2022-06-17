using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrofy
{
    abstract class CollisionEditorAction : UndoAction<CollisionEditor>
    {
    }

    class ChangeCollisionAction : CollisionEditorAction
    {
        private int tileNum;
        private ushort prevCollision;
        private ushort newCollision;

        public ChangeCollisionAction(int tileNum, ushort prevCollision, ushort newCollision) {
            this.tileNum = tileNum;
            this.prevCollision = prevCollision;
            this.newCollision = newCollision;
        }

        protected override void Undo() {
            editor.SetCollision(tileNum, prevCollision);
        }

        protected override void Redo() {
            editor.SetCollision(tileNum, newCollision);
        }

        public override bool Merge(UndoAction<CollisionEditor> action) {
            if (action is ChangeCollisionAction changeCollisionAction && changeCollisionAction.tileNum == tileNum) {
                newCollision = changeCollisionAction.newCollision;
                return true;
            }
            return false;
        }

        public override string ToString() {
            return $"Change collision on tile 0x{tileNum:X3}";
        }
    }
}
