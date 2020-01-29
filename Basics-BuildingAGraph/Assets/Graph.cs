using NaughtyAttributes;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {

    [SerializeField] Transform pointPrefab = default;
    [Range(10, 100)]
    [SerializeField] int resolution = 10;

    void Awake() => Visualize();

    [Button]
    void Visualize() {
        var scale = Vector3.one / 5;
        var position = Vector3.zero;
        foreach (var index in Enumerable.Range(-5, resolution)) {
            var point = Instantiate(pointPrefab);
            point.localScale = scale;
            position.x = (index + .5f) / 5;
            position.y = Mathf.Pow(position.x, 2);
            point.localPosition = position;
        }
    }
}
