using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) >= distance)
        {
            direction *= -1;
        }
    }
}