using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
  // === Основные настройки ===
[Range(20, 1000)] public int maxSpeed = 400;          // Макс. скорость вперед (км/ч)
[Range(10, 1000)] public int maxReverseSpeed = 80;    // Макс. скорость назад (км/ч)
[Range(1, 100)] public int accelerationMultiplier = 30; // Множитель ускорения

// === Настройки рулевого управления ===
[Range(10, 45)] public int maxSteeringAngle = 40;     // Макс. угол поворота колес
[Range(0.1f, 2f)] public float steeringSpeed = 1.5f;  // Скорость поворота руля

// === Настройки торможения ===
[Range(100, 5000)] public int brakeForce = 2000;      // Сила торможения
[Range(1, 20)] public int decelerationMultiplier = 15; // Множитель замедления

// === Центр масс ===
public Vector3 bodyMassCenter = new Vector3(0, 0.1f, -0.2f); // Смещение центра масс

// === Настройки торможения ===
[Range(0.1f, 5f)] public float brakeSmoothing = 2f; // Плавность торможения
private float currentBrakeForce;                    // Текущая сила торможения

// === Колеса ===
public WheelCollider frontLeftCollider;  // Коллайдер переднего левого колеса
public WheelCollider frontRightCollider; // Коллайдер переднего правого колеса
public WheelCollider rearLeftCollider;   // Коллайдер заднего левого колеса
public WheelCollider rearRightCollider;  // Коллайдер заднего правого колеса

// === 3D модели колес ===
public GameObject frontLeftMesh;  // Модель переднего левого колеса
public GameObject frontRightMesh; // Модель переднего правого колеса
public GameObject rearLeftMesh;   // Модель заднего левого колеса
public GameObject rearRightMesh;  // Модель заднего правого колеса

// === Эффекты ===
public ParticleSystem RLWParticleSystem; // Система частиц для левого заднего колеса
public ParticleSystem RRWParticleSystem; // Система частиц для правого заднего колеса
public TrailRenderer RLWTireSkid;        // След от левого заднего колеса
public TrailRenderer RRWTireSkid;        // След от правого заднего колеса

// === Внутренние переменные ===
private Rigidbody carRigidbody;         // Компонент Rigidbody машины
private bool isLaunching = false;       // Флаг режима запуска
private float launchTimer = 0f;         // Таймер режима запуска
private float steeringAxis;             // Текущий угол поворота (-1..1)
private float throttleAxis;             // Текущий газ (-1..1)
private float driftingAxis;             // Ось заноса
private float localVelocityZ;           // Локальная скорость по Z
private float localVelocityX;           // Локальная скорость по X
private bool deceleratingCar;           // Флаг замедления
private bool isDrifting;                // Флаг заноса
private bool isTractionLocked;          // Флаг блокировки сцепления (ручник)
private float carSpeed;                 // Текущая скорость (км/ч)

public AudioSource carEngineSound; // This variable stores the sound of the car engine.
public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

    void Start()
    {
        if(carEngineSound != null&& MusicSwtch.music){
            initialCarEngineSoundPitch = carEngineSound.pitch;
        }
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        
        // Супер-жесткая подвеска
        SetupSuspension(0.15f, 50000, 6000);
        
        // Увеличенное сцепление для лучшего разгона
        IncreaseGrip();
    }

    void SetupSuspension(float distance, float spring, float damper)
    {
        frontLeftCollider.suspensionDistance = distance;
        frontRightCollider.suspensionDistance = distance;
        rearLeftCollider.suspensionDistance = distance;
        rearRightCollider.suspensionDistance = distance;
        
        JointSpring suspensionSpring = new JointSpring();
        suspensionSpring.spring = spring;
        suspensionSpring.damper = damper;
        suspensionSpring.targetPosition = 0.5f;
        
        frontLeftCollider.suspensionSpring = suspensionSpring;
        frontRightCollider.suspensionSpring = suspensionSpring;
        rearLeftCollider.suspensionSpring = suspensionSpring;
        rearRightCollider.suspensionSpring = suspensionSpring;
    }

    void IncreaseGrip()
    {
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
        forwardFriction.extremumSlip = 0.4f;
        forwardFriction.extremumValue = 1f;
        forwardFriction.asymptoteSlip = 0.8f;
        forwardFriction.asymptoteValue = 0.5f;
        forwardFriction.stiffness = 2f;
        
        frontLeftCollider.forwardFriction = forwardFriction;
        frontRightCollider.forwardFriction = forwardFriction;
        rearLeftCollider.forwardFriction = forwardFriction;
        rearRightCollider.forwardFriction = forwardFriction;
        
    }

    void Update()
    {
        HandleInput();
        UpdateCarData();
        ApplySteering();
        AnimateWheelMeshes();
        DriftCarPS();
        HandleCarSounds();
    }
   
    private bool isBraking = false;
    private float targetSpeed = 0f;

    void FixedUpdate()
    {
        HandlePhysics();
        
        // Автоматическое торможение только если не нажаты клавиши
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !isTractionLocked)
        {
            ApplyAutoBrake();
        }
    }
    void HandlePhysics()
    {
        ApplyThrottle();
        ApplyBraking();
        ApplySteering();
        
        // Ограничение скорости
        LimitSpeed();
    }

    void ApplyAutoBrake()
    {
        // Плавное, но сильное торможение
        float brakePower = Mathf.Lerp(0, brakeForce * 5f, Time.fixedDeltaTime * 10f);
        ApplyBrakeToAllWheels(brakePower);
        ThrottleOff();
        
        // Дополнительное сопротивление
        carRigidbody.linearDamping = 0.5f;
    }
    
    void HandleInput()
    {
        // Управление газом
        if(Input.GetKey(KeyCode.W))
        {
            throttleAxis = Mathf.Min(throttleAxis + Time.deltaTime * 5f, 1f);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            // При движении назад сразу применяем торможение
            ApplyBraking();
            throttleAxis = Mathf.Max(throttleAxis - Time.deltaTime * 5f, -1f);
        }
        else
        {
            throttleAxis = 0;
        }

        // Управление рулем
        if(Input.GetKey(KeyCode.A))
        {
            steeringAxis = Mathf.Clamp(steeringAxis - Time.deltaTime * 25f * steeringSpeed, -1f, 1f);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            steeringAxis = Mathf.Clamp(steeringAxis + Time.deltaTime * 25f * steeringSpeed, -1f, 1f);
        }
        else
        {
            ResetSteeringAngle();
        }

        // Ручной тормоз
        if(Input.GetKey(KeyCode.Space))
        {
            HyperBrake();
        }
        else if(isTractionLocked)
        {
            RecoverTraction();
        }
    }
    void ApplyThrottle()
    {
        if (isTractionLocked) return;
        
        // Сбрасываем сопротивление при нажатии газа
        if(throttleAxis > 0) // Движение вперед
        {
            float torque = accelerationMultiplier * 1000f * throttleAxis;
            ApplyTorqueToAllWheels(torque);
        }
        else if(throttleAxis < 0) // Движение назад
        {
            float reverseTorque = accelerationMultiplier * 500f * throttleAxis;
            ApplyTorqueToAllWheels(reverseTorque);
        }
    }
    void ApplyTorqueToAllWheels(float torque)
    {
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    void ApplyBrakeToAllWheels(float brakePower)
    {
        frontLeftCollider.brakeTorque = brakePower;
        frontRightCollider.brakeTorque = brakePower;
        rearLeftCollider.brakeTorque = brakePower;
        rearRightCollider.brakeTorque = brakePower;
    }

    void ApplySteering()
    {
        float angle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = angle;
        frontRightCollider.steerAngle = angle;
    }

    void UpdateCarData()
    {
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;
    }

    public void DriftCarPS()
    {
        // Проверяем условия для дрифта
        bool shouldDrift = (isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f;
        isDrifting = shouldDrift;

        try 
        {
            // Управление частицами
            if(isDrifting)
            {
                if(RLWParticleSystem != null && !RLWParticleSystem.isPlaying) 
                    RLWParticleSystem.Play();
                if(RRWParticleSystem != null && !RRWParticleSystem.isPlaying) 
                    RRWParticleSystem.Play();
            }
            else
            {
                if(RLWParticleSystem != null && RLWParticleSystem.isPlaying) 
                    RLWParticleSystem.Stop();
                if(RRWParticleSystem != null && RRWParticleSystem.isPlaying) 
                    RRWParticleSystem.Stop();
            }

            // Управление следами
            if(RLWTireSkid != null) 
                RLWTireSkid.emitting = shouldDrift;
            if(RRWTireSkid != null) 
                RRWTireSkid.emitting = shouldDrift;
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    void HandleCarSounds()
    {
        if (carEngineSound != null && MusicSwtch.music)
        {
            float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
            carEngineSound.pitch = engineSoundPitch;

            if (!carEngineSound.isPlaying)
            {
                carEngineSound.Play();
            }
        }

        if (tireScreechSound != null && MusicSwtch.music)
        {
            bool shouldPlayScreech = (isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f));
        
            if (shouldPlayScreech)
            {
                if (!tireScreechSound.isPlaying)
                    tireScreechSound.Play();
            }
            else
            {
                if (tireScreechSound.isPlaying)
                    tireScreechSound.Stop(); // Немедленная остановка
            }
        }
    }

    void ApplyBraking()
    {
        if (isTractionLocked)
        {
            ApplyBrakeToAllWheels(brakeForce * 3f);
            return;
        }
        
        if(Input.GetKey(KeyCode.S))
        {
            // Прогрессивное торможение при удержании S
            float brakePower = Mathf.Lerp(0, brakeForce * 2f, Time.fixedDeltaTime * 5f);
            ApplyBrakeToAllWheels(brakePower);
        }
        else
        {
            // Сбрасываем тормоза если не тормозим
            ApplyBrakeToAllWheels(0);
        }
    }
    void LimitSpeed()
    {
        // Получаем текущую скорость в км/ч
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
    
        if (throttleAxis > 0 && currentSpeed > maxSpeed)
        {
            // Снижаем крутящий момент при приближении к максимальной скорости
            float speedRatio = currentSpeed / maxSpeed;
            float torqueReduction = Mathf.Clamp(1 - (speedRatio - 0.9f) * 10f, 0.1f, 1f);
        
            frontLeftCollider.motorTorque *= torqueReduction;
            frontRightCollider.motorTorque *= torqueReduction;
            rearLeftCollider.motorTorque *= torqueReduction;
            rearRightCollider.motorTorque *= torqueReduction;
        }
        else if (throttleAxis < 0 && currentSpeed > maxReverseSpeed)
        {
            // Аналогично для заднего хода
            float speedRatio = currentSpeed / maxReverseSpeed;
            float torqueReduction = Mathf.Clamp(1 - (speedRatio - 0.9f) * 10f, 0.1f, 1f);
        
            frontLeftCollider.motorTorque *= torqueReduction;
            frontRightCollider.motorTorque *= torqueReduction;
            rearLeftCollider.motorTorque *= torqueReduction;
            rearRightCollider.motorTorque *= torqueReduction;
        }
    }

    void HyperBrake()
    {
        // Теперь просто активируем флаг, торможение будет обрабатываться в ApplyBraking
        isTractionLocked = true;
    }

    void GoReverse()
    {
        // Убираем проверку скорости, просто даем обратный газ
        throttleAxis = Mathf.Clamp(throttleAxis - Time.deltaTime * 5f, -1f, 0f);
    
        float reverseTorque = accelerationMultiplier * 100f * throttleAxis;
        frontLeftCollider.motorTorque = reverseTorque;
        frontRightCollider.motorTorque = reverseTorque;
        rearLeftCollider.motorTorque = reverseTorque;
        rearRightCollider.motorTorque = reverseTorque;
    
        // Отключаем тормоза при движении назад
        frontLeftCollider.brakeTorque = 0;
        frontRightCollider.brakeTorque = 0;
        rearLeftCollider.brakeTorque = 0;
        rearRightCollider.brakeTorque = 0;
    
    }

    void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    void DecelerateCar()
    {
        throttleAxis = Mathf.Lerp(throttleAxis, 0f, Time.deltaTime * 5f);
        float decelerationFactor = 1f / (1f + (0.05f * decelerationMultiplier * Time.deltaTime * 60f));
        carRigidbody.linearVelocity *= decelerationFactor;
        
        if(carRigidbody.linearVelocity.magnitude < 0.5f)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
        }
    }

    void ResetSteeringAngle()
    {
        steeringAxis = Mathf.Lerp(steeringAxis, 0f, Time.deltaTime * 5f);
        ApplySteering(); // Используем новый метод вместо прямого назначения
    }

    void RecoverTraction()
    {
        isTractionLocked = false;
        // Не сбрасываем сразу, позволим ApplyBraking сделать это плавно
    }

    void AnimateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftCollider, frontLeftMesh);
        UpdateWheelMesh(frontRightCollider, frontRightMesh);
        UpdateWheelMesh(rearLeftCollider, rearLeftMesh);
        UpdateWheelMesh(rearRightCollider, rearRightMesh);
    }

    void UpdateWheelMesh(WheelCollider collider, GameObject mesh)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }
}