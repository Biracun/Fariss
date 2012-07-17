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
    class Shield : Enemy
    {
        protected Fixture _body;
        protected double _pause;

        public Shield()
             : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            _body = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0));
            _body.Body.BodyType = BodyType.Kinematic;
            _body.Body.LinearVelocity = new Vector2(Speed, 0);
            _fixtures.Add(_body);
            _fixtures.Last().UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_pause > 0)
            {
                _pause += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            
            foreach (DynamicObject child in Children)
            {
                if (child is Button)
                {
                    if (((Button)child).Pressed)
                    {
                        _pause = 1;
                        _body.Body.LinearVelocity = Vector2.Zero;
                    }
                }
            }
            

            if (X < MinX + _offset)
            {
                X = MinX + _offset;
                _pause = 1;
                _body.Body.LinearVelocity = Vector2.Zero;
                Speed = -Speed;
            }
            else if (X > MaxX + _offset)
            {
                X = MaxX + _offset;
                _pause = 1;
                _body.Body.LinearVelocity = Vector2.Zero;
                Speed = -Speed;
            }

            if (_pause > Pause)
            {
                _pause = 0;
                _body.Body.LinearVelocity = new Vector2(Speed, 0);
            }
        }

        public Direction Direction
        {
            get;
            set;
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
