using System;

namespace GameOfLife
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameOfLife game = new GameOfLife())
            {
                game.Run();
            }
        }
    }
#endif
}

