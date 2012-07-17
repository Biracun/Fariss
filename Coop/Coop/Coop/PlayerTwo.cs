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
    public class PlayerTwo : Player
    {
        public float WIDTH = 1.0f;
        public float HEIGHT = 1.5f;
        public float PUSHSCALE = 0.5f;

        public PlayerTwo()
            : base()
        {
            Colour = Color.Gray;
            ID = 2;
            PlayerIndex = PlayerIndex.Two;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void Create(World world)
        {
            base.Create(world, 0);

            _gamePadStatePrevious = GamePad.GetState(PlayerIndex);
        }

        public override void Update(GameTime gameTime)
        {
            _gamePadState = GamePad.GetState(PlayerIndex);
            base.Update(gameTime);
            _gamePadStatePrevious = GamePad.GetState(PlayerIndex);
        }
    }
}
