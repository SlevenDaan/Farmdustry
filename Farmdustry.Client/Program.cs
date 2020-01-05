using System;

namespace Farmdustry.Client
{
    #if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new FarmdustryClient())
                game.Run();
        }
    }
    #endif
}
