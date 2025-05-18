using UnityEngine;

public class CarStabilizer : MonoBehaviour
{
    [Header("Настройки стабилизации")]
    [Tooltip("Сила возврата в нормальное положение")]
    [Range(0.1f, 10f)] public float stabilizationForce = 2f;
    
    [Tooltip("Максимальный угол для срабатывания (градусы)")]
    [Range(10, 80)] public float maxAngle = 45f;
    
    [Tooltip("Задержка перед стабилизацией (секунды)")]
    [Range(0, 3)] public float stabilizationDelay = 0.5f;
    
    [Tooltip("Использовать ли физическое выравнивание")]
    public bool usePhysicsBased = true;

    private Rigidbody carRigidbody;
    private float airTime = 0f;
    private bool isGrounded = false;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        if (carRigidbody == null)
        {
            Debug.LogError("Rigidbody не найден на машине!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        
        if (!isGrounded)
        {
            airTime += Time.fixedDeltaTime;
            
            if (airTime >= stabilizationDelay)
            {
                StabilizeCar();
            }
        }
        else
        {
            airTime = 0f;
        }
    }

    void CheckGrounded()
    {
        // Проверяем, находятся ли колёса на земле (можно адаптировать под свою систему)
        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.5f);
    }

    void StabilizeCar()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        
        if (angle > maxAngle)
        {
            if (usePhysicsBased)
            {
                // Физическое выравнивание (более реалистичное)
                Vector3 torqueDirection = Vector3.Cross(transform.up, Vector3.up);
                carRigidbody.AddTorque(torqueDirection * stabilizationForce * 10f, ForceMode.Acceleration);
            }
            else
            {
                // Мгновенное выравнивание (менее реалистичное, но стабильное)
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, stabilizationForce * Time.fixedDeltaTime);
            }
            
            // Добавляем небольшую силу вниз для ускорения приземления
            carRigidbody.AddForce(Vector3.down * stabilizationForce * 5f, ForceMode.Acceleration);
        }
    }

   
    
}