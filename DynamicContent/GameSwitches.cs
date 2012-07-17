using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    [Flags]
    public enum GameSwitches
    {
        GOT_THING = 1,
        GOT_COG = 2,
        GOT_BLOB = 4,
        GOT_POWER_CELL = 8,
        KILLED_SAUCER = 16
    }
}
