using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour {

    [HideInInspector]
    [SerializeField] Image panel = default;
    [HideInInspector]
    [SerializeField] TextMeshProUGUI textComponent = default;

    [SerializeField] Vector2 size = new Vector2(330, 70);
    [SerializeField] float movingSpeed = 1;
    [Multiline]
    [SerializeField] string text = default;
    [SerializeField] string link = default;

    private Vector3 shownPosition;
    private Vector3 hiddenPosition;
    private bool hidden = true;

    void Awake() {
        shownPosition = new Vector3(panel.transform.position.x, 0);
        hiddenPosition = new Vector3(panel.transform.position.x, -panel.rectTransform.sizeDelta.y);
    }

    public void bTogglePanel() {
        if (hidden) {
            StartCoroutine(Toggle(hiddenPosition, shownPosition));
        } else {
            StartCoroutine(Toggle(shownPosition, hiddenPosition));
        }

        IEnumerator Toggle(Vector3 start, Vector3 target) {
            float time = 0f;
            while (time <= 1) {
                time += movingSpeed * Time.fixedDeltaTime;
                panel.transform.position = Vector3.Lerp(start, target, time);
                yield return new WaitForFixedUpdate();
            }
            hidden = !hidden;
        }
    }

    public void bOpenLink() => Application.OpenURL(link);

    void OnValidate() {
        panel.rectTransform.sizeDelta = size;
        textComponent.text = text;
    }
}
