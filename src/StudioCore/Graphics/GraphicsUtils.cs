using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.Graphics;

public class GraphicsUtils
{
    // From Veldrid Neo Demo
    public static Matrix4x4 CreatePerspective(
        GraphicsDevice gd,
        bool useReverseDepth,
        float fov,
        float aspectRatio,
        float near, float far)
    {
        Matrix4x4 persp;
        if (useReverseDepth)
        {
            persp = CreatePerspective(fov, aspectRatio, far, near);
        }
        else
        {
            persp = CreatePerspective(fov, aspectRatio, near, far);
        }

        if (gd.IsClipSpaceYInverted)
        {
            persp *= new Matrix4x4(
                1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }
        /*persp = new System.Numerics.Matrix4x4(
            -1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1) * persp;*/

        return persp;
    }

    private static Matrix4x4 CreatePerspective(float fov, float aspectRatio, float near, float far)
    {
        if (fov <= 0.0f || fov >= Math.PI)
        {
            throw new ArgumentOutOfRangeException(nameof(fov));
        }

        if (near <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(near));
        }

        if (far <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(far));
        }

        var yScale = 1.0f / (float)Math.Tan((double)fov * 0.5f);
        var xScale = yScale / aspectRatio;

        Matrix4x4 result;

        result.M11 = xScale;
        result.M12 = result.M13 = result.M14 = 0.0f;

        result.M22 = yScale;
        result.M21 = result.M23 = result.M24 = 0.0f;

        result.M31 = result.M32 = 0.0f;
        var negFarRange = float.IsPositiveInfinity(far) ? -1.0f : far / (near - far);
        result.M33 = -negFarRange;
        result.M34 = 1.0f;

        result.M41 = result.M42 = result.M44 = 0.0f;
        result.M43 = near * negFarRange;

        return result;
    }

    public static void ExtractScale(Matrix4x4 mat, out Vector3 scale, out Matrix4x4 post)
    {
        post = mat;
        var sx = new Vector3(post.M11, post.M12, post.M13).Length();
        var sy = new Vector3(post.M21, post.M22, post.M23).Length();
        var sz = new Vector3(post.M31, post.M32, post.M33).Length();
        scale = new Vector3(sx, sy, sz);
        post.M11 /= sx;
        post.M12 /= sx;
        post.M13 /= sx;
        post.M21 /= sy;
        post.M22 /= sy;
        post.M23 /= sy;
        post.M31 /= sz;
        post.M32 /= sz;
        post.M33 /= sz;
    }
}
