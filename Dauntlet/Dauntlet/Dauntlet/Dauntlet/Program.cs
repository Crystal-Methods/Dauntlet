using System;

namespace Dauntlet
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Dauntlet game = new Dauntlet())
            {
                game.Run();
            }
        }
    }
#endif
}

