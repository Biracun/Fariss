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
    class AutoLift : Parent
    {
        protected Fixture _body;
        protected double _pause;

        public AutoLift()
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
            base.Update(gameTime);

            bool on = true;
            foreach (DynamicObject child in Children)
            {
                if (child is Button && !((Button)child).Pressed)
                    on = false;
            }

            if (on)
            {
                if (_pause <= 0)
                    _body.Body.LinearVelocity = new Vector2(0, Speed);
                if (_pause > 0)
                {
                    _pause += gameTime.ElapsedGameTime.TotalMilliseconds;
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

                if (_pause > Pause)
                {
                    _pause = 0;
                }
            }
            else
            {
                if (_fixtures.Count > 0)
                    _fixtures.Last().Body.LinearVelocity = Vector2.Zero;
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
