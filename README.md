# Unity Labs – Third-Person Rooms Project

A Unity 6 (URP) third-person project built on the Starter Assets Third Person Controller.
The playable scene is `Assets/Starter Assets/Sample/ThirdPersonController/Playground.unity`, which has two rooms (Room-1 and Room-2) connected by a hall.

All custom scripts are in `Assets/Scripting/` and custom prefabs are in `Assets/Prefabs/`.

## Controls

| Input | Action |
|---|---|
| WASD / Mouse | Move / look around |
| Space | Jump |
| Shift | Sprint |
| I or the **Inventory** button | Open / close the inventory |
| E (near a person) | Talk |
| E / Space / Enter / Click | Continue dialogue |

---

# Lab-3

Lab 3 adds an inventory system, an inventory HUD, and two people the player can talk to.

## 1. Item Storage (Inventory)

**Script:** `Inventory.cs` (attached to the player)

- The player can carry **up to 5 items**.
- The player can carry **only one Book** at a time.
- When an item can't be picked up, a red warning appears at the top of the screen for a few seconds:
  - *"Inventory is full! Can't pick up Health Pack."*
  - *"You can only carry one Book!"*
- If an item can't be picked up, it stays in the world so the player can come back for it.

### Collectible items

| Item | Where | Script | Notes |
|---|---|---|---|
| **Health Pack** | Spawned randomly by `RandomSpawner` | `HealthPack.cs` | Restores health when used from the inventory |
| **The Book of Hogwarts** | On top of the blue cube in Room-1 | `BookPickup.cs` | Only one can be carried; cannot be used or discarded |
| **Elven Bow** | On top of the north-west cylinder in Room-1 | `CollectibleItem.cs` | Extra item with no effect |
| **Elven Sword** | On top of the south-east cylinder in Room-1 (diagonal from the bow) | `CollectibleItem.cs` | Extra item with no effect |

- Hazards (red) still damage the player but are **not** stored in the inventory.
- `CollectibleItem.cs` is a reusable pickup script. The item name is set in the Inspector, so new item types can be added without new code.
- The bow and sword use models from the Elven Weapons asset pack. Only the bow and sword assets were kept, and their materials were converted to URP Lit so they no longer render pink.
- All pickups rotate (`RotateObject.cs`) so they stand out.

## 2. Item Display (Inventory HUD)

**Script:** `InventoryHUD.cs` (attached to the `HUD` canvas)

- An **Inventory (I)** button in the bottom-right corner opens and closes the inventory panel. The **I** key does the same.
- The panel shows **5 slots, each a UI `Button`**. Each slot shows the item name and is colour-coded:
  - green: Health Pack
  - gold: Book
  - blue: other items
  - grey: empty
- Clicking a slot:

| Item clicked | Result |
|---|---|
| Health Pack | Removed from the inventory and the player's health is restored |
| The Book of Hogwarts | **Nothing happens** (the book can't be used or thrown away) |
| Any other item (Elven Bow / Elven Sword) | Removed from the inventory with no effect |

- The slots update immediately whenever an item is added or removed, through the `OnInventoryChanged` event in `Inventory.cs`.
- The mouse cursor stays visible so the button can always be clicked. The camera stops turning while the inventory is open or the mouse is over the HUD.

## 3. People to Talk To

Both people are copies of the player character (`NPC_Person.prefab`) with the player controls removed. `NPCIdle.cs` keeps them in their idle animation and turns them to face the player when nearby. Walk up to either one and **"Press E to talk"** appears on screen.

All dialogue is shown by `DialogueManager.cs`, a reusable dialogue box that displays the speaker's name and text. The player can't move while a conversation is open.

### Conversation One: Old Bartholomew (single message)

**Script:** `NPCSingleMessage.cs` on `NPC_Villager`

He always says the same message, and the player does not reply:

> **Old Bartholomew:** "Ah, another adventurer! Word of advice: the glowing green packs mend your bones, the red ones break them. I learned that the hard way... sixty-seven times."

### Conversation Two: Madam Quillsworth (the Librarian)

**Script:** `NPCBookQuest.cs` on `NPC_Librarian`

She checks the player's inventory every time they talk and plays one of two conversations.

**Player does NOT have the Book (3 lines):**

> **Madam Quillsworth:** "Psst! You there! You haven't by any chance stumbled upon The Book of Hogwarts, have you? Leather cover, gold trim, smells faintly of butterbeer?"
>
> **You:** "Uh... no. I haven't seen any book like that."
>
> **Madam Quillsworth:** "Merlin's beard! Then off you go and find it! It's somewhere in these rooms, and I am NOT explaining to the Headmaster how I lost a 1,000-year-old book."

**Player HAS the Book (5 lines):**

> **Madam Quillsworth:** "Wait... I can sense it. That faint glow, that whiff of butterbeer... You have The Book of Hogwarts, don't you?"
>
> **You:** "I do! Found it lying around in here. It's surprisingly heavy."
>
> **Madam Quillsworth:** "Oh, thank Merlin! Please, may I have it back? The Headmaster checks the shelves at midnight."
>
> **You:** "Here you go. Try not to lose it again." ← *the Book is removed from the inventory here*
>
> **Madam Quillsworth:** "Thank you, thank you! You've saved my job and quite possibly my eyebrows. Ten points to... well, whichever house you're in!"

After handing over the Book, talking to her again plays the "no book" conversation.

## Lab-3 Scripts Summary

| Script | Purpose |
|---|---|
| `Inventory.cs` | Stores up to 5 items, one-book limit, change and warning events |
| `InventoryHUD.cs` | Inventory button, slot buttons, using/discarding items, full-inventory warning |
| `HealthPack.cs` | Health pack pickup into the inventory |
| `BookPickup.cs` | The Book of Hogwarts pickup |
| `CollectibleItem.cs` | Generic pickup (Elven Bow, Elven Sword) |
| `DialogueManager.cs` | Dialogue box and "Press E to talk" prompt |
| `NPCSingleMessage.cs` | Old Bartholomew's single message |
| `NPCBookQuest.cs` | Madam Quillsworth's two Book conversations |
| `NPCIdle.cs` | Keeps NPCs idle and facing the player |
