# Notes

[Jobs - Animating a Fractal](https://catlikecoding.com/unity/tutorials/basics/jobs/)

## What's new
- Use [Burst Inspector's](https://docs.unity3d.com/Packages/com.unity.burst@1.6/manual/docs/QuickStart.html#burst-inspector) _LLVM IR Optimization Diagnostics_ to see how your Job can be optimized some more
- In `Mathematics`:
  - `Vector3.one` is equivalent to `math.one()`
  - `Quaternion * Quaternion` is equivalent to `math.mul(quaternion, quaternion)`
  - `Vector3.one * float` is equivalent to `math.float3(float)`
