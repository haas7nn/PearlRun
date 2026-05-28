// ButtonHoverEffect.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Font Size")]
    public float normalSize = 24f;
    public float hoverSize = 30f;
    public float sizeSpeed = 10f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.85f, 0f);  // gold

    private TextMeshProUGUI buttonText;
    private float targetSize;

    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        targetSize = normalSize;

        if (buttonText != null)
        {
            buttonText.fontSize = normalSize;
            buttonText.color = normalColor;
        }
    }

    void Update()
    {
        if (buttonText == null) return;

        // Smoothly animate font size
        buttonText.fontSize = Mathf.Lerp(buttonText.fontSize, targetSize, Time.deltaTime * sizeSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData) { Highlight(); }
    public void OnPointerExit(PointerEventData eventData) { Unhighlight(); }
    public void OnSelect(BaseEventData eventData) { Highlight(); }
    public void OnDeselect(BaseEventData eventData) { Unhighlight(); }

    void Highlight()
    {
        targetSize = hoverSize;
        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    void Unhighlight()
    {
        targetSize = normalSize;
        if (buttonText != null)
            buttonText.color = normalColor;
    }
}