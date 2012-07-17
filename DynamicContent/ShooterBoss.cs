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
    public class ShooterBoss : Enemy
    {
        public ShooterBoss()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0)));
            _fixtures.Last().Body.BodyType = BodyType.Dynamic;
            _fixtures.Last().Friction = 0;
            _fixtures.Last().UserData = this;
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
