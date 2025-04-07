using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public Sprite portrait;
}

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    public float typingSpeed = 0.02f;

    public DialogueLine[] dialogueLines;

    private int currentLine = 0;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Пропустить печатание — сразу показать всю строку
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueLines[currentLine].text;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        dialogueLines = lines;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        isDialogueActive = true;
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            DialogueLine line = dialogueLines[currentLine];
            portraitImage.sprite = line.portrait;
            typingCoroutine = StartCoroutine(TypeText(line.text));
            currentLine++;
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed); // важно: Realtime
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        isDialogueActive = false;
    }
}
