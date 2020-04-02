﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class WrappedMonster : WrappedLevelObject<Monster>
    {
        public WrappedMonster(Monster monster, LoadedSpriteGraphics spriteGraphics) : base(monster, spriteGraphics) { }

        public override Rectangle Bounds => spriteGraphics.GetRectangle(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, x, y);

        public override ushort x { get => wrappedObject.x; set => wrappedObject.x = value; }
        public override ushort y { get => wrappedObject.y; set => wrappedObject.y = value; }

        public override void Render(Graphics g) {
            spriteGraphics.Render(SpriteDisplay.Key.Type.Pointer, wrappedObject.type, g, x, y);
        }

        public override bool Removable => true;

        public override void Add(Level level) {
            level.monsters.Add(wrappedObject);
        }

        public override void Remove(Level level) {
            level.monsters.Remove(wrappedObject);
        }

        public override void AddToClipboard(SpriteClipboardContents clipboard) {
            clipboard.monsters.Add(wrappedObject);
        }
    }
}
