using System.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField] Mesh mesh = default;
    [SerializeField] Material material = default;
    [SerializeField] int maxDepth = 4;
    [SerializeField] float childScale = .5f;

    int depth;

    static readonly Child[] children = {
        new Child(Vector3.up, Quaternion.identity),
        new Child(Vector3.right, Quaternion.Euler(0, 0, -90)),
        new Child(Vector3.left, Quaternion.Euler(0, 0, 90)),
        new Child(Vector3.forward, Quaternion.Euler(90, 0, 0)),
        new Child(Vector3.back, Quaternion.Euler(-90, 0, 0))
    };

    void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if (depth < maxDepth) {
            StartCoroutine(AddChildren());
        }

        IEnumerator AddChildren() {
            foreach (var child in children) {
                yield return new WaitForSeconds(.5f);
                new GameObject("Fractal Child").AddComponent<Fractal>()
                    .Initialize(this, child.direction, child.orientation);
            }
        }
    }

    void Initialize(Fractal parent, Vector3 direction, Quaternion orientation) {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        var childTransform = transform;
        childTransform.parent = parent.transform;
        childTransform.localScale = Vector3.one * childScale;
        childTransform.localPosition = direction * (.5f + .5f * childScale);
        childTransform.localRotation = orientation;
    }

    readonly struct Child {
        public readonly Vector3 direction;
        public readonly Quaternion orientation;

        public Child(Vector3 direction, Quaternion orientation) {
            this.direction = direction;
            this.orientation = orientation;
        }
    }
}
