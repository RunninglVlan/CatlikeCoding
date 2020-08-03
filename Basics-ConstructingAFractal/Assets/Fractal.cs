﻿using System.Collections;
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

    [SerializeField] Mesh mesh = default;
    [SerializeField] Material material = default;
    [SerializeField] int maxDepth = 4;
    [SerializeField] float childScale = .5f;

    int depth;

    Material[,] materials;

    void Start() {
        if (materials == null) {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth) {
            StartCoroutine(AddChildren());
        }

        IEnumerator AddChildren() {
            foreach (var child in CHILDREN) {
                yield return new WaitForSeconds(Random.Range(.1f, .5f));
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
        mesh = parent.mesh;
        materials = parent.materials;
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
