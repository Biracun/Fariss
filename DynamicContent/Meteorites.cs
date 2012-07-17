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
    public class Meteorites : DynamicObject
    {
        Random _generator;
        List<Meteorite> _meteorites;

        double time = 0;

        public Meteorites()
            : base()
        {
            _generator = new Random();
            _meteorites = new List<Meteorite>();
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            time += gameTime.ElapsedGameTime.TotalMilliseconds;

            for (int i = 0; i < _meteorites.Count; i++)
            {
                _meteorites[i].Update(gameTime);
                if (!_meteorites[i].Initialised)
                {
                    _meteorites.Remove(_meteorites[i]);
                    i--;
                    continue;
                }
            }

            if (time > 0 + 3000 * _generator.NextDouble())
            {
                time = 0;
                _meteorites.Add(new Meteorite() { Radius = 0.5f + (float)_generator.NextDouble() / 4.0f, Teeth = _generator.Next(4, 6), Position = new Vector2(40.0f * (float)_generator.NextDouble() - 20.0f + _offset, 0) });
                _meteorites.Last().Create(_world, _offset);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
