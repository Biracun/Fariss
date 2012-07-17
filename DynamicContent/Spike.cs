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
    public class Spike : Enemy
    {
        protected Fixture _body;

        public Spike()
             : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _body = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, new Vector2(Position.X + _offset, Position.Y));
            _body.Body.BodyType = BodyType.Static;
            _fixtures.Add(_body);
            _fixtures.Last().UserData = this;
            _fixtures.Add(FixtureFactory.CreateRectangle(_world, Dimensions.X, 40, 1, new Vector2(Position.X, 0)));
            _fixtures.Last().IsSensor = true;
            _fixtures.Last().OnCollision += onCollision;
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool onCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //if (_fixtures.Count > 0 && _fixtures[0].Body.BodyType == BodyType.Static && fixtureB.UserData is Player)
            //{
            //    _fixtures[0].Body.BodyType = BodyType.Dynamic;
            //    _fixtures[0].Body.SleepingAllowed = false;
            //}
            return true;
        }

        public override double TouchDamage
        {
            get
            {
                return (_fixtures.Count > 0 ? _fixtures[0].Body.LinearVelocity.Length() : 0);
            }
            set
            {
                base.TouchDamage = value;
            }
        }
    }
}
