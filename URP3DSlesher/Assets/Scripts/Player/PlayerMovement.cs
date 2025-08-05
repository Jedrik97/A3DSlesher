using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CustomJoystick joystick;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private PlayerStats stats;

    private CharacterController controller;
    private Vector3 currentVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = joystick.InputDirection;
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);

        float targetSpeed = (5f + stats.Agility * 0.1f) * inputDir.magnitude;
        Vector3 desiredVelocity = inputDir.normalized * targetSpeed;

        if (inputDir.magnitude > 0.01f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputDir), Time.deltaTime * 10f);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        controller.Move(currentVelocity * Time.deltaTime);
    }
}
