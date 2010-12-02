using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace ConsolePhoto
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(1,1);
            bool didLoad = false;
            foreach (string path in args)
                if (File.Exists(path))
                    try { Image img = Image.FromFile(path); bmp = new Bitmap(img); img.Dispose(); didLoad = true; }
                    catch { }
            if (!didLoad)
            {
                try
                {
                    bmp = BitmapFromBase64(Base64.TROLLFACE);
                    didLoad = true;
                }
                catch { }
            }
            if (didLoad)
            {
                Console.WriteLine("Please press Alt+Enter to make this full-screen.\nThis may not work on Vista/7.\nThen press any key to continue.");
                Console.ReadKey();
                Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                Console.SetWindowPosition(0, 0);
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                ConsoleColor BackingColor = ConsoleColor.Black;
                Size s = GetSize(bmp.Size, new Size(Console.WindowWidth, Console.WindowHeight));
                Bitmap b = new Bitmap(s.Width, s.Height * 2);
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                g.DrawImage(bmp, new Rectangle(new Point(0, 0), b.Size), new Rectangle(new Point(0, 0), bmp.Size), GraphicsUnit.Pixel);
                g.Dispose();
                bmp.Dispose();
                ConsoleColor[,] CC = new ConsoleColor[b.Width, b.Height];
                int pad = Console.WindowWidth - b.Width;
                int padleft = pad / 2;
                for (int x = 0; x < b.Width; x++)
                    for (int y = 0; y < b.Height; y++)
                        CC[x, y] = CCFromCol(b.GetPixel(x, y));
                for (int y = 0; y < b.Height / 2; y++)
                {
                    Console.BackgroundColor = BackingColor;
                    Console.Write("".PadLeft(padleft));
                    for (int x = 0; x < b.Width; x++)
                    {
                        Console.ForegroundColor = CC[x, y * 2];
                        Console.BackgroundColor = CC[x, y * 2 + 1];
                        Console.Write('▀');
                    }
                    Console.BackgroundColor = BackingColor;
                    Console.Write("".PadLeft(pad - padleft));
                }
                Console.CursorTop = 0;
                Console.ReadLine();
            }
        }
        static Size GetSize(Size src, int width)
        { return new Size(width, (width * src.Height) / src.Width); }
        static Size GetSize(Size src, Size max)
        { return new Size(Math.Min(max.Width, (src.Width * max.Height) / src.Height), Math.Min(max.Height, (src.Height * max.Width) / src.Width)); }
        static ConsoleColor CCFromCol(Color col)
        {
            double H, S, L, R, G, B, X, N;
            R = (double)col.R / 255d;
            G = (double)col.G / 255d;
            B = (double)col.B / 255d;
            X = Math.Max(R, Math.Max(G, B));
            N = Math.Min(R, Math.Min(G, B));
            L = (X + N) / 2d;
            if (X == N)
                S = H = 0d;
            else
            {
                S = (L < 0.5d) ? (X - N) / (X + N) : (X - N) / (2d - X - N);
                if (R == X)
                    H = (G - B) / (X - N);
                else if (G == X)
                    H = 2d + ((B - R) / (X - N));
                else /*if (B == X)*/
                    H = 4d + ((R - G) / (X - N));
            }
            if (L < 0.1d)
                return ConsoleColor.Black;
            else if (L > 0.9d)
                return ConsoleColor.White;
            if (S <= 0.05d)
                return L > 0.5d ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            if (H < 0.5d || H >= 5.5d)
                return L > 0.5d ? ConsoleColor.Red : ConsoleColor.DarkRed;
            else if (H >= 0.5d && H < 1.5d)
                return L > 0.5d ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
            else if (H >= 1.5d && H < 2.5d)
                return L > 0.5d ? ConsoleColor.Green : ConsoleColor.DarkGreen;
            else if (H >= 2.5d && H < 3.5d)
                return L > 0.5d ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
            else if (H >= 3.5d && H < 4.5d)
                return L > 0.5d ? ConsoleColor.Blue : ConsoleColor.DarkBlue;
            else if (H >= 4.5d && H < 5.5d)
                return L > 0.5d ? ConsoleColor.Magenta : ConsoleColor.DarkMagenta;
            return ConsoleColor.Black;
        }
        static Bitmap BitmapFromBase64(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            return new Bitmap(Image.FromStream(ms, true));
        }
    }
}