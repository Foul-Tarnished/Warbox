//using Microsoft.Xna.Framework;

using DotNext;
using ImGuiNET;
using Microsoft.Win32;
using StudioCore.Configuration;
using StudioCore.Core.Project;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Veldrid;
using Veldrid.Utilities;

namespace StudioCore;

public static class Utils
{
    public const float Pi = (float)Math.PI;
    public const float PiOver2 = (float)Math.PI / 2.0f;
    public const float Rad2Deg = 180.0f / Pi;
    public const float Deg2Rad = Pi / 180.0f;

    private static readonly char[] _dirSep = { '\\', '/' };

    public static float DegToRadians(float deg)
    {
        return deg * Pi / 180.0f;
    }

    public static float RadiansToDeg(float rad)
    {
        return rad * 180.0f / Pi;
    }

    public static float Clamp(float f, float min, float max)
    {
        if (f < min)
        {
            return min;
        }

        if (f > max)
        {
            return max;
        }

        return f;
    }

    public static float Lerp(float a, float b, float d)
    {
        return (a * (1.0f - d)) + (b * d);
    }

    public static bool IsPowerTwo(uint a)
    {
        if (a > 0)
        {
            while (a % 2 == 0)
            {
                a >>= 1;
            }

            if (a == 1)
            {
                return true;
            }
        }

        return false;
    }

    public static Matrix4x4 Inverse(this Matrix4x4 src)
    {
        Matrix4x4.Invert(src, out Matrix4x4 result);
        return result;
    }


    // Vector rotation functions from John Alexiou at https://stackoverflow.com/questions/69245724/rotate-a-vector-around-an-axis-in-3d-space
    /// <summary>
    ///     Rotates a vector using the Rodriguez rotation formula
    ///     about an arbitrary axis.
    /// </summary>
    public static Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle)
    {
        Vector3 vxp = Vector3.Cross(axis, vector);
        Vector3 vxvxp = Vector3.Cross(axis, vxp);
        return vector + ((float)Math.Sin(angle) * vxp) + ((1 - (float)Math.Cos(angle)) * vxvxp);
    }

    /// <summary>
    ///     Rotates a vector about a point in space.
    /// </summary>
    /// <returns>The rotated vector</returns>
    public static Vector3 RotateVectorAboutPoint(Vector3 vector, Vector3 pivot, Vector3 axis, float angle)
    {
        return pivot + RotateVector(vector - pivot, axis, angle);
    }


    private static double GetColorComponent(double temp1, double temp2, double temp3)
    {
        double num;
        temp3 = MoveIntoRange(temp3);
        if (temp3 < 0.166666666666667)
        {
            num = temp1 + ((temp2 - temp1) * 6 * temp3);
        }
        else if (temp3 >= 0.5)
        {
            num = temp3 >= 0.666666666666667 ? temp1 : temp1 + ((temp2 - temp1) * (0.666666666666667 - temp3) * 6);
        }
        else
        {
            num = temp2;
        }

        return num;
    }

    private static double GetTemp2(float H, float S, float L)
    {
        double temp2;
        temp2 = L >= 0.5 ? L + S - (L * S) : L * (1 + (double)S);
        return temp2;
    }

    /// <summary>
    ///     Derived from https://stackoverflow.com/a/1626232
    /// </summary>
    public static Vector3 ColorToHSV(Color color)
    {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        var hue = color.GetHue();
        var saturation = max == 0 ? 0 : 1.0f - (1.0f * min / max);
        var value = max / 255.0f;

        return new Vector3(hue, saturation, value);
    }

    /// <summary>
    ///     Derived from https://stackoverflow.com/a/1626232
    /// </summary>
    public static Color ColorFromHSV(Vector3 hsv)
    {
        var hue = hsv.X;
        var saturation = hsv.Y;
        var value = hsv.Z;

        var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        var f = (hue / 60) - (float)Math.Floor(hue / 60);

        value *= 255.0f;
        var v = Convert.ToInt32(value);
        var p = Convert.ToInt32(value * (1 - saturation));
        var q = Convert.ToInt32(value * (1 - (f * saturation)));
        var t = Convert.ToInt32(value * (1 - ((1 - f) * saturation)));

        if (hi == 0)
        {
            return Color.FromArgb(255, v, t, p);
        }

        if (hi == 1)
        {
            return Color.FromArgb(255, q, v, p);
        }

        if (hi == 2)
        {
            return Color.FromArgb(255, p, v, t);
        }

        if (hi == 3)
        {
            return Color.FromArgb(255, p, q, v);
        }

        if (hi == 4)
        {
            return Color.FromArgb(255, t, p, v);
        }

        return Color.FromArgb(255, v, p, q);
    }

    private static double MoveIntoRange(double temp3)
    {
        if (temp3 < 0)
        {
            temp3 += 1;
        }
        else if (temp3 > 1)
        {
            temp3 -= 1;
        }

        return temp3;
    }

    public static int ParseHexFromString(string str)
    {
        return int.Parse(str.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
    }

    public static Vector3 GetDecimalColor(Color color)
    {
        float r = Convert.ToSingle(color.R);
        float g = Convert.ToSingle(color.G);
        float b = Convert.ToSingle(color.B);
        Vector3 vec = new Vector3((r / 255), (g / 255), (b / 255));

        //throw new NotImplementedException($"{vec}");

        return vec;
    }

    public static int GenerateRandomInt(RandomNumberGenerator randomSource, int min, int max)
    {
        double randomValue = randomSource.NextDouble();

        TaskLogs.AddLog($"randomValue: {randomValue}");

        int diff = max - min;

        TaskLogs.AddLog($"diff: {diff}");
        // In-case the order is swapped
        if (max < min)
            diff = min - max;

        double tResult = (diff * randomValue);

        TaskLogs.AddLog($"tResult: {tResult}");
        TaskLogs.AddLog($"tResult Rounded: {(int)Math.Round(tResult)}");
        return (int)Math.Round(tResult);
    }

    public static double GenerateRandomDouble(RandomNumberGenerator randomSource, double min, double max)
    {
        double randomValue = randomSource.NextDouble();

        double diff = max - min;

        // In-case the order is swapped
        if (max < min)
            diff = min - max;

        return (diff * randomValue);
    }
}
