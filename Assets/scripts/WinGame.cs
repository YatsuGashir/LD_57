using UnityEngine;

public class WinGame : MonoBehaviour
{
    [Header("Система диалогов")]
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] Sprite portrait1;

    private void Win()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Outstanding work, Unit." +
                                      " The company is proud of you.", portrait = portrait1 },
            new DialogueLine { text = "You've uncovered a new vein and didn't end up just another mushroom-head. " +
                                      "Return to base.", portrait = portrait1 }
        };
    }

}
