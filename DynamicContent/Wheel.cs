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
    public class Wheel : DynamicObject
    {
        float _rotation = 0;
        float _friction = 0.35f;
        float _restitution = 0.5f;

        public Wheel()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateEllipse(_world, Dimensions.X / 2.0f, Dimensions.Y / 2.0f, 16, 1, new Vector2(Position.X + _offset, Position.Y)));
            _fixtures.Last().Body.BodyType = BodyType.Dynamic;
            _fixtures.Last().Restitution = Restitution;
            _fixtures.Last().Friction = Friction;
            _fixtures.Last().Body.Rotation = _rotation;
            _fixtures.Last().UserData = this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_fixtures.Count > 0)
                spriteBatch.Draw(_textures[Textures[0]], new Rectangle((int)(X), (int)(Y), (int)Dimensions.X, (int)Dimensions.Y), null, Color.White, -_fixtures[0].Body.Rotation, new Vector2(_textures[Textures[0]].Width / 2.0f, _textures[Textures[0]].Height / 2.0f), SpriteEffects.None, 0);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public float Rotation
        {
            get
            {
                if (_fixtures.Count <= 0)
                {
                    return _rotation;
                }
                else
                {
                    return _fixtures[0].Body.Rotation;
                }
            }
            set
            {
                if (_fixtures.Count <= 0)
                {
                    _rotation = value;
                }
                else
                {
                    _fixtures[0].Body.Rotation = value;
                }
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
    }
}
