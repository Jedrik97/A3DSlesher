using UnityEngine;

public class EnemyCanvasToCamera : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera не найдена!");
        }
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            Vector3 direction = transform.position - playerCamera.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);


            targetRotation.x = 0;
            targetRotation.z = 0;

            transform.rotation = targetRotation;
        }
    }
}