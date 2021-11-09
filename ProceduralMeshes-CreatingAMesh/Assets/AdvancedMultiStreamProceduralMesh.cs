using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMultiStreamProceduralMesh : MonoBehaviour {
    const int VERTEX_ATTRIBUTES = 4, VERTICES = 4, TRIANGLE_INDICES = 6;

    void OnEnable() {
        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];
        var vertexAttributes =
            new NativeArray<VertexAttributeDescriptor>(VERTEX_ATTRIBUTES, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory
            ) {
                [0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3, stream: 0),
                [1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1),
                [2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float16, 4, 2),
                [3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2, 3)
            };
        meshData.SetVertexBufferParams(VERTICES, vertexAttributes);
        meshData.SetIndexBufferParams(TRIANGLE_INDICES, IndexFormat.UInt16);
        vertexAttributes.Dispose();

        var positions = meshData.GetVertexData<float3>();
        positions[0] = 0;
        positions[1] = math.up();
        positions[2] = math.float3(1, 1, 0);
        positions[3] = math.right();
        var normals = meshData.GetVertexData<float3>(1);
        normals[0] = normals[1] = normals[2] = normals[3] = math.back();
        var tangents = meshData.GetVertexData<half4>(2);
        tangents[0] = tangents[1] = tangents[2] = tangents[3] =
            math.half4(math.half(1), math.half(0), math.half(0), math.half(-1));
        var texCoords = meshData.GetVertexData<half2>(3);
        texCoords[0] = math.half(0);
        texCoords[1] = math.half2(math.half(0), math.half(1));
        texCoords[2] = math.half(1);
        texCoords[3] = math.half2(math.half(1), math.half(0));
        var triangleIndices = meshData.GetIndexData<ushort>();
        triangleIndices[0] = 0;
        triangleIndices[1] = 1;
        triangleIndices[2] = 3;
        triangleIndices[3] = 3;
        triangleIndices[4] = 1;
        triangleIndices[5] = 2;

        var bounds = new Bounds(new Vector3(.5f, .5f), new Vector3(1, 1));
        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, TRIANGLE_INDICES) {
            bounds = bounds,
            vertexCount = VERTICES
        }, MeshUpdateFlags.DontRecalculateBounds);

        var mesh = new Mesh { name = "ProceduralMesh", bounds = bounds };
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
