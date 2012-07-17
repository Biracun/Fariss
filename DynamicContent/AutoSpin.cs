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
    public class AutoSpin : DynamicObject
    {
        float _rotation = 0;
        float _friction = 0.35f;
        float _restitution = 0.5f;

        Fixture blade;
        Fixture pivot;

        public AutoSpin()
            : base()
        {

        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            blade = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0));
            blade.Body.BodyType = BodyType.Dynamic;
            blade.Restitution = Restitution;
            blade.Friction = Friction;
            _fixtures.Add(blade);
            pivot = FixtureFactory.CreateRectangle(_world, 0.1f, 0.1f, 1, blade.Body.Position);
            blade.CollisionFilter.IgnoreCollisionWith(pivot);
            pivot.CollisionFilter.IgnoreCollisionWith(blade);
            _joints.Add(JointFactory.CreateRevoluteJoint(_world, blade.Body, pivot.Body, Vector2.Zero));
            _fixtures.Add(pivot);
            pivot.UserData = this;
            blade.UserData = this;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            blade.Body.AngularVelocity = RotationSpeed;
        }

        public float RotationSpeed
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
                _rotation = value;
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
