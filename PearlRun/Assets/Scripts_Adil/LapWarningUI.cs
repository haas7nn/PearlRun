using System.Collections;
using TMPro;
using UnityEngine;

public class LapWarningUI : MonoBehaviour
{
    public GameObject panel;     // drag your LapWarningPanel here
    public TMP_Text text;        // drag the TMP text here

    public float duration = 2.5f;
    public float flashInterval = 0.3f;

    Coroutine routine;

    void Awake()
    {
        // panel can start disabled; this script object must stay enabled
        if (panel != null) panel.SetActive(false);
    }

    public void Show(string message)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Flash(message));
    }

    IEnumerator Flash(string message)
    {
        if (panel != null) panel.SetActive(true);
        if (text != null) text.text = message;

        float t = 0f;
        bool on = true;

        while (t < duration)
        {
            on = !on;
            if (text != null) text.enabled = on;

            yield return new WaitForSecondsRealtime(flashInterval);
            t += flashInterval;
        }

        if (text != null) text.enabled = true;
        if (panel != null) panel.SetActive(false);

        routine = null;
    }
}