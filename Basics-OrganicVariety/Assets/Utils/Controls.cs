using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Button;

public class Controls : MonoBehaviour {
    [SerializeField] Control[] controls;

    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var controlsElement = root.Q<VisualElement>("controls");
        foreach (var control in controls) {
            controlsElement.Add(new Button(control.onClick.Invoke) {
                text = control.text
            });
        }
    }

    [Serializable]
    class Control {
        public string text;
        public ButtonClickedEvent onClick;
    }
}
