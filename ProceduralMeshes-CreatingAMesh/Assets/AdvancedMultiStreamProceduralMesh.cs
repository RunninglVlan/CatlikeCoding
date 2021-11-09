using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMultiStreamProceduralMesh : MonoBehaviour {
    const int VERTEX_ATTRIBUTES = 4, VERTICES = 4;

    void OnEnable() {
        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];
        var vertexAttributes =
            new NativeArray<VertexAttributeDescriptor>(VERTEX_ATTRIBUTES, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory
            ) {
                [0] = new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3, stream: 0),
                [1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1),
                [2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4, stream: 2),
                [3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 3)
            };
        meshData.SetVertexBufferParams(VERTICES, vertexAttributes);
        vertexAttributes.Dispose();

        var mesh = new Mesh { name = "ProceduralMesh" };
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
