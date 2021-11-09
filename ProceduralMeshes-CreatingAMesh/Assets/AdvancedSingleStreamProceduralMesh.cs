using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedSingleStreamProceduralMesh : MonoBehaviour {
    const int VERTEX_ATTRIBUTES = 4, VERTICES = 4, TRIANGLE_INDICES = 6;

    void OnEnable() {
        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];
        var vertexAttributes =
            new NativeArray<VertexAttributeDescriptor>(VERTEX_ATTRIBUTES, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory
            ) {
                [0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3),
                [1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3),
                [2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float16, 4),
                [3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2)
            };
        meshData.SetVertexBufferParams(VERTICES, vertexAttributes);
        meshData.SetIndexBufferParams(TRIANGLE_INDICES, IndexFormat.UInt16);
        vertexAttributes.Dispose();

        var vertices = meshData.GetVertexData<Vertex>();
        var vertex = new Vertex {
            position = 0,
            normal = math.back(),
            tangent = math.half4(math.half(1), math.half(0), math.half(0), math.half(-1)),
            texCoord0 = math.half(0)
        };
        vertices[0] = vertex;
        vertex.position = math.up();
        vertex.texCoord0 = math.half2(math.half(0), math.half(1));
        vertices[1] = vertex;
        vertex.position = math.float3(1, 1, 0);
        vertex.texCoord0 = math.half(1);
        vertices[2] = vertex;
        vertex.position = math.right();
        vertex.texCoord0 = math.half2(math.half(1), math.half(0));
        vertices[3] = vertex;
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

    [StructLayout(LayoutKind.Sequential)]
    struct Vertex {
        public float3 position, normal;
        public half4 tangent;
        public half2 texCoord0;
    }
}
