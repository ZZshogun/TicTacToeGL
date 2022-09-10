using System.Drawing.Imaging;
using System.Drawing;
using System.Resources;

namespace TicTacToe
{
    class CustomFilestream
    {
        public static string ReadFromResources(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes)?[1..] ?? "";
        }

        // Windows Only
        public static BitmapData LoadImageBMP(string path)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            if (path == "" || path == null) return new();
            Bitmap bmp = new(path);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            bmp.UnlockBits(bmpData);
            return bmpData;
            #pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
