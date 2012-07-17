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
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class Meteorite : DynamicObject
    {
        double life;

        public Meteorite()
            : base()
        {
            life = 0;
        }

        public override void Create(World world, float Xoffset)
        {
            Console.WriteLine("Creating meteorite at " + Position);
            base.Create(world, Xoffset);

            List<Fixture> meteorite = FixtureFactory.CreateGear(_world, Radius, Teeth, 50, 0.1f, 1);

            Vector2 difference = Position + new Vector2(Xoffset) - meteorite[0].Body.Position;
            float force = Radius * -1000;
            foreach (Fixture fixture in meteorite)
            {
                fixture.Body.Position += difference;
                fixture.Body.BodyType = BodyType.Dynamic;
                _fixtures.Add(fixture);
            }
            foreach (Fixture fixture in _fixtures)
            {
                fixture.Body.ApplyForce(new Vector2(force, 0));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            life += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (life > 6000)
            {
                Dispose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public float Radius
        {
            get;
            set;
        }

        public int Teeth
        {
            get;
            set;
        }
    }
}
