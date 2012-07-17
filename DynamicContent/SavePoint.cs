﻿using System;
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
    public class SavePoint : GameObject
    {
        protected float _rotation;

        public SavePoint()
            : base()
        {
            Activated = false;
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, new Vector2(Position.X + _offset, Position.Y)));
            _fixtures.Last().UserData = this;
            _fixtures.Add(FixtureFactory.CreateRectangle(_world, Dimensions.X, 3, 1, new Vector2(Position.X, Position.Y + 1.5f)));
            _fixtures.Last().IsSensor = true;
            _fixtures.Last().UserData = this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float width, height, X, Y;

            switch (TextureMode)
            {
                case TextureMode.STRETCH:
                    width = Dimensions.X * 32;
                    height = Dimensions.Y * 32;
                    X = Position.X * 32 + 640;
                    Y = -Position.Y * 32 + 360;

                    if (Activated)
                    {
                        Y -= 48;
                        height = 96;
                        spriteBatch.Draw(_textures[Textures[1]], new Rectangle((int)(X), (int)(Y), (int)width, (int)height), null, Color.White, -Rotation, new Vector2(_textures[Textures[1]].Width / 2.0f, _textures[Textures[1]].Height / 2.0f), SpriteEffects.None, 0);
                    }

                    height = Dimensions.Y * 32;
                    Y = -Position.Y * 32 + 360;

                    spriteBatch.Draw(_textures[Textures[0]], new Rectangle((int)(X), (int)(Y), (int)width, (int)height), null, Color.White, -Rotation, new Vector2(_textures[Textures[0]].Width / 2.0f, _textures[Textures[0]].Height / 2.0f), SpriteEffects.None, 0);


                    break;
                case TextureMode.TILE:
                    width = Dimensions.X * 32;
                    height = Dimensions.Y * 32;
                    float textureWidth = _textures[Textures[0]].Width;
                    float textureHeight = _textures[Textures[0]].Height;
                    X = Position.X * 32 + 640 - (width / 2.0f);
                    Y = -Position.Y * 32 + 360 - (height / 2.0f);

                    float realX = X;
                    float realY = Y;

                    for (float currentY = Y; currentY < Y + height; currentY += textureHeight)
                    {
                        float Yleft = Y + height - currentY;
                        for (float currentX = X; currentX < X + width; currentX += textureWidth)
                        {
                            float Xleft = X + width - currentX;
                            realX = currentX;
                            realY = currentY;// -((textureHeight + currentX - X) * sinTheta);
                            Vector2 origin = new Vector2((width / 2.0f) - (currentX - X), height / 2.0f);
                            spriteBatch.Draw(_textures[Textures[0]], new Vector2(realX, realY) + origin, new Rectangle(0, 0, (int)(Xleft < textureWidth ? Xleft : textureWidth), (int)(Yleft < textureHeight ? Yleft : textureHeight)), Color.White, -Rotation, origin, 1, SpriteEffects.None, 0);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        [ContentSerializerIgnore]
        public bool Activated
        {
            get;
            set;
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = 0;
            }
        }
    }
}
