using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
    {
        public PlayerInput playerInput;

        private void OnEnable()
        {
            playerInput.SwitchCurrentActionMap("Player");
        }

    }

