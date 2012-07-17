using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class GameObject
    {
        [ContentSerializerIgnore]
        public List<Fixture> _contactFixtures;
        [ContentSerializerIgnore]
        public List<Texture2D> _textures;
        protected World _world;
        protected List<Fixture> _fixtures;
        protected List<FarseerPhysics.Dynamics.Joints.Joint> _joints;
        protected Texture2D _texture;
        protected Vector2 _dimensions;
        protected Vector2 _position;
        protected bool _init;
        protected float _offset;
        [ContentSerializerIgnore]
        public SpriteFont _defaultFont;
        [ContentSerializerIgnore]
        public ContentManager _content;

        public GameObject()
        {
            _fixtures = new List<Fixture>();
            _contactFixtures = new List<Fixture>();
            IgnoreFixtures = new List<Fixture>();
            _joints = new List<FarseerPhysics.Dynamics.Joints.Joint>();
            _init = false;
        }

        ~GameObject()
        {
            Dispose();
        }

        public virtual void Create(World world, float Xoffset)
        {
            Dispose();
            _world = world;
            _init = true;
            _offset = Xoffset;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
                                realY = currentY;// -((textureHeight + currentX - X) * sinTheta);
                                Vector2 origin = new Vector2((width / 2.0f) - (currentX - X), height / 2.0f - (currentY - Y));
                                spriteBatch.Draw(_textures[Textures[0]], new Vector2(realX, realY) + origin, new Rectangle(0, 0, (int)(Xleft < textureWidth ? Xleft : textureWidth), (int)(Yleft < textureHeight ? Yleft : textureHeight)), Color.White, -fixture.Body.Rotation, origin, 1, SpriteEffects.None, 0);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void Dispose()
        {
            _init = false;
            foreach (Fixture fixture in _fixtures)
            {
                _world.RemoveBody(fixture.Body);
            }
            _fixtures.Clear();
        }

        public List<int> Textures
        {
            get;
            set;
        }

        public TextureMode TextureMode
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public virtual Fixture MainFixture
        {
            get
            {
                return _fixtures[0];
            }
        }

        [ContentSerializerIgnore]
        public virtual SaveGame GameSave
        {
            get;
            set;
        }

        public virtual Vector2 Position
        {
            get
            {
                if (_fixtures.Count <= 0)
                    return _position;
                else
                    return _fixtures[0].Body.Position;
            }
            set
            {
                if (_fixtures.Count <= 0)
                    _position = value;
                else
                {
                    Vector2 difference = value - _fixtures[0].Body.Position;
                    foreach (Fixture fixture in _fixtures)
                    {
                        fixture.Body.Position += difference;
                    }
                }
            }
        }

        [ContentSerializerIgnore]
        public List<Fixture> Fixtures
        {
            get
            {
                return _fixtures;
            }
        }

        [ContentSerializerIgnore]
        public virtual float X
        {
            get
            {
                if (_fixtures.Count <= 0)
                    return _position.X;
                else
                    return _fixtures[0].Body.Position.X;
            }
            set
            {
                if (_fixtures.Count <= 0)
                    _position.X = value;
                else
                {
                    float difference = value - _fixtures[0].Body.Position.X;
                    foreach (Fixture fixture in _fixtures)
                    {
                        fixture.Body.Position += new Vector2(difference, 0);
                    }
                }
            }
        }
        
        [ContentSerializerIgnore]
        public virtual float Y
        {
            get
            {
                if (_fixtures.Count <= 0)
                    return _position.Y;
                else
                    return _fixtures[0].Body.Position.Y;
            }
            set
            {
                if (_fixtures.Count <= 0)
                    _position.Y = value;
                else
                {
                    float difference = value - _fixtures[0].Body.Position.Y;
                    foreach (Fixture fixture in _fixtures)
                    {
                        fixture.Body.Position += new Vector2(0, difference);
                    }
                }
            }
        }

        public virtual Vector2 Dimensions
        {
            get
            {
                return _dimensions;
            }
            set
            {
                _dimensions = value;
            }
        }

        [ContentSerializerIgnore]
        public bool Initialised
        {
            get
            {
                return _init;
            }
            set
            {
                _init = value;
            }
        }

        public bool CollideWithPlayer
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Fixture> IgnoreFixtures
        {
            get;
            set;
        }
    }

    public enum TextureMode
    {
        NONE,
        STRETCH,
        TILE
    }
}