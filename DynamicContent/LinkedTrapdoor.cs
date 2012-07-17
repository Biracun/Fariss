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
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class LinkedTrapdoor : Parent
    {
        List<Fixture> _pressing;
        Fixture _door;
        Fixture _pivot;

        public LinkedTrapdoor()
            : base()
        {
            Reversed = false;
            Pressure = false;
            _pressing = new List<Fixture>();
        }

        public override void Create(World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _door = FixtureFactory.CreateRectangle(world, Dimensions.X, Dimensions.Y, 1, Position + new Vector2(Xoffset, 0) - new Vector2((Reversed ? -Dimensions.X : Dimensions.X) / 2.0f, 0));
            _pivot = FixtureFactory.CreateCircle(world, 0.1f, 1, Position + new Vector2(Xoffset, 0));
            JointFactory.CreateRevoluteJoint(_world, _door.Body, _pivot.Body, Vector2.Zero);
            _fixtures.Add(_door);
            _fixtures.Last().UserData = this;
            _fixtures.Add(_pivot);
            _fixtures.Last().UserData = this;
            _door.Body.BodyType = BodyType.Static;
            _pivot.Body.BodyType = BodyType.Static;
            _door.OnCollision += new OnCollisionEventHandler(OnCollision);
            _door.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public virtual bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            Vector2 normal = Vector2.Zero;

            Contact currentContact = contact;
            Vector2 nextNormal = Vector2.Zero;
            FixedArray2<Vector2> fx; // No idea what that is, but the function wants it lol
            // Iterate through the contacts, summing the normals
            do
            {
                Vector2 vec = Vector2.Zero;
                contact.GetWorldManifold(out vec, out fx);
                normal += vec;
                currentContact = currentContact.Next;
            } while (currentContact != null);

            if (normal.Y > Y && normal.X == 0)
            {
                _pressing.Add(fixtureB);
                Pressure = true;
                bool allPressed = true;
                foreach (GameObject gameObject in Children)
                {
                    if (gameObject is LinkedTrapdoor && !((LinkedTrapdoor)gameObject).Pressure)
                    {
                        allPressed = false;
                    }
                }
                if (allPressed)
                {
                    _door.Body.BodyType = BodyType.Dynamic;
                    foreach (GameObject gameObject in Children)
                    {
                        if (gameObject is LinkedTrapdoor)
                        {
                            ((LinkedTrapdoor)gameObject).Fixtures[0].Body.BodyType = BodyType.Dynamic;
                        }
                    }
                }
            }

            return true;
        }

        public virtual void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (_pressing.Contains(fixtureB))
            {
                _pressing.Remove(fixtureB);
                if (_pressing.Count == 0)
                {
                    Pressure = false;
                }
            }
        }

        [ContentSerializerIgnore]
        public bool Pressure
        {
            get;
            set;
        }

        public bool Reversed
        {
            get;
            set;
        }
    }
}
