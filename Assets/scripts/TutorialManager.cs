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
            new DialogueLine { text = "Hey miner.", portrait = portrait1 },
            new DialogueLine { text = "You've been sent to this hole to excavate some shroomium. ", portrait = portrait1 },
            new DialogueLine { text = "This is one of the few remaining parts of this space rock that hasn't been nearly touched by those filthy mushrooms.", portrait = portrait1 },
            new DialogueLine { text = "Bring the drills to this purple ore to extract sroomium.", portrait = portrait1 },
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstOreEx()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Good.", portrait = portrait1 },
            new DialogueLine { text = "Your platform needs constant cooling to work. " +
                                      "And you can help her cool down faster.", portrait = portrait1 },
            new DialogueLine { text = "Fly up to the coolant flasks and speed up the cooling.", portrait = portrait1 }
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstCoolR()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Great.", portrait = portrait1 },
            new DialogueLine { text = "Now you can get into the station's control module." +
                                      " It allows you to take control under the turret to fend off some... " +
                                      "uninveted guests. (RMB click)", portrait = portrait1 }
        };

        dialogueManager.StartDialogue(lines);
    }

    public void firstSit()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { text = "Don't forget to keep coolers maintaining. Good luck miner.\n", portrait = portrait1 },

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
