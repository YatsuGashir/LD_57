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

    public float typingSpeed = 0.05f;

    private DialogueLine[] dialogueLines;
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipRequested = false;

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            // Запрашиваем пропуск анимации
            skipRequested = true;
        }
        else
        {
            // Показываем следующую строку только если не идет анимация
            ShowNextLine();
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        // Сброс состояния перед началом нового диалога
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueLines = lines;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        isDialogueActive = true;
        isTyping = false;
        skipRequested = false;
        
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
        float lastSoundTime = 0f;
        float soundCooldown = 0.05f; // Минимальный интервал между звуками
    
        foreach (char c in text)
        {
            if (skipRequested)
            {
                dialogueText.text = text;
                skipRequested = false;
                break;
            }
        
            dialogueText.text += c;
        
            // Проигрываем звук только если прошло достаточно времени
            if (Time.unscaledTime - lastSoundTime >= soundCooldown)
            {
                AudioManager.instance.Play("typing");
                lastSoundTime = Time.unscaledTime;
            }
        
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        isDialogueActive = false;
        isTyping = false;
        skipRequested = false;
    }
}