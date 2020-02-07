using System.Collections;
using TMPro;
using UnityEngine;

public class Information : MonoBehaviour {

    [HideInInspector]
    [SerializeField] RectTransform panel = default;
    [HideInInspector]
    [SerializeField] RectTransform panelBackground = default;
    [HideInInspector]
    [SerializeField] TextMeshProUGUI textComponent = default;

    [SerializeField] Vector2 size = new Vector2(330, 70);
    [SerializeField] float movingSpeed = 1;
    [Multiline]
    [SerializeField] string text = default;
    [SerializeField] string link = default;

    private float shownPositionY, hiddenPositionY;
    private bool hidden = true;

    void Awake() {
        shownPositionY = 0;
        hiddenPositionY = -panel.sizeDelta.y;
    }

    public void bTogglePanel() {
        if (hidden) {
            StartCoroutine(Toggle(hiddenPositionY, shownPositionY));
        } else {
            StartCoroutine(Toggle(shownPositionY, hiddenPositionY));
        }

        IEnumerator Toggle(float startY, float targetY) {
            float time = 0f;
            while (time <= 1) {
                time += movingSpeed * Time.fixedDeltaTime;
                panelBackground.position = panelBackground.position.SetY(Mathf.Lerp(startY, targetY, time));
                yield return new WaitForFixedUpdate();
            }
            hidden = !hidden;
        }
    }

    public void bOpenLink() => Application.OpenURL(link);

    void OnValidate() {
        panel.sizeDelta = size;
        panelBackground.anchoredPosition = panelBackground.anchoredPosition.SetY(-size.y);
        textComponent.text = text;
    }
}

public static class VectorExtensions {
    public static Vector2 SetY(this Vector2 vector, float y) => new Vector2(vector.x, y);
    public static Vector3 SetY(this Vector3 vector, float y) => new Vector3(vector.x, y);
}
