using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class Parent : DynamicObject
    {
        public Parent()
            : base()
        {
            Children = new List<DynamicObject>();
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
        }

        [ContentSerializerIgnore]
        public List<DynamicObject> Children
        {
            get;
            set;
        }

        public List<int> ChildIDs
        {
            get;
            set;
        }
    }
}
