using NaughtyAttributes;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {

    [SerializeField] Transform pointPrefab = default;
    [Range(10, 100)]
    [SerializeField] int resolution = 10;
    [SerializeField] int power = 3;

    void Awake() => Visualize();

    [Button]
    void Visualize() {
        Clear();
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        foreach (var index in Enumerable.Range(-resolution / 2, resolution)) {
            var point = Instantiate(pointPrefab, transform);
            point.localScale = scale;
            position.x = (index + .5f) * step;
            position.y = Mathf.Pow(position.x, power);
            point.localPosition = position;
        }

        void Clear() {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
    }
}
