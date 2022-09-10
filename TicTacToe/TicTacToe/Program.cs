using OpenTK.Mathematics;

namespace TicTacToe {
    class Program
    {
        static void Main()
        {
            Game game = new(900, 1000, LogEnabled: false);
            game.Run();
        }
    }
}