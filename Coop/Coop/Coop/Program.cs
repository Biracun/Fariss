using System;

namespace Platformer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PlatformGame game = new PlatformGame())
            {
                game.Run();
            }
        }
    }
#endif
}

