using System;
using TMPro;
using UnityEngine;

public class Information : MonoBehaviour {

    [HideInInspector]
    [SerializeField] RectTransform panel = default;
    [HideInInspector]
    [SerializeField] TextMeshProUGUI textComponent = default;

    [SerializeField] bool panelEnabled = false;
    [SerializeField] Vector2 size = new Vector2(330, 70);
    [Multiline]
    [SerializeField] string text = default;
    [SerializeField] string[] controls = default;
    [SerializeField] string link = default;

    public void bTogglePanel() => panel.gameObject.SetActive(!panel.gameObject.activeSelf);

    public void bOpenLink() => Application.OpenURL(link);

    void OnValidate() {
        panel.gameObject.SetActive(panelEnabled);
        panel.sizeDelta = size;
        var joinedControls = String.Join(", ", controls);
        textComponent.text = string.Join("\n", text, $"Controls: {joinedControls}");
    }
}
