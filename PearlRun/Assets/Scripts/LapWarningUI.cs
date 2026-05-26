using System.Collections;
using TMPro;
using UnityEngine;

public class LapWarningUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text text;

    public float duration = 2.5f;
    public float flashInterval = 0.15f;

    Coroutine routine;

    void Awake()
    {
        if (panel == null) panel = gameObject;
        panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Flash(message));
    }

    IEnumerator Flash(string message)
    {
        panel.SetActive(true);
        if (text != null) text.text = message;

        float t = 0f;
        bool visible = true;

        while (t < duration)
        {
            visible = !visible;
            if (text != null) text.enabled = visible;

            yield return new WaitForSeconds(flashInterval);
            t += flashInterval;
        }

        if (text != null) text.enabled = true;
        panel.SetActive(false);
        routine = null;
    }
}