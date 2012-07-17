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
    public class FerrisWheel : DynamicObject
    {
        float _rotation = 0;
        float _friction = 0.35f;
        float _restitution = 0.5f;

        Fixture blade;
        Fixture pivot;
        List<Fixture> blades;

        public FerrisWheel()
            : base()
        {
            blades = new List<Fixture>();
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);

            Vector2 position = Position + new Vector2(Xoffset, 0);

            blade = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, position);
            blade.Body.BodyType = BodyType.Kinematic;
            Fixture blade2 = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, position);
            blade2.Body.BodyType = BodyType.Kinematic;
            blade2.Body.Rotation += MathHelper.PiOver4;
            Fixture blade3 = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, position);
            blade3.Body.BodyType = BodyType.Kinematic;
            blade3.Body.Rotation += MathHelper.PiOver2;
            Fixture blade4 = FixtureFactory.CreateRectangle(_world, Dimensions.X, Dimensions.Y, 1, position);
            blade4.Body.BodyType = BodyType.Kinematic;
            blade4.Body.Rotation += MathHelper.PiOver2 + MathHelper.PiOver4;
            pivot = FixtureFactory.CreateRectangle(_world, 0.1f, 0.1f, 1, blade.Body.Position);
            pivot.Body.BodyType = BodyType.Static;
            blade.CollisionFilter.IgnoreCollisionWith(pivot);
            blade.CollisionFilter.IgnoreCollisionWith(blade2);
            pivot.CollisionFilter.IgnoreCollisionWith(blade);
            pivot.CollisionFilter.IgnoreCollisionWith(blade2);
            IgnoreFixtures.Add(pivot);
            _joints.Add(JointFactory.CreateRevoluteJoint(_world, blade.Body, pivot.Body, Vector2.Zero));
            _fixtures.Add(pivot);

            Fixture platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2(Dimensions.X / 2.0f, 0));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2(-Dimensions.X / 2.0f, 0));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2(0, -Dimensions.X / 2.0f));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade3.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2(0, Dimensions.X / 2.0f));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade3.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);

            // Diagonal ones
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2((float)(Dimensions.X * Math.Sin(MathHelper.PiOver4) / 2.0), (float)(Dimensions.X * Math.Cos(MathHelper.PiOver4) / 2.0)));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade2.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2((float)(-Dimensions.X * Math.Sin(MathHelper.PiOver4) / 2.0), (float)(-Dimensions.X * Math.Cos(MathHelper.PiOver4) / 2.0)));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade2.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);

            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2((float)(Dimensions.X * Math.Sin(MathHelper.PiOver4) / 2.0), (float)(-Dimensions.X * Math.Cos(MathHelper.PiOver4) / 2.0)));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade4.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);
            platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, position + new Vector2((float)(-Dimensions.X * Math.Sin(MathHelper.PiOver4) / 2.0), (float)(Dimensions.X * Math.Cos(MathHelper.PiOver4) / 2.0)));
            platform.Body.BodyType = BodyType.Dynamic;
            JointFactory.CreateFixedAngleJoint(_world, platform.Body);
            JointFactory.CreateRevoluteJoint(_world, blade4.Body, platform.Body, Vector2.Zero);
            _fixtures.Add(platform);
            IgnoreBladeCollision(platform);

            blades.Add(blade);
            blades.Add(blade2);
            blades.Add(blade3);
            blades.Add(blade4);

            foreach (Fixture fixture in _fixtures)
            {
                fixture.Body.SleepingAllowed = false;
            }

            foreach (Fixture bladeFixture in blades)
            {
                _fixtures.Add(bladeFixture);
                IgnoreFixtures.Add(bladeFixture);
                foreach (Fixture otherBlade in blades)
                {
                    if (bladeFixture != otherBlade)
                    {
                        bladeFixture.CollisionFilter.IgnoreCollisionWith(otherBlade);
                    }
                }
            }

            //pivot = FixtureFactory.CreateRectangle(_world, 0.1f, 0.1f, 1, Position + new Vector2(Xoffset, 0));
            //pivot.Body.BodyType = BodyType.Dynamic;

            //blade = FixtureFactory.CreateRectangle(_world, 15, 0.5f, 1, Position + new Vector2(Xoffset, 0));
            //blade.Body.BodyType = BodyType.Dynamic;
            //blade.Body.Rotation = Rotation;
            //blade.Restitution = Restitution;
            //blade.Friction = Friction;

            //Fixture blade2 = FixtureFactory.CreateRectangle(_world, 15, 0.5f, 1, Position + new Vector2(Xoffset, 0));
            //blade2.Body.BodyType = BodyType.Dynamic;
            //blade2.Body.Rotation = Rotation + MathHelper.PiOver4;
            //blade2.Restitution = Restitution;
            //blade2.Friction = Friction;

            //Fixture blade3 = FixtureFactory.CreateRectangle(_world, 15, 0.5f, 1, Position + new Vector2(Xoffset, 0));
            //blade3.Body.BodyType = BodyType.Dynamic;
            //blade3.Body.Rotation = Rotation + MathHelper.PiOver2;
            //blade3.Restitution = Restitution;
            //blade3.Friction = Friction;

            //Fixture blade4 = FixtureFactory.CreateRectangle(_world, 15, 0.5f, 1, Position + new Vector2(Xoffset, 0));
            //blade4.Body.BodyType = BodyType.Dynamic;
            //blade4.Body.Rotation = Rotation - MathHelper.PiOver4;
            //blade4.Restitution = Restitution;
            //blade4.Friction = Friction;

            //Fixture platform = FixtureFactory.CreateRectangle(_world, 3, 0.5f, 1, new Vector2(-7.5f, 0));
            //platform.CollisionFilter.IgnoreCollisionWith(blade);
            //platform.Body.BodyType = BodyType.Dynamic;
            //platform.Restitution = Restitution;
            //platform.Friction = Friction;
            ////JointFactory.CreateRevoluteJoint(_world, platform.Body, blade.Body, Vector2.Zero);
            ////JointFactory.CreateWeldJoint(_world, platform.Body, blade.Body, new Vector2(-7.5f, 0));

            //JointFactory.CreateWeldJoint(_world, pivot.Body, blade.Body, Vector2.Zero);
            //JointFactory.CreateWeldJoint(_world, pivot.Body, blade2.Body, Vector2.Zero);
            //JointFactory.CreateWeldJoint(_world, pivot.Body, blade3.Body, Vector2.Zero);
            //JointFactory.CreateWeldJoint(_world, pivot.Body, blade4.Body, Vector2.Zero);

            //// Blade1
            //blade.CollisionFilter.IgnoreCollisionWith(pivot);
            //blade.CollisionFilter.IgnoreCollisionWith(blade2);
            //blade.CollisionFilter.IgnoreCollisionWith(blade3);
            //blade.CollisionFilter.IgnoreCollisionWith(blade4);
            //// Blade 2
            //blade2.CollisionFilter.IgnoreCollisionWith(pivot);
            //blade2.CollisionFilter.IgnoreCollisionWith(blade);
            //blade2.CollisionFilter.IgnoreCollisionWith(blade3);
            //blade2.CollisionFilter.IgnoreCollisionWith(blade4);
            //// Blade 3
            //blade3.CollisionFilter.IgnoreCollisionWith(pivot);
            //blade3.CollisionFilter.IgnoreCollisionWith(blade);
            //blade3.CollisionFilter.IgnoreCollisionWith(blade2);
            //blade3.CollisionFilter.IgnoreCollisionWith(blade4);
            //// Blade 4
            //blade4.CollisionFilter.IgnoreCollisionWith(pivot);
            //blade4.CollisionFilter.IgnoreCollisionWith(blade);
            //blade4.CollisionFilter.IgnoreCollisionWith(blade2);
            //blade4.CollisionFilter.IgnoreCollisionWith(blade3);

            //_fixtures.Add(blade);
            //_fixtures.Add(blade2);
            //_fixtures.Add(blade3);
            //_fixtures.Add(blade4);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Initialised)
            {
                foreach (Fixture blade in blades)
                {
                    blade.Body.Rotation += RotationSpeed;
                }
            }
        }

        public void IgnoreBladeCollision(Fixture fixture)
        {
            foreach (Fixture blade in blades)
            {
                fixture.CollisionFilter.IgnoreCollisionWith(blade);
            }
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

        public float RotationSpeed
        {
            get;
            set;
        }
    }
}
