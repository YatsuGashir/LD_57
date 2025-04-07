using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [Header("Система диалогов")]
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] Sprite portrait1;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Fly up to the ore and it will be mined.", portrait = portrait1 },
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstOreEx()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Good.", portrait = portrait1 },
            new DialogueLine { text = "Now move towards the platform and fly up to the coolers on the sides. " +
                                      "Stand next to them to refuel.", portrait = portrait1 }
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstCoolR()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Great.", portrait = portrait1 },
            new DialogueLine { text = "Now, right-click while near the platform to sit at the turret.", portrait = portrait1 }
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstSit()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Briefing is complete, unit. " +
                                      "Upgrade the platform with the mined ore. " +
                                      "Keep an eye on the coolant and drill all the way to the bottom.\"", portrait = portrait1 },

        };

        dialogueManager.StartDialogue(lines);
        DrillController.Instance.isDrill = true;
    }

    public void HardGround()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "The enemy has retreated to regroup. " +
                                      "Gather as much ore as you can before reinforcements arrive.", portrait = portrait1 },
        };

        dialogueManager.StartDialogue(lines);
    }

}
