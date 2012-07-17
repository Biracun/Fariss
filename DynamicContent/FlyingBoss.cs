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
    public class FlyingBoss : Boss
    {
        protected Direction _direction = Direction.RIGHT;
        double _changeTime = 0;
        double _attackTime = 0;
        float _nextY = 0;
        Random _generator;
        List<DynamicObject> _dynamicObjects;

        public FlyingBoss()
            : base()
        {
            TouchDamage = 10;
            _dynamicObjects = new List<DynamicObject>();
        }

        public override void Create(World world, float Xoffset)
        {
            if ((GameSave.CurrentGameSwitches & GameSwitches.KILLED_SAUCER) != GameSwitches.KILLED_SAUCER)
            {
                base.Create(world, Xoffset);

                _generator = new Random();
                MaxHealth = 250;
                Health = MaxHealth;
                _fixtures.Add(FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0)));
                _fixtures.Last().Body.BodyType = BodyType.Kinematic;
                _fixtures.Last().Friction = 0;
                _fixtures.Last().UserData = this;
                _fixtures.Last().OnCollision += onCollision;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
            {
                GameSave.CurrentGameSwitches |= GameSwitches.KILLED_SAUCER;
                Dispose();
            }
            if ((GameSave.CurrentGameSwitches & GameSwitches.KILLED_SAUCER) != GameSwitches.KILLED_SAUCER)
            {
                _changeTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                _attackTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                for (int i = _dynamicObjects.Count - 1; i >= 0; i--)
                {
                    DynamicObject dynamicObject = _dynamicObjects[i];
                    if (!dynamicObject.Initialised)
                        _dynamicObjects.Remove(dynamicObject);
                    else
                        dynamicObject.Update(gameTime);
                }

                if (_fixtures.Count > 0)
                {
                    if (_direction == Direction.RIGHT)
                    {
                        _fixtures[0].Body.LinearVelocity = new Vector2(3 + (float)(Health > 25 ? (3 * MaxHealth / Health) : (3 * MaxHealth / 25)), 0);
                        if (X > 10 + _offset)
                            _direction = Direction.LEFT;
                    }
                    else
                    {
                        _fixtures[0].Body.LinearVelocity = new Vector2(-3 - (float)(Health > 25 ? (3 * MaxHealth / Health) : (3 * MaxHealth / 25)), 0);
                        if (X < -10 + _offset)
                            _direction = Direction.RIGHT;
                    }

                    if (_changeTime > 3000)
                    {
                        _nextY = _generator.Next(-4, 6);
                        _changeTime = 0;
                    }

                    if (Math.Abs(Y - _nextY) < 0.1)
                        Y = _nextY;
                    if (Y != _nextY)
                    {
                        if (Y < _nextY)
                            _fixtures[0].Body.LinearVelocity = new Vector2(_fixtures[0].Body.LinearVelocity.X, 1);
                        else
                            _fixtures[0].Body.LinearVelocity = new Vector2(_fixtures[0].Body.LinearVelocity.X, -1);
                    }

                    if (_attackTime > _generator.Next(1000, 3000))
                    {
                        _dynamicObjects.Add(new Bullet() { Player = false, LifeTime = 3500, Position = this.Position, Dimensions = new Vector2(1, 1), Speed = 5, TextureMode = TextureMode.STRETCH, Textures = new List<int>() { 44 }, _textures = _textures });
                        _dynamicObjects.Last().Create(_world, 0);
                        _dynamicObjects.Last().MainFixture.Body.LinearVelocity = new Vector2(_generator.Next(8, 14), _generator.Next(6, 14));
                        _dynamicObjects.Last().MainFixture.Body.AngularVelocity = (float)(_generator.NextDouble() * 5) + 0.1f;
                        _dynamicObjects.Last().MainFixture.CollisionFilter.IgnoreCollisionWith(MainFixture);
                        _attackTime = 0;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_fixtures.Count > 0 && _fixtures[0] != null)
            {
                Fixture fixture = Fixtures[0];
                float width, height, X, Y;

                switch (TextureMode)
                {
                    case TextureMode.STRETCH:
                        width = Dimensions.X * 32;
                        height = Dimensions.Y * 32;
                        X = fixture.Body.Position.X * 32 + 640;
                        Y = -fixture.Body.Position.Y * 32 + 360;

                        spriteBatch.Draw(_textures[Textures[0]], new Rectangle((int)(X), (int)(Y), (int)width, (int)height), null, Color.White, -fixture.Body.Rotation, new Vector2(_textures[Textures[0]].Width / 2.0f, _textures[Textures[0]].Height / 2.0f), SpriteEffects.None, 0);
                        break;
                    case TextureMode.TILE:
                        width = Dimensions.X * 32;
                        height = Dimensions.Y * 32;
                        float textureWidth = _textures[Textures[0]].Width;
                        float textureHeight = _textures[Textures[0]].Height;
                        X = fixture.Body.Position.X * 32 + 640 - (width / 2.0f);
                        Y = -fixture.Body.Position.Y * 32 + 360 - (height / 2.0f);

                        float realX = X;
                        float realY = Y;

                        for (float currentY = Y; currentY < Y + height; currentY += textureHeight)
                        {
                            float Yleft = Y + height - currentY;
                            for (float currentX = X; currentX < X + width; currentX += textureWidth)
                            {
                                float Xleft = X + width - currentX;
                                realX = currentX;
                                realY = currentY;
                                Vector2 origin = new Vector2((width / 2.0f) - (currentX - X), height / 2.0f - (currentY - Y));
                                spriteBatch.Draw(_textures[Textures[0]], new Vector2(realX, realY) + origin, new Rectangle(0, 0, (int)(Xleft < textureWidth ? Xleft : textureWidth), (int)(Yleft < textureHeight ? Yleft : textureHeight)), Color.White, -fixture.Body.Rotation, origin, 1, SpriteEffects.None, 0);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            foreach (DynamicObject dynamicObject in _dynamicObjects)
            {
                dynamicObject.Draw(gameTime, spriteBatch);
            }
        }

        public bool onCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.UserData is Bullet && ((Bullet)fixtureB.UserData).Player)
            {
                Health -= 5;
                ((Bullet)fixtureB.UserData).Dispose();
            }

            return true;
        }


        public override void Dispose()
        {
            base.Dispose();
            if (_dynamicObjects != null)
            {
                foreach (DynamicObject dObject in _dynamicObjects)
                {
                    if (dObject is Bullet)
                    {
                        ((Bullet)dObject).Dispose();
                    }
                }
            }
        }
    }
}
