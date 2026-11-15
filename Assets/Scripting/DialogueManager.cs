using System;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)] public string text;

    public DialogueLine(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}

// Shows dialogue boxes and the "Press E to talk" prompt.
// Creates its own screen-space canvas the first time it is used,
// so nothing has to be set up in the scene.
public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    public static DialogueManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("DialogueManager").AddComponent<DialogueManager>();
            return instance;
        }
    }

    public bool IsTalking => lines != null;

    private GameObject dialogueBox;
    private TMP_Text speakerText;
    private TMP_Text bodyText;
    private TMP_Text promptText;

    private List<DialogueLine> lines;
    private int lineIndex;
    private Action onLineShown;
    private Action<int> lineCallback;
    private Action onFinished;
    private int startedFrame;

    private ThirdPersonController playerController;
    private StarterAssetsInputs playerInputs;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        BuildUI();
    }

    // Start a conversation. onLine(i) is called when line i appears,
    // onFinished when the last line is dismissed.
    public void StartDialogue(List<DialogueLine> newLines, Action<int> onLine = null, Action finished = null)
    {
        if (IsTalking || newLines == null || newLines.Count == 0)
            return;

        lines = newLines;
        lineIndex = 0;
        lineCallback = onLine;
        onFinished = finished;
        startedFrame = Time.frameCount;

        SetPrompt(null);
        FreezePlayer(true);
        dialogueBox.SetActive(true);
        ShowLine();
    }

    public void SetPrompt(string message)
    {
        bool show = !string.IsNullOrEmpty(message) && !IsTalking;
        promptText.gameObject.SetActive(show);
        if (show) promptText.text = message;
    }

    void Update()
    {
        if (!IsTalking || Time.frameCount == startedFrame)
            return;

        bool next =
            (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame ||
                                          Keyboard.current.spaceKey.wasPressedThisFrame ||
                                          Keyboard.current.enterKey.wasPressedThisFrame)) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (next)
        {
            lineIndex++;
            if (lineIndex >= lines.Count)
                EndDialogue();
            else
                ShowLine();
        }
    }

    void ShowLine()
    {
        DialogueLine line = lines[lineIndex];
        speakerText.text = line.speaker;
        bodyText.text = line.text;
        lineCallback?.Invoke(lineIndex);
    }

    void EndDialogue()
    {
        Action finished = onFinished;
        lines = null;
        lineCallback = null;
        onFinished = null;
        dialogueBox.SetActive(false);
        FreezePlayer(false);
        finished?.Invoke();
    }

    // Stop the player walking away mid-conversation
    void FreezePlayer(bool freeze)
    {
        if (playerController == null) playerController = FindAnyObjectByType<ThirdPersonController>();
        if (playerInputs == null) playerInputs = FindAnyObjectByType<StarterAssetsInputs>();

        if (playerInputs != null)
        {
            playerInputs.MoveInput(Vector2.zero);
            playerInputs.SprintInput(false);
            playerInputs.JumpInput(false);
        }
        if (playerController != null)
            playerController.enabled = !freeze;
    }

    // ---------- UI construction ----------

    void BuildUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        RectTransform root = (RectTransform)transform;

        // "Press E to talk" prompt (centre, slightly low)
        promptText = CreateText(root, "TalkPrompt", "", 34, TextAlignmentOptions.Center);
        Place(promptText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, -140), new Vector2(900, 60));
        promptText.fontStyle = FontStyles.Bold;
        promptText.outlineWidth = 0.2f;
        promptText.outlineColor = Color.black;
        promptText.gameObject.SetActive(false);

        // Dialogue box (bottom of screen)
        dialogueBox = new GameObject("DialogueBox", typeof(RectTransform), typeof(Image));
        RectTransform box = dialogueBox.GetComponent<RectTransform>();
        box.SetParent(root, false);
        box.anchorMin = box.anchorMax = new Vector2(0.5f, 0);
        box.pivot = new Vector2(0.5f, 0);
        box.anchoredPosition = new Vector2(0, 40);
        box.sizeDelta = new Vector2(1300, 260);
        dialogueBox.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.88f);

        speakerText = CreateText(box, "Speaker", "", 38, TextAlignmentOptions.TopLeft);
        speakerText.color = new Color(1f, 0.82f, 0.35f);
        speakerText.fontStyle = FontStyles.Bold;
        Stretch(speakerText.rectTransform, new Vector2(40, 190), new Vector2(-40, -20));

        bodyText = CreateText(box, "Body", "", 32, TextAlignmentOptions.TopLeft);
        Stretch(bodyText.rectTransform, new Vector2(40, 50), new Vector2(-40, -80));

        TMP_Text hint = CreateText(box, "Hint", "Press E / Click to continue", 22, TextAlignmentOptions.BottomRight);
        hint.color = new Color(1f, 1f, 1f, 0.6f);
        Stretch(hint.rectTransform, new Vector2(40, 15), new Vector2(-30, -200));

        dialogueBox.SetActive(false);
    }

    TMP_Text CreateText(RectTransform parent, string name, string value, float size, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.alignment = align;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }
}
