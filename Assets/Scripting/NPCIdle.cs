using UnityEngine;

// Keeps a copy of the player character standing idle as an NPC,
// and turns it to face the player when they come close.
public class NPCIdle : MonoBehaviour
{
    [SerializeField] private float lookAtRange = 5f;
    [SerializeField] private float turnSpeed = 4f;

    private Animator animator;
    private Transform player;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // The Starter Assets controller defaults to "in air"; force idle.
            animator.SetBool("Grounded", true);
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 1f);
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        Quaternion target = startRotation;

        if (player != null)
        {
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude < lookAtRange * lookAtRange && toPlayer.sqrMagnitude > 0.01f)
                target = Quaternion.LookRotation(toPlayer);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    // The shared animation clips fire these events; NPCs don't need sounds,
    // but the receivers must exist to avoid "no receiver" warnings.
    private void OnFootstep(AnimationEvent animationEvent) { }
    private void OnLand(AnimationEvent animationEvent) { }
}
