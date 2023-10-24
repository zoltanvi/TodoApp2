﻿// See: https://github.com/albi005/MaterialColorUtilities
using System;

namespace TodoApp2.Material
{
    /// <summary>
    /// Utility methods for mathematical operations.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>The linear interpolation function.</summary>
        /// <returns>
        /// <paramref name="start"/> if <paramref name="amount"/> = 0 and <paramref name="stop"/> if <paramref name="amount"/> = 1
        /// </returns>
        public static double Lerp(double start, double stop, double amount)
        {
            return (1.0 - amount) * start + amount * stop;
        }

        /// <summary>Clamps an integer between two integers.</summary>
        /// <returns>
        /// <paramref name="input"/> when <paramref name="min"/> ≤ <paramref name="input"/> ≤ <paramref name="max"/>,
        /// and either <paramref name="min"/> or <paramref name="max"/> otherwise.
        /// </returns>
        public static int ClampInt(int min, int max, int input)
        {
            if (input < min)
                return min;
            if (input > max)
                return max;
            return input;
        }

        /// <summary>Clamps an integer between two floating-point numbers.</summary>
        /// <returns>
        /// <paramref name="input"/> when <paramref name="min"/> ≤ <paramref name="input"/> ≤ <paramref name="max"/>,
        /// and either <paramref name="min"/> or <paramref name="max"/> otherwise.
        /// </returns>
        public static double ClampDouble(double min, double max, double input)
        {
            if (input < min)
                return min;
            if (input > max)
                return max;
            return input;
        }

        /// <summary>Sanitizes a degree measure as an integer.</summary>
        /// <returns>A degree measure between 0 (inclusive) and 360 (exclusive).</returns>
        public static uint SanitizeDegreesInt(int degrees)
        {
            degrees %= 360;
            if (degrees < 0)
            {
                degrees += 360;
            }
            return (uint)degrees;
        }

        /// <summary>Sanitizes a degree measure as a floating-point number.</summary>
        /// <returns>A degree measure between 0.0 (inclusive) and 360.0 (exclusive).</returns>
        public static double SanitizeDegreesDouble(double degrees)
        {
            degrees %= 360.0;
            if (degrees < 0)
            {
                degrees += 360.0;
            }
            return degrees;
        }

        /// <summary>
        /// Sign of direction change needed to travel from one angle to another.
        /// </summary>
        /// <param name="from">The angle travel starts from, in degrees.</param>
        /// <param name="to">The angle travel ends at, in degrees.</param>
        /// <returns>
        /// -1 if decreasing <paramref name="from"/> leads to the shortest travel distance, 1 if increasing <paramref name="from"/> leads
        /// to the shortest travel distance.
        /// </returns>
        public static double RotationDirection(double from, double to)
        {
            double increasingDifference = SanitizeDegreesDouble(to - from);
            return increasingDifference <= 180.0 ? 1.0 : -1.0;
        }

        /// <summary>Distance of two points on a circle, represented using degrees.</summary>
        public static double DifferenceDegrees(double a, double b)
        {
            return 180.0 - Math.Abs(Math.Abs(a - b) - 180.0);
        }

        /// <summary>Multiplies a 1x3 row vector with a 3x3 matrix.</summary>
        public static double[] MatrixMultiply(double[] row, double[][] matrix)
        {
            double a = row[0] * matrix[0][0] + row[1] * matrix[0][1] + row[2] * matrix[0][2];
            double b = row[0] * matrix[1][0] + row[1] * matrix[1][1] + row[2] * matrix[1][2];
            double c = row[0] * matrix[2][0] + row[1] * matrix[2][1] + row[2] * matrix[2][2];
            return new double[] { a, b, c };
        }

        public static double ToRadians(double angdeg) => Math.PI / 180 * angdeg;
        public static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);
        public static double Log1p(double x) => Math.Log(1 + x);
        public static double Expm1(double x) => Math.Exp(x) - 1;

        /// <summary>
        /// Given a seed hue, and a mapping of hues to hue rotations, find which hues in the mapping the seed hue falls
        /// between, and add the hue rotation of the lower hue to the seed hue.
        /// </summary>
        /// <param name="seedHue">Hue of the seed color</param>
        /// <param name="hueAndRotations">
        /// List of pairs, where the first item in a pair is a hue, and the second item in
        /// the pair is a hue rotation that should be applied</param>
        /// <returns></returns>
        public static double RotateHue(double seedHue, params (int Hue, int Rotation)[] hueAndRotations)
        {
            for (int i = 0; i < hueAndRotations.Length - 1; i++)
            {
                double thisHue = hueAndRotations[i].Hue;
                double nextHue = hueAndRotations[i + 1].Hue;
                if (thisHue <= seedHue && seedHue < nextHue)
                    return SanitizeDegreesDouble(seedHue + hueAndRotations[i].Rotation);
            }

            return seedHue;
        }
    }
}
