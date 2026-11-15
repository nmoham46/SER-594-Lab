using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// An NPC that always says the same single message when spoken to.
// Walk up to them and press E. The player does not reply.
public class NPCSingleMessage : MonoBehaviour
{
    [SerializeField] private string npcName = "Old Bartholomew";
    [SerializeField, TextArea(2, 4)] private string message =
        "Ah, another adventurer! Word of advice: the glowing green packs mend your bones, " +
        "the red ones break them. I learned that the hard way... sixty-seven times.";
    [SerializeField] private float talkRange = 2.5f;

    private Transform player;
    private bool promptShown;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        DialogueManager dialogue = DialogueManager.Instance;
        bool inRange = Vector3.Distance(player.position, transform.position) <= talkRange;

        // Show / hide the "Press E" prompt
        bool wantPrompt = inRange && !dialogue.IsTalking;
        if (wantPrompt != promptShown)
        {
            dialogue.SetPrompt(wantPrompt ? "Press E to talk to " + npcName : null);
            promptShown = wantPrompt;
        }

        if (wantPrompt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            promptShown = false;
            dialogue.StartDialogue(new List<DialogueLine>
            {
                new DialogueLine(npcName, message)
            });
        }
    }
}
