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
    public class PickupSpecial : Parent, IPickable
    {
        public PickupSpecial()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            bool spawntime = true;
            foreach (GameObject child in Children)
            {
                if (child is Boss)
                {
                    if (((Boss)child).Health > 0)
                    {
                        spawntime = false;
                    }
                }
            }
            if (GameSave != null && (GameSave.CurrentGameSwitches & GameSwitch) == GameSwitch)
                spawntime = false;
            if (spawntime)
            {
                Dead = false;
                _fixtures.Add(FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 5, Position + new Vector2(Xoffset, 0)));
                _fixtures.Last().Body.BodyType = BodyType.Dynamic;
                _fixtures.Last().UserData = this;
            }
            else
            {
                Initialised = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Initialised)
            {
                base.Update(gameTime);

                if (Dead)
                    Dispose();

                if (X < -19.5 + _offset)
                {
                    Release();
                    X = -19.5f + _offset;
                }
                else if (X > 19.5 + _offset)
                {
                    Release();
                    X = 19.5f + _offset;
                }
            }
            else
            {
                bool spawntime = true;
                foreach (GameObject child in Children)
                {
                    if (child is Boss)
                    {
                        if (((Boss)child).Health > 0)
                        {
                            spawntime = false;
                        }
                    }
                }
                if (spawntime && GameSave != null && (GameSave.CurrentGameSwitches & GameSwitch) != GameSwitch)
                    Create(_world, _offset);
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

        public GameSwitches GameSwitch
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
