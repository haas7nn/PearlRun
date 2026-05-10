using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Image characterPortrait;

    [Header("Dialogue")]
    public string[] dialogueLines;
    public string characterName = "Awal";

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public string nextSceneName = "Level2_Main";

    private int currentLineIndex;
    private bool isTyping;

    void Start()
    {
        characterNameText.text = characterName;
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in dialogueLines[currentLineIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}