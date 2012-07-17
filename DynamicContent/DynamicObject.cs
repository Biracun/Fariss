using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
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
    public class DynamicObject : GameObject
    {
        public DynamicObject()
            : base()
        {
            IgnoreIDs = new List<int>();
            Health = 100;
            MaxHealth = 100;
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public int ID
        {
            get;
            set;
        }

        public List<int> IgnoreIDs
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public virtual double Health
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public virtual double MaxHealth
        {
            get;
            set;
        }
    }
}
