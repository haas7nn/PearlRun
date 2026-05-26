using UnityEngine;

public class AreaTitleTrigger : MonoBehaviour
{
    [Header("Title to show")]
    public string areaName = "Desert Village";  // type your area name here

    private bool triggered = false;
    private Collider _col;

    void Awake()
    {
        _col = GetComponent<Collider>();
        if (_col != null) _col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (AreaTitleDisplay.instance != null)
            AreaTitleDisplay.instance.ShowTitle(areaName);

        // Disable collider so it never fires again
        if (_col != null) _col.enabled = false;
    }
}