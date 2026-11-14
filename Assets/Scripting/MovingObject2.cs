using UnityEngine;

public class MovingObject2 : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float speed = 2f;
    public float distance = 3f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(startPosition, transform.position) >= distance)
        {
            direction = -direction;
        }
    }
}