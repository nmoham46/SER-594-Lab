using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Inventory HUD: an "Inventory" button toggles a panel of 5 slot Buttons.
// Clicking a slot:
//   - Health Pack -> removed, player is healed
//   - Book        -> nothing happens
//   - Anything else -> removed with no effect
// Click the button (or press I) to open/close. The mouse cursor stays visible
// so the button can always be clicked. Camera look pauses while the inventory
// is open or while the mouse is over the HUD buttons.
public class InventoryHUD : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private StarterAssetsInputs playerInputs;
    [SerializeField] private int healAmount = 1;
    [SerializeField] private Key toggleKey = Key.I;

    private GameObject panel;
    private TMP_Text messageText;
    private readonly List<Button> slotButtons = new List<Button>();
    private readonly List<TMP_Text> slotLabels = new List<TMP_Text>();
    private bool isOpen;

    [Header("Warning Popup")]
    [SerializeField] private float warningDuration = 2.5f;
    private TMP_Text warningText;
    private float warningTimer;

    void Start()
    {
        if (inventory == null) inventory = FindAnyObjectByType<Inventory>();
        if (playerHealth == null) playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerInputs == null) playerInputs = FindAnyObjectByType<StarterAssetsInputs>();

        if (inventory == null)
        {
            Debug.LogError("InventoryHUD could not find an Inventory in the scene.", this);
            enabled = false;
            return;
        }

        BuildUI();
        inventory.OnInventoryChanged += Refresh;
        inventory.OnPickupFailed += ShowWarning;
        Refresh();
        SetOpen(false);
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
            inventory.OnPickupFailed -= ShowWarning;
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            SetOpen(!isOpen);

        UpdateCameraLook();

        // Hide the warning after a few seconds
        if (warningTimer > 0f)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
                warningText.gameObject.SetActive(false);
        }
    }

    void ShowWarning(string msg)
    {
        warningText.text = msg;
        warningText.gameObject.SetActive(true);
        warningTimer = warningDuration;
    }

    // Keep the cursor free and only let the mouse turn the camera when
    // the player is not using the inventory UI.
    void UpdateCameraLook()
    {
        if (playerInputs == null || !playerInputs.enabled)
            return;
        if (playerHealth != null && playerHealth.IsDead)
            return;

        if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool allowLook = !isOpen && !overUI;

        if (playerInputs.cursorInputForLook != allowLook)
        {
            playerInputs.cursorInputForLook = allowLook;
            if (!allowLook)
                playerInputs.LookInput(Vector2.zero); // stop the camera drifting
        }
    }

    // ---------- Slot behaviour ----------

    void OnSlotClicked(int index)
    {
        string item = inventory.GetItem(index);
        if (item == null)
            return;

        if (inventory.IsBook(item))
        {
            ShowMessage(item + " is too precious to use.");
            return; // clicking the book does nothing
        }

        inventory.RemoveItemAt(index);

        if (item == "Health Pack")
        {
            if (playerHealth != null)
                playerHealth.Heal(healAmount);
            ShowMessage("Used Health Pack: +" + healAmount + " health");
        }
        else
        {
            ShowMessage("Discarded " + item);
        }
    }

    void Refresh()
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            string item = inventory.GetItem(i);
            slotLabels[i].text = item ?? "Empty";
            slotButtons[i].interactable = item != null;

            Image img = slotButtons[i].GetComponent<Image>();
            img.color = SlotColor(item);
        }
    }

    Color SlotColor(string item)
    {
        if (item == null) return new Color(0.2f, 0.2f, 0.2f, 0.8f);
        if (item == "Health Pack") return new Color(0.2f, 0.6f, 0.25f, 0.95f);
        if (inventory.IsBook(item)) return new Color(0.75f, 0.6f, 0.15f, 0.95f);
        return new Color(0.25f, 0.4f, 0.7f, 0.95f);
    }

    void SetOpen(bool open)
    {
        isOpen = open;
        panel.SetActive(open);
        if (open) ShowMessage("Click an item to use it. Click Inventory again (or press I) to close.");

        // Cursor is never locked, so the Inventory button is always clickable
        if (playerInputs != null)
            playerInputs.cursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateCameraLook();
    }

    void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
    }

    // ---------- UI construction ----------

    void BuildUI()
    {
        RectTransform root = (RectTransform)transform;

        // Warning popup (top-centre, red)
        warningText = CreateText(root, "InventoryWarning", "", 32);
        Place(warningText.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(800, 50), new Vector2(0.5f, 1));
        warningText.color = new Color(1f, 0.3f, 0.3f);
        warningText.fontStyle = FontStyles.Bold;
        warningText.outlineWidth = 0.2f;
        warningText.outlineColor = Color.black;
        warningText.gameObject.SetActive(false);

        // Toggle button (bottom-right)
        Button toggle = CreateButton(root, "InventoryButton", "Inventory (I)",
            new Vector2(1, 0), new Vector2(-20, 20), new Vector2(180, 50), new Vector2(1, 0));
        toggle.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        toggle.onClick.AddListener(() => SetOpen(!isOpen));

        // Panel (bottom-centre)
        panel = new GameObject("InventoryPanel", typeof(RectTransform), typeof(Image));
        RectTransform prt = panel.GetComponent<RectTransform>();
        prt.SetParent(root, false);
        prt.anchorMin = prt.anchorMax = new Vector2(0.5f, 0);
        prt.pivot = new Vector2(0.5f, 0);
        prt.anchoredPosition = new Vector2(0, 90);
        prt.sizeDelta = new Vector2(5 * 150 + 6 * 10, 200);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        TMP_Text title = CreateText(prt, "Title", "Inventory", 26);
        Place(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -8), new Vector2(400, 34), new Vector2(0.5f, 1));

        messageText = CreateText(prt, "Message", "", 18);
        Place(messageText.rectTransform, new Vector2(0.5f, 0), new Vector2(0, 8), new Vector2(700, 28), new Vector2(0.5f, 0));

        for (int i = 0; i < inventory.MaxItems; i++)
        {
            int index = i; // capture for the lambda
            Button slot = CreateButton(prt, "Slot" + (i + 1), "Empty",
                new Vector2(0, 0.5f), new Vector2(10 + i * 160, 0), new Vector2(150, 100), new Vector2(0, 0.5f));
            slot.onClick.AddListener(() => OnSlotClicked(index));
            slotButtons.Add(slot);
            slotLabels.Add(slot.GetComponentInChildren<TMP_Text>());
        }
    }

    Button CreateButton(RectTransform parent, string name, string label,
        Vector2 anchor, Vector2 pos, Vector2 size, Vector2 pivot)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        Place(rt, anchor, pos, size, pivot);

        Button button = go.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        colors.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        button.colors = colors;

        TMP_Text text = CreateText(rt, "Label", label, 18);
        RectTransform trt = text.rectTransform;
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(6, 6);
        trt.offsetMax = new Vector2(-6, -6);
        return button;
    }

    TMP_Text CreateText(RectTransform parent, string name, string value, float size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size, Vector2 pivot)
    {
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }
}
