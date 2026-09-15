using System.Numerics;
using Vortice.Direct2D1;

namespace SinTachiePlugin.Draw
{
    internal record PartNodeDrawDescription(
        Vector3 Draw,
        Vector2 Center,
        Vector2 Zoom,
        Matrix4x4 Rotate,
        Matrix4x4 Camera,
        float Opacity,
        bool Inverse,
        InterpolationMode InterpolationMode)
    {
        public bool Is2DTransform()
        {
            if (Draw.Z != 0)
                return false;

            Matrix4x4 m = Rotate * Camera;

            return
                m.M13 == 0 &&
                m.M23 == 0 &&
                m.M31 == 0 &&
                m.M32 == 0 &&
                m.M33 == 1 &&
                m.M43 == 0 &&

                m.M14 == 0 &&
                m.M24 == 0 &&
                m.M34 == 0 &&
                m.M44 == 1;
        }

        public float GetZ() => (Rotate
                    * Matrix4x4.CreateTranslation(Draw)
                    * Camera
                    * new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, -0.001f, 0f, 0f, 0f, 1f)).M43;

        public static PartNodeDrawDescription Identity { get => _identity; }
        private static readonly PartNodeDrawDescription _identity = new(default, default, default, default, default, 1, false, InterpolationMode.HighQualityCubic);
    }
}
