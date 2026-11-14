using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform player;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text rotationText;

    [Header("Health Blocks")]
    [SerializeField] private GameObject healthBlock1;
    [SerializeField] private GameObject healthBlock2;
    [SerializeField] private GameObject healthBlock3;

    void Update()
    {
        // Health text
        healthText.text =
            "Health: " + playerHealth.CurrentHealth +
            " / " + playerHealth.MaxHealth;

        // Health blocks
        healthBlock1.SetActive(playerHealth.CurrentHealth >= 1);
        healthBlock2.SetActive(playerHealth.CurrentHealth >= 2);
        healthBlock3.SetActive(playerHealth.CurrentHealth >= 3);

        // Player X and Z position
        positionText.text =
            "X: " + player.position.x.ToString("F1") +
            "   Z: " + player.position.z.ToString("F1");

        // Player Y rotation (0-360 degrees)
        float yRotation = player.eulerAngles.y;

        rotationText.text =
            "Y Rotation: " + yRotation.ToString("F0") + "°";
    }
}