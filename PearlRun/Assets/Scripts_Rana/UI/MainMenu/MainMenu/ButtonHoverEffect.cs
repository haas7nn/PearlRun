// ButtonHoverEffect.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [Header("Scale Hover (UI controls base size)")]
    public float hoverScaleMultiplier = 1.08f;
    public float scaleSpeed = 12f;

    [Header("Colors")]
    public Color normalColor = Color.black;
    public Color hoverColor = new Color(1f, 0.85f, 0f);

    private TextMeshProUGUI buttonText;
    private RectTransform rt;
    private Vector3 baseScale;
    private Vector3 targetScale;
    private Color targetColor;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        baseScale = rt.localScale;
        targetScale = baseScale;
        targetColor = normalColor;

        if (buttonText != null)
            buttonText.color = normalColor;
    }

    void Update()
    {
        rt.localScale = Vector3.Lerp(rt.localScale, targetScale, Time.deltaTime * scaleSpeed);

        if (buttonText != null)
            buttonText.color = Color.Lerp(buttonText.color, targetColor, Time.deltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData e) => Highlight();
    public void OnPointerExit(PointerEventData e) => Unhighlight();
    public void OnSelect(BaseEventData e) => Highlight();
    public void OnDeselect(BaseEventData e) => Unhighlight();

    void Highlight()
    {
        targetScale = baseScale * hoverScaleMultiplier;
        targetColor = hoverColor;
    }

    void Unhighlight()
    {
        targetScale = baseScale;
        targetColor = normalColor;
    }
}