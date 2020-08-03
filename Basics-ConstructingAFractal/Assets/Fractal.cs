using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fractal : MonoBehaviour {
    static readonly Child[] CHILDREN = {
        new Child(Vector3.up, Quaternion.identity),
        new Child(Vector3.right, Quaternion.Euler(0, 0, -90)),
        new Child(Vector3.left, Quaternion.Euler(0, 0, 90)),
        new Child(Vector3.forward, Quaternion.Euler(90, 0, 0)),
        new Child(Vector3.back, Quaternion.Euler(-90, 0, 0))
    };

    [SerializeField] Mesh[] meshes = default;
    [SerializeField] Material material = default;
    [SerializeField] int maxDepth = 4;
    [SerializeField] float childScale = .5f;
    [SerializeField] float spawnChance = .5f;
    [SerializeField] float maxRotationSpeed = 60;
    [SerializeField] float maxTwist = 20;

    Material[,] materials;
    float rotationSpeed;
    int depth;

    void Start() {
        if (materials == null) {
            InitializeMaterials();
        }
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0, 0);
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth) {
            StartCoroutine(AddChildren());
        }

        IEnumerator AddChildren() {
            foreach (var child in CHILDREN) {
                yield return new WaitForSeconds(Random.Range(.1f, .5f));
                if (Random.value > spawnChance) {
                    continue;
                }
                new GameObject("Fractal Child").AddComponent<Fractal>()
                    .Initialize(this, child.direction, child.orientation);
            }
        }
    }

    void InitializeMaterials() {
        materials = new Material[maxDepth + 1, 2];
        for (var index = 0; index <= maxDepth; index++) {
            var t = (float) index / (maxDepth - 1);
            t *= t;
            materials[index, 0] = new Material(material) {color = Color.Lerp(Color.white, Color.yellow, t)};
            materials[index, 1] = new Material(material) {color = Color.Lerp(Color.white, Color.cyan, t)};
        }
        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    void Initialize(Fractal parent, Vector3 direction, Quaternion orientation) {
        meshes = parent.meshes;
        maxDepth = parent.maxDepth;
        childScale = parent.childScale;
        spawnChance = parent.spawnChance;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
        materials = parent.materials;
        depth = parent.depth + 1;
        var childTransform = transform;
        childTransform.parent = parent.transform;
        childTransform.localScale = Vector3.one * childScale;
        childTransform.localPosition = direction * (.5f + .5f * childScale);
        childTransform.localRotation = orientation;
    }

    void Update() => transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

    readonly struct Child {
        public readonly Vector3 direction;
        public readonly Quaternion orientation;

        public Child(Vector3 direction, Quaternion orientation) {
            this.direction = direction;
            this.orientation = orientation;
        }
    }
}
