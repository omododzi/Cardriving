using System;
using UnityEngine;

public class ExtremeCarController : MonoBehaviour
{
    // Экстремальные настройки
    [Header("Extreme Performance")]
    [Range(20, 1000)] public int maxSpeed = 400;
    [Range(10, 1000)] public int maxReverseSpeed = 80;
    [Range(1, 100)] public int accelerationMultiplier = 30;
    [Space(10)]
    [Range(10, 45)] public int maxSteeringAngle = 40;
    [Range(0.1f, 2f)] public float steeringSpeed = 1.5f;
    [Space(10)]
    [Range(100, 5000)] public int brakeForce = 2000;
    [Range(1, 20)] public int decelerationMultiplier = 15;
    [Range(1, 20)] public int handbrakeDriftMultiplier = 10;
    [Space(10)]
    public Vector3 bodyMassCenter = new Vector3(0, 0.1f, -0.2f);

    [Header("Launch Control")]
    public float launchRPM = 5000f;
    public float launchTorqueBoost = 3f;
    public float launchDuration = 0.5f;
    public float launchImpulseForce = 10000f;
    [Header("Braking Settings")]
    [Range(0.1f, 5f)] public float brakeSmoothing = 2f; // Параметр плавности торможения
    private float currentBrakeForce; 

    [Header("Wheels")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;
    public GameObject frontLeftMesh;
    public GameObject frontRightMesh;
    public GameObject rearLeftMesh;
    public GameObject rearRightMesh;

    // Приватные переменные
    private Rigidbody carRigidbody;
    private bool isLaunching = false;
    private float launchTimer = 0f;
    private float steeringAxis;
    private float throttleAxis;
    private float driftingAxis;
    private float localVelocityZ;
    private float localVelocityX;
    private bool deceleratingCar;
    private bool isDrifting;
    private bool isTractionLocked;
    private float carSpeed;

    void Start()
    {
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
        ApplyBraking();
        HandleInput();
        UpdateCarData();
        ApplySteering(); // Добавляем вызов метода поворота колес
        AnimateWheelMeshes();
    }
    void HandleInput()
    {
        // Управление газом/тормозом
        if(Input.GetKey(KeyCode.W))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            throttleAxis = Mathf.Min(throttleAxis + Time.deltaTime * 5f, 1f); // Плавное увеличение газа
            ApplyThrottle(throttleAxis);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            throttleAxis = Mathf.Max(throttleAxis - Time.deltaTime * 5f, -1f); // Плавное увеличение обратного газа
            ApplyThrottle(throttleAxis);
        }
        else
        {
            ThrottleOff();
            if(!deceleratingCar)
            {
                InvokeRepeating("DecelerateCar", 0f, 0.1f);
                deceleratingCar = true;
            }
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
    void ApplyThrottle(float throttleInput)
    {
        if(throttleInput > 0) // Движение вперед
        {
            float torque = accelerationMultiplier * 1000f * throttleInput;
            frontLeftCollider.motorTorque = torque;
            frontRightCollider.motorTorque = torque;
            rearLeftCollider.motorTorque = torque;
            rearRightCollider.motorTorque = torque;
        
            // Сбрасываем тормоза
            frontLeftCollider.brakeTorque = 0;
            frontRightCollider.brakeTorque = 0;
            rearLeftCollider.brakeTorque = 0;
            rearRightCollider.brakeTorque = 0;
        }
        else if(throttleInput < 0) // Движение назад
        {
            float reverseTorque = accelerationMultiplier * 500f * throttleInput; // Меньше мощности для заднего хода
            frontLeftCollider.motorTorque = reverseTorque;
            frontRightCollider.motorTorque = reverseTorque;
            rearLeftCollider.motorTorque = reverseTorque;
            rearRightCollider.motorTorque = reverseTorque;
        
            // Сбрасываем тормоза
            frontLeftCollider.brakeTorque = 0;
            frontRightCollider.brakeTorque = 0;
            rearLeftCollider.brakeTorque = 0;
            rearRightCollider.brakeTorque = 0;
        }
    
        // Контроль заноса
        isDrifting = Mathf.Abs(localVelocityX) > 3f;
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

    void HyperAccelerate()
    {
        // Система запуска с импульсом
        if(carSpeed < 20f && throttleAxis > 0.9f && !isLaunching)
        {
            isLaunching = true;
            launchTimer = 0f;
            carRigidbody.AddForce(transform.forward * launchImpulseForce, ForceMode.Impulse);
        }
        
        float torque = accelerationMultiplier * 1000f * (isLaunching ? launchTorqueBoost : 1f);
        
        frontLeftCollider.motorTorque = torque;
        frontRightCollider.motorTorque = torque;
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
        
        // Быстрое нарастание газа
        throttleAxis = Mathf.Min(throttleAxis + Time.deltaTime * 10f, 1f);
        
        // Контроль заноса
        if(Mathf.Abs(localVelocityX) > 3f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
        
        if(isLaunching)
        {
            launchTimer += Time.deltaTime;
            if(launchTimer >= launchDuration)
            {
                isLaunching = false;
            }
        }
    }
    void ApplyBraking()
    {
        float targetBrakeForce = 0f;
        
        if(isTractionLocked)
        {
            targetBrakeForce = brakeForce * 3f;
        }
        else if(localVelocityZ > 1f && Input.GetKey(KeyCode.S)) // Торможение при движении назад
        {
            targetBrakeForce = brakeForce * 0.7f;
        }
        
        // Плавное изменение силы торможения
        currentBrakeForce = Mathf.Lerp(currentBrakeForce, targetBrakeForce, Time.deltaTime * brakeSmoothing);
        
        // Применяем торможение ко всем колесам
        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
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