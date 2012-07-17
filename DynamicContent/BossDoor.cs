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
using FarseerPhysics.Factories;

namespace Platformer
{
    class BossDoor : Parent
    {
        protected Fixture _door;

        public BossDoor()
            : base()
        {
        }

        public override void Create(FarseerPhysics.Dynamics.World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _door = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1000000, Position + new Vector2(Xoffset, 0));
            JointFactory.CreateFixedAngleJoint(_world, _door.Body);
            _door.Body.BodyType = BodyType.Dynamic;
            _fixtures.Add(_door);
            _door.Restitution = -1.0f;
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _door.Body.Position = new Vector2(_position.X + _offset, _door.Body.Position.Y);

            bool allButtonsPressed = true;
            bool anyButtonPressed = false;
            foreach (DynamicObject child in Children)
            {
                if (child is Enemy)
                {
                    if (child.Health <= 0)
                    {
                        anyButtonPressed = true;
                    }
                    else
                    {
                        allButtonsPressed = false;
                    }
                }
            }
            if ((allButtonsPressed && AllTriggersRequired) || (anyButtonPressed && !AllTriggersRequired))
            {
                _door.Body.LinearVelocity = new Vector2(0, 5);
            }
            if (_door.Body.Position.Y > MaxPosition.Y)
            {
                _door.Body.Position = new Vector2(MaxPosition.X, MaxPosition.Y);
                _door.Body.LinearVelocity = new Vector2(0, 0);
            }
        }

        public Vector2 MaxPosition
        {
            get;
            set;
        }

        public bool AllTriggersRequired
        {
            get;
            set;
        }
    }
}
