using NaughtyAttributes;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] GameObject prefab = default;

    GameObject spawned;

    void Start() => Respawn();

    [Button]
    public void Respawn() {
        if (spawned) {
            Destroy(spawned);
        }
        spawned = Instantiate(prefab);
    }
}
