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
    public class Bouncer : Enemy
    {
        protected Fixture _body;
        Random _generator;

        public Bouncer()
             : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _generator = new Random();

            _body = FixtureFactory.CreateEllipse(_world, Dimensions.X / 2.0f, Dimensions.Y / 2.0f, 16, 1, new Vector2(Position.X + _offset, Position.Y));
            _body.Body.BodyType = BodyType.Dynamic;
            _fixtures.Add(_body);
            _fixtures.Last().OnCollision += onCollision;
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool onCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.UserData is Bullet)
            {
                Health -= 1;
                ((Bullet)fixtureB.UserData).Dispose();
                if (Health <= 0)
                    Dispose();
            }
            else
            {
                if (_fixtures.Count > 0)
                {
                    _fixtures[0].Body.ApplyForce(new Vector2(0, _generator.Next(5000, 6000)));
                }
            }

            return true;
        }

        public new double Health
        {
            get;
            set;
        }
    }
}
