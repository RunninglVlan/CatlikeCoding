using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {

    [SerializeField] Transform pointPrefab = default;

    void Awake() {
        foreach (var index in Enumerable.Range(-5, 10)) {
            var point = Instantiate(pointPrefab);
            point.localPosition = Vector3.right * (index + .5f) / 5;
            point.localScale = Vector3.one / 5;
        }
    }
}
