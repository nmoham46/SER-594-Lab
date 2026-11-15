using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The Librarian: has a conversation that depends on whether the player
// is carrying the Book. Walk up and press E to talk.
public class NPCBookQuest : MonoBehaviour
{
    [SerializeField] private string npcName = "Madam Quillsworth";
    [SerializeField] private float talkRange = 2.5f;

    // Conversation 1: the player does NOT have the book (3 lines)
    [SerializeField] private List<DialogueLine> noBookDialogue = new List<DialogueLine>
    {
        new DialogueLine("Madam Quillsworth",
            "Psst! You there! You haven't by any chance stumbled upon The Book of Hogwarts, have you? " +
            "Leather cover, gold trim, smells faintly of butterbeer?"),
        new DialogueLine("You",
            "Uh... no. I haven't seen any book like that."),
        new DialogueLine("Madam Quillsworth",
            "Merlin's beard! Then off you go and find it! It's somewhere in these rooms, " +
            "and I am NOT explaining to the Headmaster how I lost a 1,000-year-old book."),
    };

    private Transform player;
    private Inventory inventory;
    private bool promptShown;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            inventory = p.GetComponentInParent<Inventory>();
            if (inventory == null) inventory = p.GetComponentInChildren<Inventory>();
        }
    }

    void Update()
    {
        if (player == null)
            return;

        DialogueManager dialogue = DialogueManager.Instance;
        bool inRange = Vector3.Distance(player.position, transform.position) <= talkRange;

        bool wantPrompt = inRange && !dialogue.IsTalking;
        if (wantPrompt != promptShown)
        {
            dialogue.SetPrompt(wantPrompt ? "Press E to talk to " + npcName : null);
            promptShown = wantPrompt;
        }

        if (wantPrompt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            promptShown = false;
            Talk(dialogue);
        }
    }

    void Talk(DialogueManager dialogue)
    {
        bool hasBook = inventory != null && inventory.HasBook();

        if (!hasBook)
        {
            dialogue.StartDialogue(noBookDialogue);
        }
        // The "has the book" conversation is added in the next step.
    }
}
