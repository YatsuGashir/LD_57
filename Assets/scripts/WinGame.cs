using Unity.VisualScripting;
using UnityEngine;

public class WinGame : MonoBehaviour
{
    [Header("Система диалогов")]
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] Sprite portrait1;


    private void OnTriggerEnter2D(Collider2D other)
    {
        Win();
    }
    private void Win()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Good job miner. Go into the station's control module and get ready to return to orbit.", portrait = portrait1 },
        };
        dialogueManager.StartDialogue(lines);
        
    }

}
