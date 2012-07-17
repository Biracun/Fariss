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

namespace Platformer
{
    public class Level
    {
        public Level()
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach (DynamicObject dynamicObject in DynamicObjects)
            {
                dynamicObject.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Background background in Backgrounds)
            {
                spriteBatch.Draw(Background[background.ID], new Vector2(Offset * 32, 0), Color.White);
            }
            foreach (GameObject gameObject in StaticObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            foreach (GameObject gameObject in DynamicObjects )
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
        }

        public String Name
        {
            get;
            set;
        }

        public List<Background> Backgrounds
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public List<Texture2D> Background
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public float Offset
        {
            get;
            set;
        }

        public List<GameObject> StaticObjects
        {
            get;
            set;
        }

        public List<DynamicObject> DynamicObjects
        {
            get;
            set;
        }
    }

    public class LevelContentReader : ContentTypeReader<Level>
    {
        protected override Level Read(ContentReader input, Level existingInstance)
        {
            Level level = existingInstance;

            if (level == null)
            {
                level = new Level();
            }

            level.StaticObjects = input.ReadObject<List<GameObject>>();
            level.DynamicObjects = input.ReadObject<List<DynamicObject>>();

            // More stuff to come here

            return level;
        }
    }
}
