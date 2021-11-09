using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour {
    void OnEnable() {
        var mesh = new Mesh {
            name = "ProceduralMesh",
            vertices = new[] {
                Vector3.zero, Vector3.up, Vector3.right,
                new Vector3(0, 1.1f, 0), new Vector3(1.1f, 1.1f, 0), new Vector3(1.1f, 0, 0)
            },
            normals = new[] {
                Vector3.back, Vector3.back, Vector3.back,
                Vector3.back, Vector3.back, Vector3.back
            },
            tangents = new[] {
                new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1),
                new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1), new Vector4(1, 0, 0, -1)
            },
            uv = new[] {
                Vector2.zero, Vector2.up, Vector2.right,
                Vector2.up, Vector2.one, Vector2.right
            },
            triangles = new[] {
                0, 1, 2,
                3, 4, 5
            }
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
