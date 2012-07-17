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
    public class Boss : Enemy
    {
        public Boss()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_fixtures.Last().Body.LinearVelocity.Y < -100)
                Health = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
