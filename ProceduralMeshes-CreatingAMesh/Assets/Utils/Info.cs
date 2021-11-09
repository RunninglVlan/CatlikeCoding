using UnityEngine;
using UnityEngine.UIElements;

public class Info : MonoBehaviour {
    [SerializeField, Multiline] string text;
    [SerializeField] string link;

    VisualElement panel;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var info = root.Q<VisualElement>("info");
        info.Q<Label>().text = text;
        panel = info.Q<VisualElement>("info-panel");
        info.Q<Button>().clicked += TogglePanel;
        panel.Q<Button>().clicked += OpenLink;
    }

    void TogglePanel() {
        var current = panel.resolvedStyle.display;
        panel.style.display = current == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
    }

    void OpenLink() => Application.OpenURL(link);
}
