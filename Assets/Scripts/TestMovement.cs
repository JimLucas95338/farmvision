using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float speed = 1f;
    private Vector3 centerPosition;

    void Start()
    {
        centerPosition = transform.position;
    }

    void Update()
    {
        float x = centerPosition.x + radius * Mathf.Cos(Time.time * speed);
        float z = centerPosition.z + radius * Mathf.Sin(Time.time * speed);

        transform.position = new Vector3(x, centerPosition.y, z);
    }
}