
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TestTools;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]

    public class ThirdPersonController : NetworkBehaviour
    {

        public Animator _animator;
        public MultiAimConstraint AimConstraint;
        public Transform rigTarget;


        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkSpeed = 1f;

        [Tooltip("Move speed of the character in m/s")]
        public float RunSpeed = 3f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 3.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // player
        private float _speed;
        private float _animationBlend;
        private float _animationLeftRightBlend;
        private float _animationUpDownBlend;

        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDUpDown;
        private int _animIDLeftRight;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDIsAiming;
        private int _animIDIsCrouching;

        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private CameraController _cameraController;

        private bool isWalking;
        private bool isRunning;
        private bool isSprinting;
        private bool isAiming;
        private bool isCrouching;


        private void AssignAnimationIDs()
        {
            //Movement
            _animIDUpDown = Animator.StringToHash("UpDown");
            _animIDLeftRight = Animator.StringToHash("LeftRight");
            _animIDIsAiming = Animator.StringToHash("isAiming");
            _animIDIsCrouching = Animator.StringToHash("isCrouching");

            //Jump
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");

        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {

            if (!IsOwner)
            {
                enabled = false;
            }

            gameObject.tag = "MainPlayer";

            _controller = GetComponent<CharacterController>();
            _cameraController = GetComponent<CameraController>();
            _input = GameObject.FindWithTag("GameController").GetComponent<StarterAssetsInputs>();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

        }

        private void Update()
        {

            // set target speed based on move speed, sprint speed and if sprint is pressed

            float targetSpeed = 0;
            isAiming = false;

            targetSpeed = _input.sprint ? SprintSpeed : (_input.walk ? WalkSpeed : RunSpeed);
            isAiming = _input.fire2;
            targetSpeed = (isAiming || isCrouching) ? WalkSpeed : targetSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            if (_input.use)
            {
                tryInteract();
                _input.use = false;
            }

            JumpAndGravity();
            GroundedCheck();

            UpdateHorizontalSpeed(targetSpeed);
            Move(targetSpeed, isAiming);

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character

            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void UpdateHorizontalSpeed(float targetSpeed)
        {

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

        }

        private void Move(float targetSpeed, bool isAiming)
        {



            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            isCrouching = _input.crouch && !_input.sprint;
            _animator.SetBool(_animIDIsCrouching, isCrouching);

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

                if (!isAiming)
                {

                    if (_input.move != Vector2.zero)
                    {

                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +_mainCamera.transform.eulerAngles.y;

                        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                        // rotate to face input direction relative to camera position
                        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);



                    }
                    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                    // move the player
                    _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

                }
                else if( isAiming && !inputDirection.Equals(Vector3.zero))
                {

                    float rotation;
                    if (_input.move != Vector2.zero)
                    {
                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +_mainCamera.transform.eulerAngles.y;

                    }
                    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                    // move the player
                    _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


                    _targetRotation = _cameraController.mouseAngle;


                    rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    rigTarget.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    _animationUpDownBlend = Mathf.Lerp(_animationUpDownBlend, Vector3.Dot(transform.forward, new Vector3(_input.move.x, 0, _input.move.y)), Time.deltaTime * SpeedChangeRate);
                    _animationLeftRightBlend = Mathf.Lerp(_animationLeftRightBlend, Vector3.Dot(transform.right, new Vector3(_input.move.x, 0, _input.move.y)), Time.deltaTime * SpeedChangeRate);

                }
                else
                {

                    
                    float rotation;
                    if (_input.move != Vector2.zero)
                    {
                        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +_mainCamera.transform.eulerAngles.y;

                    }
                    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                    // move the player
                    _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


                    _targetRotation = _cameraController.mouseAngle;


                    rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    rigTarget.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    _animationUpDownBlend = Mathf.Lerp(_animationUpDownBlend, Vector3.Dot(transform.forward, new Vector3(_input.move.x, 0, _input.move.y)), Time.deltaTime * SpeedChangeRate);
                    _animationLeftRightBlend = Mathf.Lerp(_animationLeftRightBlend, Vector3.Dot(transform.right, new Vector3(_input.move.x, 0, _input.move.y)), Time.deltaTime * SpeedChangeRate);
                    // float rotation;
                    // if (_input.move != Vector2.zero)
                    // {
                    //     _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +_mainCamera.transform.eulerAngles.y;

                    // }

                    // _targetRotation = Mathf.Atan2((_cameraController.mouseWorldPosition - transform.position).x, (_cameraController.mouseWorldPosition - transform.position).z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;


                    // rotation = Mathf.SmoothDampAngle(rigTarget.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // // rotate to face input direction relative to camera position
                    // //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    // rigTarget.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    // _animationUpDownBlend = 0;
                    // _animationLeftRightBlend = 0;
                }



                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                
                if (isAiming)
                {
                    _animator.SetTrigger(_animIDIsAiming);
                    _animator.SetFloat(_animIDUpDown, _animationUpDownBlend * _animationBlend);
                    _animator.SetFloat(_animIDLeftRight, _animationLeftRightBlend * _animationBlend);
                }
                else
                {
                    _animator.ResetTrigger(_animIDIsAiming);
                    _animator.SetFloat(_animIDUpDown, _animationBlend);
                    _animator.SetFloat(_animIDLeftRight, 0);
                }

                if (isAiming && inputDirection.Equals(Vector3.zero))
                {
                    AimConstraint.weight = 1;
                }
                else
                {
                    AimConstraint.weight = 0;
                }
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);


                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    _animator.SetBool(_animIDJump, true);

                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    _animator.SetBool(_animIDFreeFall, true);

                }

                // if we are not grounded, do not jump or roll
                _input.jump = false;
                _input.roll = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }


        private bool tryInteract()
        {

            var colliders = new Collider[3];
            var numFound =
                Physics.OverlapSphereNonAlloc(transform.position + transform.forward * 0.5f, 0.5f, colliders, 1<<12);

            Debug.Log("try to interact  "+ numFound);
            if (numFound > 0)
            {
                var interactable = colliders[0].GetComponent<IInteractable>();
                if (interactable != null)
                {
                    return interactable.Interact(this);
                }
            }

            return false;
        }


        /*private void Roll()
        {
            if (!Grounded)
            {
                return;
            }

            // Jump
            if (_input.roll && _rollTimeoutDelta <= 0.0f && (_input.move != Vector2.zero))
            {
                _rollTimeoutDelta = RollTimeout;
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                //_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDRoll, true);
                }
                _input.roll = false;
                _input.fire2 = false;
            }
            else
            {
                _input.roll = false;
                _animator.SetBool(_animIDRoll, false);
            }

            // roll timeout
            if (_rollTimeoutDelta >= 0.0f)
            {
                _rollTimeoutDelta -= Time.deltaTime;
            }
        }*/



/*

        private void UpdateFSM()
        {
            isAiming = _input.fire2;
            isSprinting = _input.sprint;
            isWalking = _input.walk;


        }

        public void ReserFSm()
        {
            isWalking = false;
            isRunning = false;
            isSprinting = false;
            isAiming = false;
            isCrouching = false;
        }*/


        /*private void Cover()
        {
            if (_input.move != Vector2.zero)
            {
                _animator.SetLayerWeight(_animator.GetLayerIndex("Cover down"),0);
            }
            RaycastHit hit;
            int layerMask = 1 << 12;
            Vector3 v = transform.position + new Vector3(0, 0.5f, 0);
            //Debug.DrawLine(v, v + transform.forward, Color.red, 5f);
            if (Physics.Raycast(v, transform.forward, out hit, 3, layerMask ))
            {
                //Debug.Log("Cover found : " + hit.transform.name);
                Debug.DrawLine(hit.transform.position,  hit.transform.position + hit.transform.forward, Color.green, 5f);

                if (_input.cover)
                {
                    var targetpos = hit.transform.position; //+ new Vector3( hit.collider.bounds.size.x/2 ,0,0);
                    //Debug.Log(new Vector3(targetpos.x, transform.position.y, targetpos.z) + hit.transform.forward * 0.5f);
                    //Debug.Log("cover  " + (new Vector3(targetpos.x, transform.position.y, targetpos.z) + hit.transform.forward * 0.5f));
                    transform.position = new Vector3(targetpos.x, transform.position.y, targetpos.z) + hit.transform.forward * 0.5f;
                    //transform.position = hit.transform.position;

                    //_controller.Move((targetpos- transform.position).normalized * (_speed * Time.deltaTime));
                    //_controller.transform.position = new Vector3(targetpos.x, transform.position.y, targetpos.z)+ hit.transform.forward * 0.5f;
                    _input.cover = false;

                    if (hit.collider.bounds.size.y > 1)
                    {

                    }
                    else
                    {
                        _animator.SetTrigger("Cover");
                        _animator.SetLayerWeight(_animator.GetLayerIndex("Cover down"),1);
                    }
                }
            }
        }*/



    }
}
