using UnityEngine;
using StarterAssets;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject deathPanel;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Health: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log("Health: " + currentHealth + "/" + maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");

        ThirdPersonController controller =
            GetComponent<ThirdPersonController>();

        if (controller != null)
            controller.enabled = false;

        StarterAssetsInputs inputs =
            GetComponent<StarterAssetsInputs>();

        if (inputs != null)
        {
            inputs.cursorLocked = false;
            inputs.cursorInputForLook = false;
            inputs.enabled = false;
        }

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}