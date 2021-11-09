# Notes

[Creating a Mesh - Vertices and Triangles](https://catlikecoding.com/unity/tutorials/procedural-meshes/creating-a-mesh/)

## What's new
- [Texture](https://docs.unity3d.com/Manual/AnatomyofaMesh.html) is a bit like an image printed on a stretchable sheet of rubber. For each mesh triangle, a triangular area of the texture image is defined and that texture triangle is stretched and "pinned" to fit the mesh triangle. To make this work, each vertex needs to store the coordinates of the image position that will be pinned to it. These coordinates are 2D and scaled to the 0..1 range (0 means the bottom/left of the image and 1 means the right/top). To avoid confusing these coordinates with the Cartesian coordinates of the 3D world, they are referred to as U and V rather than the more familiar X and Y, and so they are commonly called UV coordinates.
