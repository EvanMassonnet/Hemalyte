using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public enum MovementState
{
    Idle = 0,
    PathFollowing = 1,
    Charging = 2,
    Knockback = 3,
}



[RequireComponent(typeof(CharacterController))]
public class HumanController : MonoBehaviour
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

    private MovementState m_MovementState;

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

    private void Start()
    {

        _controller = GetComponent<CharacterController>();

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

    }

    private void Update()
    {
        GroundedCheck();
        UpdateGravity();

    }

    public void StartCharge(Vector3 direction, float speed, float duration)
    {

    }

    public void StartKnockback(Vector3 direction, float speed, float duration)
    {

    }

    public bool IsPerformingForcedMovement()
    {
        return m_MovementState == MovementState.Knockback || m_MovementState == MovementState.Charging;
    }

    public bool IsMoving()
    {
        return m_MovementState != MovementState.Idle;
    }

    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
    }


    private void UpdateGravity()
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
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

    private void GroundedCheck()
    {
        /*// set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character

        _animator.SetBool(_animIDGrounded, Grounded);*/

        Grounded = _controller.isGrounded;
    }


    public void Jump()
    {
        if (_jumpTimeoutDelta <= 0.0f && Grounded)
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            // update animator if using character
            _animator.SetBool(_animIDJump, true);
        }

    }

    public void Move(Vector3 targetDirection, float targetSpeed)
    {
        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }




        if (targetDirection != Vector3.zero)
        {

            _targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        }

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    }




}
