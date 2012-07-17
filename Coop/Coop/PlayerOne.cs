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
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class PlayerOne : Player
    {
        public float WIDTH = 1.0f;
        public float HEIGHT = 1.5f;
        public float PUSHSCALE = 0.5f;

        private double _bulletTimer = 0;

        protected SoundEffect _shooting;

        public PlayerOne()
            : base()
        {
            Colour = Color.White;
            ID = 1;
            PlayerIndex = PlayerIndex.One;
            _dynamicObjects = new List<DynamicObject>();
            BulletSpeed = 35;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (DynamicObject dynamicObject in _dynamicObjects)
            {
                dynamicObject.Draw(gameTime, spriteBatch);
            }

            base.Draw(gameTime, spriteBatch);
        }

        public void Create(World world)
        {
            _shooting = _content.Load<SoundEffect>("Sounds\\shoot");
            base.Create(world, 0);

            _gamePadStatePrevious = GamePad.GetState(PlayerIndex.Two);
        }

        public override void Update(GameTime gameTime)
        {
            _gamePadState = GamePad.GetState(PlayerIndex);
            _bulletTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            for (int i = _dynamicObjects.Count - 1; i >= 0; i--)
            {
                DynamicObject dynamicObject = _dynamicObjects[i];
                if (!dynamicObject.Initialised)
                    _dynamicObjects.Remove(dynamicObject);
                else
                    dynamicObject.Update(gameTime);
            }

            if (GamePad.GetState(PlayerIndex).IsButtonDown(Buttons.X) || GamePad.GetState(PlayerIndex).IsButtonDown(Buttons.RightShoulder))
            {
                if (_bulletTimer > 100)
                {
                    _shooting.Play();
                    _dynamicObjects.Add(new Bullet() { Player = true, LifeTime = 1500, Position = this.Position, Dimensions = new Vector2(0.1f, 0.1f), Speed = (Direction == Direction.RIGHT ? 1 : -1) * BulletSpeed, TextureMode = TextureMode.STRETCH, Textures = new List<int>() { 1 }, _textures = _textures });
                    _dynamicObjects.Last().Create(_world, 0);
                    _dynamicObjects.Last().MainFixture.CollisionFilter.IgnoreCollisionWith(MainFixture);
                    _bulletTimer = 0;
                }
            }

            base.Update(gameTime);

            _gamePadStatePrevious = GamePad.GetState(PlayerIndex);
        }

        public float BulletSpeed
        {
            get;
            set;
        }
    }
}
