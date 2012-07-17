using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Platformer
{
    class BouncySpikeball : Enemy
    {
        protected Fixture _body;
        bool left = false;

        public BouncySpikeball()
             : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _body = FixtureFactory.CreateEllipse(_world, Dimensions.X / 2.0f, Dimensions.Y / 2.0f, 16, 1, new Vector2(Position.X + _offset, Position.Y));
            _body.Body.BodyType = BodyType.Dynamic;
            _fixtures.Add(_body);
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (_fixtures.Count > 0)
            {
                if ((X < MinX && !left) || (X > MaxX && left))
                {
                    _fixtures.Last().Body.AngularVelocity = 0;
                    Torque = -Torque;
                    left = !left;
                }
                _fixtures.Last().Body.ApplyTorque(Torque);
            }
        }

        public bool onCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Torque = -Torque;
            _fixtures.Last().Body.AngularVelocity = 0;

            return true;
        }

        public float Torque
        {
            get;
            set;
        }

        public float MinX
        {
            get;
            set;
        }

        public float MaxX
        {
            get;
            set;
        }
    }
}
