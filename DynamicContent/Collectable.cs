using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    [Flags]
    public enum Collectable
    {
        JUMP = 1,
        GRAB = 2,
        HEALTH = 4,
        FULLHEALTH = 8,
        COG = 16,
        BLOB = 32,
        POWER_CELL = 64
    }
}
