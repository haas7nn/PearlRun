// VictoryPanelAnimator.cs
// When the Victory Panel becomes active (SetActive true),
// this script hides all the stat texts first, then reveals
// them one by one with a short delay between each.
// At the end it shows "To Be Continued..."

using UnityEngine;
using TMPro;
using System.Collections;

public class VictoryPanelAnimator : MonoBehaviour
{
    [Header("Texts unique to this panel")]
    public TMP_Text titleText;           // Shows "Awal Made It!"
    public TMP_Text toBeContinuedText;   // Shows "To Be Continued..." at the end

    [Header("Particle Effect")]
    public ParticleSystem celebrationParticles;  // Optional celebratory burst

    [Header("Timing")]
    public float delayBetweenStats = 0.4f;  // Seconds between each stat appearing

    // OnEnable runs every time this GameObject is activated (SetActive true)
    private void OnEnable()
    {
        if (titleText != null)
            titleText.text = "Awal Made It!";

        // Hide "To Be Continued" — it appears last
        if (toBeContinuedText != null)
            toBeContinuedText.gameObject.SetActive(false);

        // Play the particle burst
        if (celebrationParticles != null)
            celebrationParticles.Play();

        // Start the staggered reveal
        StartCoroutine(RevealOneByOne());
    }

    private IEnumerator RevealOneByOne()
    {
        ScoreManager sm = ScoreManager.Instance;
        if (sm == null) yield break;

        // Hide all 4 stat fields immediately
        Hide(sm.pearlsText);
        Hide(sm.timeText);
        Hide(sm.livesText);
        Hide(sm.gradeText);

        yield return new WaitForSecondsRealtime(0.3f);  // Short pause before starting

        Show(sm.pearlsText);                              // Pearls appear
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        Show(sm.timeText);                                // Time appears
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        Show(sm.livesText);                               // Lives appear
        yield return new WaitForSecondsRealtime(delayBetweenStats);

        Show(sm.gradeText);                               // Grade appears
        yield return new WaitForSecondsRealtime(delayBetweenStats * 2f);

        // Finally show To Be Continued
        if (toBeContinuedText != null)
            toBeContinuedText.gameObject.SetActive(true);
    }

    private void Hide(TMP_Text t) { if (t != null) t.gameObject.SetActive(false); }
    private void Show(TMP_Text t) { if (t != null) t.gameObject.SetActive(true); }
}