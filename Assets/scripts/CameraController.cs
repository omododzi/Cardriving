using UnityEngine;

public class CameraController : MonoBehaviour
{
   private Transform target;
   public Vector3 offset;
   [Range(0.1f, 50f)] // Уменьшил максимальное значение
   public float followSpeed = 2f;

   [Tooltip("Скорость поворота")]
   [Range(0.1f, 5f)] // Уменьшил максимальное значение
   public float rotationSpeed = 1f;

   [Header("Advanced")]
   [Tooltip("Сглаживание движения")]
   [Range(0.1f, 1f)] // Увеличил минимальное значение
   public float smoothTime = 1f;

   [Tooltip("Смотреть немного выше цели")]
   public float lookAtHeightOffset = 0.5f;

   private Vector3 velocity = Vector3.zero;

   void Start()
   {
       // Назначаем цель один раз при старте
       target = GameObject.FindGameObjectWithTag("Player").transform;
       if (target == null)
       {
           Debug.LogWarning("Target not assigned for camera follow!");
       }
   }

   void LateUpdate()
   {
       if (target == null)
       {
           Debug.LogWarning("Target not assigned for camera follow!");
           return;
       }

       // Вычисляем желаемую позицию с учетом смещения
       Vector3 desiredPosition = target.position +
                                 target.right * offset.x +
                                 target.up * offset.y +
                                 target.forward * offset.z;

       // Плавное перемещение к позиции
       transform.position = Vector3.SmoothDamp(
           transform.position,
           desiredPosition,
           ref velocity,
           smoothTime,
           followSpeed
       );

       // Вычисляем точку для взгляда (немного выше цели)
       Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;

       // Плавный поворот к цели
       Quaternion desiredRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
       transform.rotation = Quaternion.Slerp(
           transform.rotation,
           desiredRotation,
           rotationSpeed * Time.deltaTime
       );
   }

   // Метод для ручного изменения смещения (например, при смене вида)
   public void SetOffset(Vector3 newOffset)
   {
       offset = newOffset;
   }
}
