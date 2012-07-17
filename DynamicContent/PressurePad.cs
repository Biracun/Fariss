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
    class PressurePad : Button
    {
        public PressurePad()
            : base()
        {

        }

        public override void Create(FarseerPhysics.Dynamics.World world, float Xoffset)
        {
            base.Create(world, Xoffset);
            _button.OnCollision += new OnCollisionEventHandler(OnCollision);
            _button.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Textures.Count > 1 && _fixtures.Count > 0)
            {
                if (Pressed)
                {
                    spriteBatch.Draw(_textures[Textures[1]], new Vector2(MainFixture.Body.Position.X * 32 + 640, (-MainFixture.Body.Position.Y * 32 + 360) - 0.5f * (96)), null, Color.White, 0, new Vector2(48, 48), 1, SpriteEffects.None, 0);
                }
            }

            base.Draw(gameTime, spriteBatch);

            if (Textures.Count > 2 && _fixtures.Count > 0)
            {
                spriteBatch.Draw(_textures[Textures[2]], new Vector2(MainFixture.Body.Position.X * 32 + 640, (-MainFixture.Body.Position.Y * 32 + 360) - 0.5f * (Dimensions.Y + 120)), null, Color.White, 0, new Vector2(48, 48), 1, SpriteEffects.None, 0);
            }
        }

        public virtual bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            Vector2 normal = Vector2.Zero;

            Contact currentContact = contact;
            Vector2 nextNormal = Vector2.Zero;
            FixedArray2<Vector2> fx; // No idea what that is, but the function wants it
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
                Pressed = true;

                _pressing.Add(fixtureB);
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
                    Pressed = false;
                }
            }
        }
    }
}
