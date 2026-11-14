using UnityEngine;

public class RoomToRoomMover : MonoBehaviour
{
    public float speed = 3f;
    public float travelDistance = 30f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move along the X axis
        transform.Translate(
            Vector3.right * direction * speed * Time.deltaTime,
            Space.World
        );

        // Room 2 reached
        if (transform.position.x >= startPosition.x + travelDistance)
        {
            direction = -1;
        }

        // Room 1 reached
        if (transform.position.x <= startPosition.x)
        {
            direction = 1;
        }
    }
}