using System;
using System.Collections;
using System.Collections.Generic;
using Unity.BossRoom.Gameplay.GameplayObjects;
using UnityEngine;

public class Input_Player : Input_Abstract
{
    //Input
    private StarterAssetsInputs _input;

    //Character
    private HumanController _humanController;

    //Camera
    private GameObject _mainCamera;
    private CameraController _cameraController;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float WalkSpeed = 1f;

    [Tooltip("Move speed of the character in m/s")]
    public float RunSpeed = 3f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5f;



    private void Start()
    {
        gameObject.tag = "MainPlayer";

        _input = GameObject.FindWithTag("GameController").GetComponent<StarterAssetsInputs>();

        _humanController = GetComponent<HumanController>();

        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _cameraController = GetComponent<CameraController>();

    }

    void Update()
    {
        if (_input.roll)
        {
            //RequestAction(GameDataSource.Instance.PickUpActionPrototype, SkillTriggerStyle.MouseClick);
            RequestAction(GameDataSource.Instance.RollActionPrototype);
            _input.roll = false;
        }

        if (_input.toss)
        {
            //RequestAction(GameDataSource.Instance.PickUpActionPrototype, SkillTriggerStyle.MouseClick);
            //RequestAction(CharacterClass.Skill2);
            _input.toss= false;
        }

        if ( _input.fire1)
        {
            //RequestAction(CharacterClass.Skill1);
        }

        Move();
    }


    private void Move()
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        //Mathf.Round(_mainCamera.transform.eulerAngles.y)
        inputDirection = Quaternion.AngleAxis(Mathf.Round(_mainCamera.transform.eulerAngles.y), Vector3.up) *
                         inputDirection;
        float targetSpeed = _input.sprint ? SprintSpeed : (_input.walk ? WalkSpeed : RunSpeed);
        targetSpeed = _input.fire2 ? WalkSpeed : targetSpeed;

        _humanController.Move(inputDirection, targetSpeed);
    }
}
