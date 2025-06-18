using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TerrainGenerator.ExampleSetup
{
    public class GenerationCenterMovable : MonoBehaviour
    {
        public Transform generationCenter;
        public Camera camera;

        private const float minDistanceToCamera = 0.1f;
        [Min(minDistanceToCamera)]
        public float distanceToCamera = 1;
        [Min(0.01f)]
        public float distanceToCameraAdjustingStep = 0.5f;

        private const float minMovementSpeed = 0f;
        [Min(minMovementSpeed)]
        [Tooltip("speed in meters per second")]
        public float movementSpeed = 1;
        [Min(0.01f)]
        public float movementSpeedAdjustingStep = 0.1f;

        private const float minRotationSpeed = 0f;
        [Min(minRotationSpeed)]
        [Tooltip("speed in degrees per second")]
        public float rotationSpeed = 1;
        [Min(0.01f)]
        public float rotationSpeedAdjustingStep = 0.1f;

        public TextMeshProUGUI movementSpeedText;
        public TextMeshProUGUI rotationSpeedText;

        private bool isChangedZoom;
        private bool isChangedMovementSpeed;
        private bool isChangedRotationSpeed;
        private bool isMoved;
        private bool isRotated;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;

            movementSpeedText.text = movementSpeed + "m / s";
            rotationSpeedText.text = rotationSpeed + "d / s";
        }

        void Start()
        {
            SetDistanceToCamera();
        }

        void Update()
        {
            CheckForAdjustingRotationSpeed();

            if (!isChangedRotationSpeed)
            {
                CheckForAdjustingMovementSpeed();

                if (!isChangedMovementSpeed)
                {
                    CheckForAdjustingZoom();

                    if (!isChangedZoom)
                    {
                        CheckForRotation();

                        if (!isRotated)
                        {
                            CheckForMove();
                        }
                    }
                }
            }

            isChangedZoom = false;
            isChangedMovementSpeed = false;
            isChangedRotationSpeed = false;
            isMoved = false;
            isRotated = false;
        }

        private void SetDistanceToCamera()
        {
            Vector3 direction = camera.transform.position - generationCenter.position;

            camera.transform.position = generationCenter.position + direction.normalized * distanceToCamera;
        }

        private void CheckForAdjustingZoom()
        {
            bool isMinusPressed = Keyboard.current.minusKey.IsPressed();
            bool isEqualsPressed = Keyboard.current.equalsKey.IsPressed();

            if (isEqualsPressed)
            {
                Vector3 direction = camera.transform.position - generationCenter.position;

                distanceToCamera = direction.magnitude;
                distanceToCamera -= distanceToCameraAdjustingStep;
                distanceToCamera = Mathf.Clamp(distanceToCamera, minDistanceToCamera, float.MaxValue);

                camera.transform.position = generationCenter.position + direction.normalized * distanceToCamera;

                isChangedZoom = true;
            }
            if (isMinusPressed)
            {
                Vector3 direction = camera.transform.position - generationCenter.position;

                distanceToCamera = direction.magnitude;
                distanceToCamera += distanceToCameraAdjustingStep;

                camera.transform.position = generationCenter.position + direction.normalized * distanceToCamera;

                isChangedZoom = true;
            }
        }

        private void CheckForAdjustingMovementSpeed()
        {
            bool isMinusPressed = Keyboard.current.minusKey.IsPressed();
            bool isEqualsPressed = Keyboard.current.equalsKey.IsPressed();
            bool isLeftCtrlPressed = Keyboard.current.leftCtrlKey.IsPressed();
            
            if (isLeftCtrlPressed)
            {
                if (isEqualsPressed)
                {
                    movementSpeed += movementSpeedAdjustingStep;

                    isChangedMovementSpeed = true;

                    movementSpeedText.text = movementSpeed + "m / s";
                }
                else if (isMinusPressed)
                {
                    movementSpeed -= movementSpeedAdjustingStep;
                    movementSpeed = Mathf.Clamp(movementSpeed, minMovementSpeed, float.MaxValue);

                    isChangedMovementSpeed = true;

                    movementSpeedText.text = movementSpeed + "m / s";
                }
            }
        }

        private void CheckForAdjustingRotationSpeed()
        {
            bool isMinusPressed = Keyboard.current.minusKey.IsPressed();
            bool isEqualsPressed = Keyboard.current.equalsKey.IsPressed();
            bool isLeftCtrlPressed = Keyboard.current.leftCtrlKey.IsPressed();
            bool isLeftAltPressed = Keyboard.current.leftAltKey.IsPressed();
            
            if (isLeftCtrlPressed && isLeftAltPressed)
            {
                if (isEqualsPressed)
                {
                    rotationSpeed += rotationSpeedAdjustingStep;

                    isChangedRotationSpeed = true;

                    rotationSpeedText.text = rotationSpeed + "d / s";
                }
                else if (isMinusPressed)
                {
                    rotationSpeed -= rotationSpeedAdjustingStep;
                    rotationSpeed = Mathf.Clamp(rotationSpeed, minRotationSpeed, float.MaxValue);

                    isChangedRotationSpeed = true;

                    rotationSpeedText.text = rotationSpeed + "d / s";
                }
            }
        }

        private void CheckForMove()
        {
            bool isAPressed = Keyboard.current.aKey.isPressed;
            bool isDPressed = Keyboard.current.dKey.isPressed;
            bool isWPressed = Keyboard.current.wKey.isPressed;
            bool isSPressed = Keyboard.current.sKey.isPressed;

            bool isAnyPressed = isAPressed || isDPressed || isWPressed || isSPressed;

            if (isAnyPressed)
            {
                float xMovement = 0f;
                float zMovement = 0f;

                if (isAPressed) xMovement -= Time.deltaTime * movementSpeed;
                if (isDPressed) xMovement += Time.deltaTime * movementSpeed;
                if (isWPressed) zMovement += Time.deltaTime * movementSpeed;
                if (isSPressed) zMovement -= Time.deltaTime * movementSpeed;

                Vector3 forward = camera.transform.forward;
                forward.y = 0f;
                forward.Normalize();

                Vector3 right = camera.transform.right;
                right.y = 0f;
                right.Normalize();

                Vector3 movement = right * xMovement + forward * zMovement;

                generationCenter.position += movement;
                isMoved = true;
            }
        }

        private void CheckForRotation()
        {
            bool isLeftAltPressed = Keyboard.current.leftAltKey.IsPressed();

            if (isLeftAltPressed)
            {
                bool isAPressed = Keyboard.current.aKey.IsPressed();
                bool isDPressed = Keyboard.current.dKey.IsPressed();
                bool isWPressed = Keyboard.current.wKey.IsPressed();
                bool isSPressed = Keyboard.current.sKey.IsPressed();

                bool isAnyPressed = isAPressed || isDPressed || isWPressed || isSPressed;

                if (isAnyPressed)
                {
                    float verticalRotation = 0f;
                    float horizontalRotation = 0f;

                    if (isAPressed) verticalRotation += Time.deltaTime * rotationSpeed;
                    if (isDPressed) verticalRotation -= Time.deltaTime * rotationSpeed;
                    if (isWPressed) horizontalRotation += Time.deltaTime * rotationSpeed;
                    if (isSPressed) horizontalRotation -= Time.deltaTime * rotationSpeed;

                    Vector3 e = generationCenter.localEulerAngles;

                    float currentPitch = e.x > 180f ? e.x - 360f : e.x;
                    float currentYaw = e.y;

                    float newPitch = Mathf.Clamp(currentPitch + horizontalRotation, -80f, 80f);
                    float newYaw = currentYaw + verticalRotation;

                    generationCenter.localEulerAngles = new Vector3(newPitch, newYaw, e.z);

                    isRotated = true;
                }
            }
        }
    }
}