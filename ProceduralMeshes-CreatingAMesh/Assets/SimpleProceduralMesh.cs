using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour {
    void Awake() {
        var mesh = new Mesh { name = "ProceduralMesh" };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
