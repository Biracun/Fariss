using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
    public class Pickup : DynamicObject, IPickable
    {
        public Pickup()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            Dead = false;
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 5, Position + new Vector2(Xoffset, 0)));
            _fixtures.Last().Body.BodyType = BodyType.Dynamic;
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Dead)
                Dispose();

            if (X < -19.5)
            {
                Release();
                X = -19.5f;
            }
            else if (X > 19.5)
            {
                Release();
                X = 19.5f;
            }
        }

        public void SetHeld(float X, float Y, Vector2 velocity, Direction direction)
        {
            foreach (Fixture fixture in _contactFixtures)
            {
                ((GameObject)fixture.UserData)._contactFixtures.Remove(_fixtures.Last());

                if (fixture.UserData is PressurePad)
                {
                    ((PressurePad)fixture.UserData)._pressing.Remove(_fixtures.Last());
                }
            }

            if (_fixtures.Count > 0)
            {
                _fixtures[0].Body.AngularVelocity = 0;
                _fixtures[0].Body.Position = new Vector2(X + (direction == Direction.LEFT ? -1 : 1), Y);
                _fixtures[0].Body.LinearVelocity = velocity;
                _fixtures[0].CollisionFilter.CollisionCategories = Category.None;
            }
        }

        public void Release()
        {
            if (_fixtures.Count > 0)
            {
                _fixtures[0].CollisionFilter.CollisionCategories = Category.All;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public PlayerIndex PlayerIndex
        {
            get;
            set;
        }

        public Collectable Collectable
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool Dead
        {
            get;
            set;
        }
    }
}
