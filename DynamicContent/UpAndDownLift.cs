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
    class UpAndDownLift : Parent
    {
        protected Fixture _body;
        protected double _pause;

        public UpAndDownLift()
             : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _body = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0));
            _body.Body.BodyType = BodyType.Kinematic;
            _body.Body.LinearVelocity = new Vector2(0, Speed);
            _fixtures.Add(_body);
            _body.UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            if (Children.Count > 1)
            {
                if (Y < MaxY && Children[0] is Button && ((Button)Children[0]).Pressed)
                {
                    _body.Body.LinearVelocity = new Vector2(0, Speed);
                }
                else if (Y > MinY && Children[1] is Button && ((Button)Children[1]).Pressed)
                {
                    _body.Body.LinearVelocity = new Vector2(0, -Speed);
                }
                else
                {
                    _body.Body.LinearVelocity = Vector2.Zero;
                }
            }
            else
            {
                _body.Body.LinearVelocity = Vector2.Zero;
            }

            if (Y < MinY)
            {

                Y = MinY;
                _pause = 1;
                _body.Body.LinearVelocity = Vector2.Zero;
                Speed = -Speed;
            }
            else if (Y > MaxY)
            {
                Y = MaxY;
                _pause = 1;
                _body.Body.LinearVelocity = Vector2.Zero;
                Speed = -Speed;
            }
        }

        public double Pause
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }

        public float MinY
        {
            get;
            set;
        }

        public float MaxY
        {
            get;
            set;
        }
    }
}
