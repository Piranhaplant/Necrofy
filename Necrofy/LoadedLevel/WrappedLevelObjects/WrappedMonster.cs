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
    }
}
