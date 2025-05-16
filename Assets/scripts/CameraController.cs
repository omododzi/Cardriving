using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    
    [Header("Follow Settings")]
    [Range(0.1f, 10f)]
    public float followSpeed = 5f;
    
    [Header("Rotation Settings")]
    [Range(0.1f, 5f)]
    public float rotationSpeed = 1f;
    
    [Header("Smoothing")]
    [Range(0.01f, 0.5f)]
    public float positionSmoothTime = 0.1f;
    
    public float lookAtHeightOffset = 0.5f;

    private Vector3 velocity = Vector3.zero;
    private bool targetFound = false;

    void Start()
    {
        FindTarget();
    }

    void LateUpdate()
    {
        if (!targetFound || target == null)
        {
            FindTarget();
            return;
        }

        FollowTarget();
        RotateTowardsTarget();
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            targetFound = true;
            
            // Устанавливаем начальную позицию камеры
            transform.position = target.position + offset;
            transform.LookAt(target.position + Vector3.up * lookAtHeightOffset);
        }
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + 
                                target.right * offset.x + 
                                target.up * offset.y + 
                                target.forward * offset.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            positionSmoothTime,
            Mathf.Infinity, // Макс. скорость - без ограничений
            Time.deltaTime
        );
    }

    private void RotateTowardsTarget()
    {
        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        Vector3 direction = lookAtPoint - transform.position;
        
        // Используем SmoothDamp и для вращения (более плавно)
        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}