using UnityEngine;

public class ResetSave : MonoBehaviour
{
    public void ResetProgress()
    {
        SaveSystem.ResetAllSaves();
        Debug.Log("All save data has been reset.");
    }
}