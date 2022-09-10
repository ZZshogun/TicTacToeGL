using OpenTK.Mathematics;

namespace TicTacToe
{
    public static class Extension
    {
        public static void ForEach<T>(this T[] source, Action<int> action)
        {
            for(int i = 0; i < source.Length; i++)
            {
                action(i);
            }
        }

        public static void ForEach<T>(this T[,] source, Action<int, int> action)
        {
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    action(i, j);
                }
            }
        }

        public static int ToInt(this bool var)
        {
            return var ? 1 : 0;
        }

        public static Vector2i Floor(this Vector2 vec)
        {
            return new((int)MathF.Floor(vec.X), (int)MathF.Floor(vec.Y));
        }
    }
}
