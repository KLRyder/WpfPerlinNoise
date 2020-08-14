using System;
using System.Windows;

namespace wpf_tut
{
    public class PerlinNoiseGenerator2D
    {
        private Random _randNumber = new Random();
        private Vector[,] _gridVectors;

        public PerlinNoiseGenerator2D(int gridWidth, int gridHight)
        {
            _gridVectors = new Vector[gridWidth, gridHight];

            for (var x = 1; x < gridWidth; x++)
            {
                for (var y = 1; y < gridHight; y++)
                {
                    _gridVectors[x, y] = GenUnitVector();
                }
            }
        }

        /// <summary>
        /// Creates a 2D unit vector in a random direction distributed uniformly by selecting a point on a unit circle.
        /// </summary>
        /// <returns>Uniformly distributed random 2D unit vector</returns>
        private Vector GenUnitVector()
        {
            var angle = _randNumber.NextDouble() * 2 * Math.PI;
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector(x, y);
        }

        /// <summary>
        /// Finds the value linearly interpreted between start and end.
        /// </summary>
        /// <param name="start">Double representing the start value.</param>
        /// <param name="end">Double representing the end value.</param>
        /// <param name="weight">Double between 0 and 1, defaults to 0.5.</param>
        /// <returns>Double between start and end.</returns>
        private static double Lerp(double start, double end, double weight = 0.5)
        {
            return (1 - weight) * start + weight * end;
        }

        /// <summary>
        /// Finds the value bilinearly interpreted between 4 values modeled on a unit square.
        /// </summary>
        /// <param name="p00">Double representing the value at point (0,0) on the unit square.</param>
        /// <param name="p10">Double representing the value at point (1,0) on the unit square.</param>
        /// <param name="p01">Double representing the value at point (0,1) on the unit square.</param>
        /// <param name="p11">Double representing the value at point (1,1) on the unit square.</param>
        /// <param name="x">Double between 0 and 1, defaults to 0.5.</param>
        /// <param name="y">Double between 0 and 1, defaults to 0.5.</param>
        /// <returns>Double between the max and min values of p00, p10, p01, and p11.</returns>
        private static double Blurp(double p00, double p10, double p01, double p11, double x, double y)
        {
            return p00 * (1 - x) * (1 - y) + p10 * x * (1 - y) + p01 * (1 - x) * y + p11 * x * y;
        }

        /// <summary>
        /// Takes a sample of the plane described by the pre-generated gradent vectors. If scale is an integer the result will always be 0.
        /// </summary>
        /// <param name="x">Int representing x value on the plane</param>
        /// <param name="y">Int representing y value on the plane</param>
        /// <param name="scale">Double to scale the plane by.</param>
        /// <returns>Double between -0.75 and 0.75.</returns>
        public double GenerateValue(int x, int y, double scale)
        {
            if (scale <= 0.0001)
            {
                scale = 0.0001;
            }

            // Scale the plane for more control over the results.
            var scaledX = x * scale;
            var scaledY = y * scale;

            // Find co-ords for cell corners ajacent to point x,y.
            var x0 = (int) Math.Floor(scaledX) % _gridVectors.GetLength(0);
            var x1 = (x0 + 1) % _gridVectors.GetLength(0);
            var y0 = (int) Math.Floor(scaledY) % _gridVectors.GetLength(1);
            var y1 = (y0 + 1) % _gridVectors.GetLength(1);

            // find the distance point x,y is from each cell corner.
            var dx0 = scaledX - Math.Floor(scaledX);
            var dx1 = dx0 - 1;
            var dy0 = scaledY - Math.Floor(scaledY);
            var dy1 = dy0 - 1;

            var dv00 = new Vector(dx0, dy0);
            var dv10 = new Vector(dx1, dy0);
            var dv01 = new Vector(dx0, dy1);
            var dv11 = new Vector(dx1, dy1);

            // smooth out corners for later linear interpolation.
            var dxs = Smootherstep(dx0);
            var dys = Smootherstep(dy0);

            // grab the gradents at each cell corner.
            var g00 = _gridVectors[x0, y0];
            var g10 = _gridVectors[x1, y0];
            var g01 = _gridVectors[x0, y1];
            var g11 = _gridVectors[x1, y1];

            // take the dot product of the gradent and distance vectors.
            var p00 = Vector.Multiply(dv00, g00);
            var p10 = Vector.Multiply(dv10, g10);
            var p01 = Vector.Multiply(dv01, g01);
            var p11 = Vector.Multiply(dv11, g11);

            // take the liner interpilations of all points to find final value.
            var a = Lerp(p00, p10, dxs);
            var b = Lerp(p01, p11, dxs);

            return Lerp(a, b, dys);
        }

        public double GenerateSecondLevelValue(int x, int y, double scale1, double scale2, double mix)
        {
            if (scale1 <= 0.00001)
            {
                scale1 = 0.00001;
            }

            if (scale2 <= 0.00001)
            {
                scale2 = 0.00001;
            }

            return Lerp(GenerateValue(x, y, scale1),
                GenerateValue(x, y, scale2),
                mix);
        }

        public double GenerateOctavesValue(int x, int y, double scale, int octaves, double persistance,
            double lacunarity)
        {
            var val = 0.0;
            var amplitude = 1.0;
            var fequ = 1.0;
            for (int i = 0; i < octaves; i++)
            {
                val+=GenerateValue(x, y, scale*fequ)*amplitude;
                amplitude *= persistance;
                fequ *= lacunarity;
            }

            return val;
        }

        public static double Smoothstep(double x)
        {
            return Math.Pow(3 * x, 2) - Math.Pow(2 * x, 3);
        }

        public static double Smootherstep(double x)
        {
            return 6 * Math.Pow(x, 5) - 15 * Math.Pow(x, 4) + 10 * Math.Pow(x, 3);
        }
    }
}