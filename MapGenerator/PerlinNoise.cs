using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace MapGenerator
{
    public static class PerlinNoise
    {
        private static readonly int[] p = new int[512];
        private static readonly byte[] permutation = new byte[] {
              225,155,210,108,175,199,221,144,203,116, 70,213, 69,158, 33,252,
                5, 82,173,133,222,139,174, 27,  9, 71, 90,246, 75,130, 91,191,
              169,138,  2,151,194,235, 81,  7, 25,113,228,159,205,253,134,142,
              248, 65,224,217, 22,121,229, 63, 89,103, 96,104,156, 17,201,129,
               36,  8,165,110,237,117,231, 56,132,211,152, 20,181,111,239,218,
              170,163, 51,172,157, 47, 80,212,176,250, 87, 49, 99,242,136,189,
              162,115, 44, 43,124, 94,150, 16,141,247, 32, 10,198,223,255, 72,
               53,131, 84, 57,220,197, 58, 50,208, 11,241, 28,  3,192, 62,202,
               18,215,153, 24, 76, 41, 15,179, 39, 46, 55,  6,128,167, 23,188,
              106, 34,187,140,164, 73,112,182,244,195,227, 13, 35, 77,196,185,
               26,200,226,119, 31,123,168,125,249, 68,183,230,177,135,160,180,
               12,  1,243,148,102,166, 38,238,251, 37,240,126, 64, 74,161, 40,
              184,149,171,178,101, 66, 29, 59,146, 61,254,107, 42, 86,154,  4,
              236,232,120, 21,233,209, 45, 98,193,114, 78, 19,206, 14,118,127,
               48, 79,147, 85, 30,207,219, 54, 88,234,190,122, 95, 67,143,109,
              137,214,145, 93, 92,100,245,  0,216,186, 60, 83,105, 97,204, 52};

        public static double OctavePerlin(double x, double y, double z, int octaves, double persistence)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;            // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++)
            {
                total += Perlin(x * frequency, y * frequency, z * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        public static System.Drawing.Bitmap CreatePerlinNoise()
        {
            Bitmap bitmap = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            double widthDivisor = 1 / (double)512;
            double heightDivisor = 1 / (double)512;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {

                    //double v = (OctavePerlin(x * widthDivisor, y * heightDivisor, 1.89f, 16, 0.75f));

                    //double r = (OctavePerlin(x * widthDivisor, y * heightDivisor, 0.5f, 4, 2f));
                    //double g = (OctavePerlin(x * widthDivisor, y * heightDivisor, 0.5f, 16, 2.75f));
                    //double b = (OctavePerlin(x * widthDivisor, y * heightDivisor, 0.5f, 8, 2.75f));
                    double v = Perlin(8 * x * widthDivisor, 8 * y * widthDivisor, 0) * .6 +
                        Perlin(16 * x * widthDivisor, 16 * y * widthDivisor, 0) * .3 +
                        Perlin(32 * x * widthDivisor, 32 * y * widthDivisor, 0) * .1;
                    //v = Math.Min(1, Math.Max(0, v));
                    //byte b = (byte)(v * 255);
                    v = Math.Pow(v, 0.93);
                    int s = Math.Min(Math.Max((int)(255 * v), 0), 255);
                    //int r1 = Math.Min(Math.Max((int)(255 * r), 0), 255);
                    //int g1 = Math.Min(Math.Max((int)(255 * g), 0), 255);
                    //int b1 = Math.Min(Math.Max((int)(255 * b), 0), 255);
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(s, s, s);

                    bitmap.SetPixel(x, y, color);
                }
            }
            
            //System.IO.FileStream fs = new System.IO.FileStream("PerlinNoise.png", System.IO.FileMode.Create);
            //bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);

            //bitmap.LockBits(new System.Drawing.Rectangle(0, 0, 128, 128), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            /*bitmap.SetEachPixelColour(
                (point, color) =>
                {
                    // Note that the result from the noise function is in the range -1 to 1, but I want it in the range of 0 to 1
                    // that's the reason of the strange code
                    double v = (Perlin(2 * point.X * widthDivisor, 2 * point.Y * heightDivisor, -0.5) + 1) / 2 * 0.2;
                    
                        // First octave
                        (perlinNoise.Noise(2 * point.X * widthDivisor, 2 * point.Y * heightDivisor, -0.5) + 1) / 2 * 0.7 +
                        // Second octave
                        (perlinNoise.Noise(4 * point.X * widthDivisor, 4 * point.Y * heightDivisor, 0) + 1) / 2 * 0.2 +
                        // Third octave
                        (perlinNoise.Noise(8 * point.X * widthDivisor, 8 * point.Y * heightDivisor, +0.5) + 1) / 2 * 0.1;
                        
                    v = Math.Min(1, Math.Max(0, v));
                    byte b = (byte)(v * 255);
                    return System.Drawing.Color.FromArgb(b, b, b);
                });
            */
            return bitmap;
        }

        public static double Perlin(double x, double y, double z)
        {
            for (int i = 0; i < 512; i++)
            {
                p[i] = permutation[i % 256];
            }

            int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
            int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
            int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
            double xf = x - (int)x;
            double yf = y - (int)y;
            double zf = z - (int)z;

            double u = Fade(xf);
            double v = Fade(yf);
            double w = Fade(zf);

            int aaa, aba, aab, abb, baa, bba, bab, bbb;
            aaa = p[p[p[xi] + yi] + zi];
            aba = p[p[p[xi] + Inc(yi)] + zi];
            aab = p[p[p[xi] + yi] + Inc(zi)];
            abb = p[p[p[xi] + Inc(yi)] + Inc(zi)];
            baa = p[p[p[Inc(xi)] + yi] + zi];
            bba = p[p[p[Inc(xi)] + Inc(yi)] + zi];
            bab = p[p[p[Inc(xi)] + yi] + Inc(zi)];
            bbb = p[p[p[Inc(xi)] + Inc(yi)] + Inc(zi)];

            double x1, x2, y1, y2;
            x1 = Lerp(Grad(aaa, xf, yf, zf),           // The gradient function calculates the dot product between a pseudorandom
                        Grad(baa, xf - 1, yf, zf),             // gradient vector and the vector from the input coordinate to the 8
                        u);                                     // surrounding points in its unit cube.
            x2 = Lerp(Grad(aba, xf, yf - 1, zf),           // This is all then lerped together as a sort of weighted average based on the faded (u,v,w)
                       Grad(bba, xf - 1, yf - 1, zf),             // values we made earlier.
                          u);
            y1 = Lerp(x1, x2, v);

            x1 = Lerp(Grad(aab, xf, yf, zf - 1),
                        Grad(bab, xf - 1, yf, zf - 1),
                        u);
            x2 = Lerp(Grad(abb, xf, yf - 1, zf - 1),
                          Grad(bbb, xf - 1, yf - 1, zf - 1),
                          u);
            y2 = Lerp(x1, x2, v);

            return (Lerp(y1, y2, w) + 1) / 2;                      // For convenience we bind the result to 0 - 1 (theoretical min/max before is [-1, 1])
        }

        private static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static int Inc(int num)
        {
            num++;
            //if (repeat > 0) num %= repeat;

            return num;
        }

        // Source: http://riven8192.blogspot.com/2010/08/calculate-perlinnoise-twice-as-fast.html
        public static double Grad(int hash, double x, double y, double z)
        {
            switch (hash & 0xF)
            {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                case 0x4: return x + z;
                case 0x5: return -x + z;
                case 0x6: return x - z;
                case 0x7: return -x - z;
                case 0x8: return y + z;
                case 0x9: return -y + z;
                case 0xA: return y - z;
                case 0xB: return -y - z;
                case 0xC: return y + x;
                case 0xD: return -y + z;
                case 0xE: return y - x;
                case 0xF: return -y - z;
                default: return 0; // never happens
            }
        }

        // Linear Interpolate
        public static double Lerp(double a, double b, double x)
        {
            return a + x * (b - a);
        }

        public static void SetEachPixelColour(this Bitmap bitmap, Func<System.Drawing.Point, System.Drawing.Color> colourFunc)
        {
            System.Drawing.Point point = new System.Drawing.Point(0, 0);
            for (int x = 0; x < bitmap.Width; x++)
            {
                point.X = x;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    point.Y = y;
                    bitmap.SetPixel(x, y, colourFunc(point));
                }
            }
        }

        public static void SetEachPixelColour(this Bitmap bitmap, Func<System.Drawing.Point, System.Drawing.Color, System.Drawing.Color> colourFunc)
        {
            System.Drawing.Point point = new System.Drawing.Point(0, 0);
            for (int x = 0; x < bitmap.Width; x++)
            {
                point.X = x;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    point.Y = y;
                    bitmap.SetPixel(x, y, colourFunc(point, bitmap.GetPixel(x, y)));
                }
            }
        }
    }
}
