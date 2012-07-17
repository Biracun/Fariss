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
    class Water : GameObject
    {
        float _rotation = 0;
        float _friction = 0.35f;
        float _restitution = 0.5f;

        public Water()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _fixtures.Add(FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0)));
            _fixtures.Last().Body.IsStatic = true;
            _fixtures.Last().Restitution = Restitution;
            _fixtures.Last().Friction = Friction;
            _fixtures.Last().Body.Rotation = _rotation;
            _fixtures.Last().UserData = this;
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
