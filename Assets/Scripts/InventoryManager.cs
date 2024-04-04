using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
    {
        public PlayerInput playerInput;
        public StarterAssetsInputs inputs;
        //public GameObject firstSelectedGameObject;

        public List<GameObject> Tabs;
        public List<Button> TabButton;
        public int currentTab;


        private void OnEnable()
        {
            playerInput.SwitchCurrentActionMap("UI");
            //EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

            inputs.SwitchTabCall += TabSwitch;
        }

        private void OnDisable()
        {
            inputs.SwitchTabCall -= TabSwitch;
        }

        private void TabSwitch(float dir)
        {
            if (dir > 0)
            {
                currentTab = (currentTab + 1) % Tabs.Count;
            }
            if(dir < 0)
            {
                --currentTab;
                if (currentTab < 0)
                {
                    currentTab = Tabs.Count-1;
                }
            }
            SelectedTab(currentTab);
        }

        private void SelectedTab(int select)
        {
            for (int i = 0; i < Tabs.Count; ++i)
            {
                if (i == select)
                {
                    Tabs[i].SetActive(true);
                }
                else
                {
                    Tabs[i].SetActive(false);
                }
            }
        }
    }

