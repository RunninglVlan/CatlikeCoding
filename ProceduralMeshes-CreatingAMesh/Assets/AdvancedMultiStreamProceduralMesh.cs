using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMultiStreamProceduralMesh : MonoBehaviour {
    void OnEnable() {
        var mesh = new Mesh { name = "ProceduralMesh" };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
