using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonFishingController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;


        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        private float _cinemachineTargetPitch;


        // player
        private float _speed;
        private float _rotationVelocity;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
        private InputActionMap _playerActionMap = _playerInput.currentActionMap;
        private InputAction _moveAction;
        private InputAction _fishingAction;
#endif


        private CharacterController _controller;


        private const float _threshold = 0.01f;


        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
return false;
#endif
            }
        }


        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (_controller == null) Debug.LogError("[FirstPersonFishingController] CharacterController missing.");


#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null)
            {
                Debug.LogError("[FirstPersonFishingController] PlayerInput component missing. Add one and assign your InputActions asset with an action map named 'Player'.");
            }
#endif


            if (CinemachineCameraTarget == null)
            {
                Debug.LogWarning("[FirstPersonFishingController] CinemachineCameraTarget not assigned.");
            }
        }


        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            if (_playerInput != null && _playerInput.actions != null)
            {
                _playerActionMap = _playerInput.actions.FindActionMap("Player", false);
                if (_playerActionMap == null)
                {
                    Debug.LogError("[FirstPersonFishingController] ActionMap 'Player' not found in PlayerInput.actions.");
                    return;
                }


                _moveAction = _playerActionMap.FindAction("Move", false);
                _fishingAction = _playerActionMap.FindAction("Fishing", false);

#endif

            }
        }
    }
}
