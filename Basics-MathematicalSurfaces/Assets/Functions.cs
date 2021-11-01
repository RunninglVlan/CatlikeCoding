using Unity.Mathematics;
using UnityEngine;

public static class Functions {
    public delegate Vector3 Function(float u, float v, float time);

    private static readonly Function[] functions = {
        Sine, Sine2D, MultiSine, MultiSine2D, Ripple, Cylinder, Sphere, Torus
    };

    public enum Name {
        Sine,
        Sine2D,
        MultiSine,
        MultiSine2D,
        Ripple,
        Cylinder,
        Sphere,
        Torus
    }

    public static Function Get(Name name) => functions[(int)name];
    public static Name NextName(Name current) => (int)current + 1 < functions.Length ? current + 1 : 0;

    private static Vector3 Sine(float x, float z, float time) {
        var y = Sine(x, time);
        return new Vector3(x, y, z);
    }

    private static Vector3 Sine2D(float x, float z, float time) {
        var y = (Sine(x, time) + Sine(z, time)) * .5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine(float x, float z, float time) {
        var y = Sine(x, time);
        y += math.sin(2 * math.PI * (x + 2 * time)) / 2;
        y *= 2f / 3;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine2D(float x, float z, float time) {
        var y = 4f * math.sin(math.PI * (x + z + time * .5f));
        y += Sine(x, time);
        y += math.sin(2f * math.PI * (z + 2f * time)) * .5f;
        y *= 1f / 5.5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 Ripple(float x, float z, float time) {
        const float multiplier = 1.3f;
        var distance = math.sqrt(x * x + z * z);
        var y = math.sin(4 * (math.PI * distance - time));
        y /= 1 + 10 * distance;
        return new Vector3(x * multiplier, y, z * multiplier);
    }

    private static Vector3 Cylinder(float u, float v, float time) {
        const float multiplier = 1.3f;
        var radius = .8f + Sine(6 * u + 2 * v, time) * .2f;
        return new Vector3(radius * Sine(u) * multiplier, v, radius * Cos(u) * multiplier);
    }

    private static Vector3 Sphere(float u, float v, float time) {
        const float multiplier = 1.3f;
        var radius = .9f + Sine(6 * u + 4 * v, time) * .1f;
        var s = radius * Cos(.5f * v);
        return new Vector3(s * Sine(u) * multiplier, radius * Sine(.5f * v) * multiplier, s * Cos(u) * multiplier);
    }

    private static Vector3 Torus(float u, float v, float time) {
        const float multiplier = 1.5f;
        var radius1 = .65f + Sine(6 * u, .5f * time) * .1f;
        var radius2 = .2f + Sine(8 * u + 4 * v, 2 * time) * .05f;
        var s = radius2 * Cos(v) + radius1;
        return new Vector3(s * Sine(u) * multiplier, radius2 * Sine(v) * multiplier, s * Cos(u) * multiplier);
    }

    private static float Sine(float angle, float time = 0) => math.sin(math.PI * (angle + time));
    private static float Cos(float angle, float time = 0) => math.cos(math.PI * (angle + time));
}
