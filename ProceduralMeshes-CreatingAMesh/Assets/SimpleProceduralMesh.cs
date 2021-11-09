using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour {
    void OnEnable() {
        var mesh = new Mesh {
            name = "ProceduralMesh",
            vertices = new[] { Vector3.zero, Vector3.up, Vector3.right },
            triangles = new[] { 0, 1, 2 }
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
