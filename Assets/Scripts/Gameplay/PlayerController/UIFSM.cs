using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class UIFSM : MonoBehaviour
    {

        private GameObject MenuUI;

        [SerializeField] private Texture2D DefaultCursor;
        [SerializeField] private Texture2D AimingCursor;

        public GameObject player;

        private StarterAssetsInputs _input;

        private enum state
        {
            InGame,
            Menu,
            InGameUI
        }


        private void OnEnable()
        {
            _input = GameObject.FindWithTag("MainPlayer").GetComponent<StarterAssetsInputs>();
            _input.MenuInputCall += MenuCall;
        }

        private void OnDisable()
        {
            _input.MenuInputCall -= MenuCall;
        }

        void MenuCall()
        {
            Debug.Log("menu call");
        }



        /*
        if (newFire2State)
        {
            Cursor.SetCursor(AimingCursor, new Vector2(100,100), CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
        }
        */



    }
}
