using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Necrofy
{
    class SpriteTool : Tool
    {
        public SpriteTool(LevelEditor editor) : base(editor) { }

        public override ObjectType objectType => ObjectType.Sprites;
    }
}
