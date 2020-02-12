using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class Controls : MonoBehaviour {

    [HideInInspector]
    [SerializeField] RectTransform container = default;
    [HideInInspector]
    [SerializeField] Button prefabButton = default;

    [SerializeField] Control[] controls = default;

    [Button]
    void Rebuild() {
        foreach (Transform button in container) {
            if (!button.gameObject.activeSelf) {
                continue;
            }
            DestroyImmediate(button.gameObject);
        }
        foreach (var control in controls) {
            var button = Instantiate(prefabButton, container);
            button.GetComponentInChildren<TMP_Text>().text = control.text;
            button.onClick = control.onClick;
            button.gameObject.SetActive(true);
        }
    }

    [Serializable]
    struct Control {
        public string text;
        public ButtonClickedEvent onClick;
    }
}
