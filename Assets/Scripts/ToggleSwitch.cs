using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimatedToggleSwitch : MonoBehaviour
{
    [Header("References")]
    public RectTransform knob;
    public Image backgroundImage;
    public Button toggleButton;

    [Header("Positions")]
    public float leftX = -40f;
    public float rightX = 40f;

    [Header("Animation")]
    public float animationDuration = 0.2f;

    [Header("Colors")]
    public Color offColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    public Color onColor = Color.white;

    private bool isOn = false;
    private Coroutine currentAnimation;

    void Start()
    {
        toggleButton.onClick.AddListener(Toggle);
        ApplyInstantState();
    }

    public void Toggle()
    {
        isOn = !isOn;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimateToggle());
    }

    IEnumerator AnimateToggle()
    {
        float elapsed = 0f;

        float startX = knob.anchoredPosition.x;
        float targetX = isOn ? rightX : leftX;

        Color startColor = backgroundImage.color;
        Color targetColor = isOn ? onColor : offColor;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            float newX = Mathf.Lerp(startX, targetX, t);
            knob.anchoredPosition = new Vector2(newX, knob.anchoredPosition.y);

            backgroundImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        knob.anchoredPosition = new Vector2(targetX, knob.anchoredPosition.y);
        backgroundImage.color = targetColor;
    }

    void ApplyInstantState()
    {
        float targetX = isOn ? rightX : leftX;
        knob.anchoredPosition = new Vector2(targetX, knob.anchoredPosition.y);
        backgroundImage.color = isOn ? onColor : offColor;
    }
}