using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {

    [SerializeField] Transform pointPrefab = default;

    void Awake() {
        foreach (var index in Enumerable.Range(0, 10)) {
            var point = Instantiate(pointPrefab);
            point.localPosition = Vector3.right * index;
        }
    }
}
