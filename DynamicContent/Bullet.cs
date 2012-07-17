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
    public class Bullet : DynamicObject
    {
        float _rotation = 0;
        float _friction = 0;
        float _restitution = 0;
        double _life = 0;

        public Bullet()
            : base()
        {
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateRectangle(world, 0.1f, 0.1f, 1, Position + new Vector2(Xoffset, 0)));
            _fixtures.Last().Body.BodyType = BodyType.Dynamic;
            _fixtures.Last().Body.LinearVelocity = new Vector2(Speed, 0);
            _fixtures.Last().Restitution = Restitution;
            _fixtures.Last().Friction = Friction;
            _fixtures.Last().Body.Rotation = Rotation;
            _fixtures.Last().UserData = this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public float Friction
        {
            get
            {
                return _friction;
            }
            set
            {
                _friction = value;
            }
        }

        public float Restitution
        {
            get
            {
                return _restitution;
            }
            set
            {
                _restitution = value;
            }
        }

        public float Speed
        {
            get;
            set;
        }

        public override void Update(GameTime gameTime)
        {
            _life += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_life > LifeTime)
            {
                Dispose();
            }
        }

        public int LifeTime
        {
            get;
            set;
        }

        public bool Player
        {
            get;
            set;
        }
    }
}
