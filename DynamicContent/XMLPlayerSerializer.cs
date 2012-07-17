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
    public class XMLPlayerSerializer
    {
        public XMLPlayerSerializer()
        {

        }

        public float Health
        {
            get;
            set;
        }

        public float MaxHealth
        {
            get;
            set;
        }

        public Vector2 WorldPosition
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public List<Collectables> Collectables
        {
            get;
            set;
        }
    }

    public enum Collectables
    {
        HEART,
        JUMPHEIGHT
    }
}
