using System;

namespace HKU_Y2_Graphics_Programming
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using(var game = new Game1())
                game.Run();
        }
    }
}
