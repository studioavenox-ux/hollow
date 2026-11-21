using System;
using UnityEngine;

namespace Hollow.Player
{
    /// <summary>
    /// First-person controller: WASD movement, mouse look, sprint,
    /// gravity, subtle head-bob, footstep events for audio.
    /// Uses the legacy Input API exclusively; Active Input Handling
    /// must include the legacy Input Manager (Master §11.1).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float sprintSpeed = 5.2f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float gravity = -18f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2.2f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Head-bob (subtle)")]
        [SerializeField] private float bobAmplitude = 0.035f;
        [SerializeField] private float bobFrequency = 1.6f;

        [Header("Camera")]
        [SerializeField] private Camera playerCamera;

        /// <summary>Raised on each footstep (for SFX/ambience hooks).</summary>
        public static event Action OnFootstep;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _pitch;
        private float _bobTime;
        private Vector3 _cameraBasePos;
        private bool _controlsEnabled = true;

        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set
            {
                _controlsEnabled = value;
                if (!value) _velocity = Vector3.zero;
            }
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null) _cameraBasePos = playerCamera.transform.localPosition;
        }

        private void Update()
        {
            if (!_controlsEnabled) return;

            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            _pitch = Mathf.Clamp(_pitch - mouseY, minPitch, maxPitch);
            if (playerCamera != null)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool sprinting = Input.GetKey(KeyCode.LeftShift);

            Vector3 inputDir = (transform.right * horizontal + transform.forward * vertical);
            inputDir.y = 0f;
            inputDir.Normalize();

            float targetSpeed = sprinting ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = inputDir * targetSpeed;

            // Smooth acceleration (no per-frame allocations).
            _velocity.x = Mathf.Lerp(_velocity.x, targetVelocity.x, acceleration * Time.deltaTime);
            _velocity.z = Mathf.Lerp(_velocity.z, targetVelocity.z, acceleration * Time.deltaTime);

            // Gravity.
            if (_controller.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);

            // Head-bob + footstep timing.
            float planarSpeed = new Vector2(_velocity.x, _velocity.z).magnitude;
            if (_controller.isGrounded && planarSpeed > 0.5f)
            {
                _bobTime += Time.deltaTime * bobFrequency * planarSpeed;
                float bob = Mathf.Sin(_bobTime * Mathf.PI * 2f) * bobAmplitude;
                if (playerCamera != null)
                {
                    playerCamera.transform.localPosition = _cameraBasePos + Vector3.up * bob;
                }
                if (Mathf.Sin(_bobTime * Mathf.PI * 2f) > 0.98f)
                {
                    OnFootstep?.Invoke();
                }
            }
            else if (playerCamera != null)
            {
                playerCamera.transform.localPosition = _cameraBasePos;
            }
        }
    }
}
