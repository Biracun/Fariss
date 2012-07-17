using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace Platformer
{
    public class SaveGame
    {
        public PlayerLoad PlayerOne
        {
            get;
            set;
        }

        public PlayerLoad PlayerTwo
        {
            get;
            set;
        }

        public double Health
        {
            get;
            set;
        }

        public GameSwitches GameSwitches
        {
            get;
            set;
        }

        [XmlIgnore]
        public GameSwitches CurrentGameSwitches
        {
            get;
            set;
        }

#if (XBOX360)
        [XmlIgnore]
        StorageDevice _storageDevice;
#endif
    }
}
