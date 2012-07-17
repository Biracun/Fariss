using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class Button : DynamicObject
    {
        protected Fixture _button;
        [ContentSerializerIgnore]
        public List<Fixture> _pressing;

        public Button()
            : base()
        {
            _pressing = new List<Fixture>();
            Pressed = false;
        }

        public override void Create(FarseerPhysics.Dynamics.World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _button = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0));
            _fixtures.Add(_button);
            _fixtures.Last().UserData = this;
        }

        public override void Dispose()
        {
            Pressed = false;

            base.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual bool Pressed
        {
            get;
            set;
        }
    }
}
