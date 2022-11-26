using System;
using System.Runtime.InteropServices;

namespace SpinningCube
{
    public class SpinningCube
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;

        private float A, B, C;

        private float cubeWidth = 10;
        private const int width = 160;
        private const int height = 44;

        private double[] zBuffer = new double[width * height];

        private char[] buffer = new char[width * height];

        private char backgroundASCIICode = ' ';

        private float incrementSpeed = 0.6f;

        private double x = 0d, y = 0d, z = 0d, ooz = 0d;

        private int distanceFromCamera = 60;

        private int xp, yp;
        private int idx;

        private float K1 = 40f;

        private double CalculateX(float i, float j, float k)
        {
            return j * Math.Sin(A) * Math.Sin(B) * Math.Cos(C) -
                   k * Math.Cos(A) * Math.Sin(B) * Math.Cos(C) +
                   j * Math.Cos(A) * Math.Sin(C) +
                   k * Math.Sin(A) * Math.Sin(C) +
                   i * Math.Cos(B) * Math.Cos(C);
        }

        private double CalculateY(float i, float j, float k)
        {
            return j * Math.Cos(A) * Math.Cos(C) +
                   k * Math.Sin(A) * Math.Cos(C) -
                   j * Math.Sin(A) * Math.Sin(B) * Math.Sin(C) +
                   k * Math.Cos(A) * Math.Sin(B) * Math.Sin(C) -
                   i * Math.Cos(B) * Math.Sin(C);
        }

        private double CalculateZ(float i, float j, float k)
        {
            return k * Math.Cos(A) * Math.Cos(B) -
                   j * Math.Sin(A) * Math.Cos(B) +
                   i * Math.Sin(B);
        }

        private void CalculateForSurface(float cubeX, float cubeY, float cubeZ, char ch )
        {
            x = CalculateX(cubeX, cubeY, cubeZ);
            y = CalculateY(cubeX, cubeY, cubeZ);
            z = CalculateZ(cubeX, cubeY, cubeZ) + distanceFromCamera;

            ooz = 1 / z;

            xp = (int)(width / 2 + K1 * ooz * x * 2);
            yp = (int)(height / 2 + K1 * ooz * y);

            idx = xp + yp * width;

            if (idx >= 0 && idx < width * height)
            {
                if (ooz > zBuffer[idx])
                {
                    zBuffer[idx] = ooz;
                    buffer[idx] = ch;
                }
            }
        }


        internal void Draw()
        {
            Console.CursorVisible = false;
            Console.Write("\x1b[2j");
            while (true)
            {
                buffer = Enumerable.Repeat(backgroundASCIICode, width * height).ToArray<char>();
                zBuffer = Enumerable.Repeat(0d, width * height * 4).ToArray<double>();

                for(float cubeX = - cubeWidth; cubeX < cubeWidth; cubeX += incrementSpeed)
                {
                    for (float cubeY = -cubeWidth; cubeY < cubeWidth; cubeY += incrementSpeed)
                    {
                        CalculateForSurface(cubeX, cubeY, -cubeWidth, '.');
                        CalculateForSurface(cubeWidth, cubeY, cubeX, '$');
                        CalculateForSurface(-cubeWidth, cubeY, -cubeX, '~');
                        CalculateForSurface(-cubeX, cubeY, cubeWidth, '#');
                        CalculateForSurface(cubeX, cubeWidth, -cubeY, ';');
                        CalculateForSurface(cubeX, cubeWidth, cubeY, '-');
                    }
                }
                Console.Write("\x1b[H");
                for (int k = 0; k < width * height; k++)
                {
                    Console.Write(k % width != 0 ? buffer[k] : char.ToString('\n'));
                }

                A += 0.005f;
                B += 0.005f;
            }
        }

        public static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
            new SpinningCube().Draw();
        }
    }
}