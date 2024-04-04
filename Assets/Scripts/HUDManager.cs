using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour
    {
        public GameObject player;
        public StarterAssetsInputs inputs;

        public GameObject menu;
        public GameObject inventory;
        public GameObject inGameHUD;

        private void Awake()
        {
            menu.SetActive(false);
            inventory.SetActive(false);
            inGameHUD.SetActive(true);
        }

        private void OnEnable()
        {
            inputs.InventoryInputCall += DisplayInventory;
            inputs.MenuInputCall += DisplayMenu;

            InputSystem.onActionChange += (obj, change) =>
            {
                if (change == InputActionChange.ActionPerformed)
                {
                    var inputAction = (InputAction) obj;
                    var lastControl = inputAction.activeControl;
                    var lastDevice = lastControl.device;

                    //Debug.Log($"device: {lastDevice.displayName}");
                }
            };
        }

        private void OnDisable()
        {
            inputs.InventoryInputCall -= DisplayInventory;
            inputs.MenuInputCall -= DisplayMenu;
        }

        private void DisplayInventory()
        {
            if (inventory.activeSelf)
            {
                DiplayInGameUI();
                return;
            }
            menu.SetActive(false);
            inventory.SetActive(true);
            inGameHUD.SetActive(false);
        }

        private void DisplayMenu()
        {
            if (menu.activeSelf)
            {
                inputs.InventoryInputCall += DisplayInventory;
                DiplayInGameUI();
                return;
            }
            inputs.InventoryInputCall -= DisplayInventory;
            menu.SetActive(true);
            inventory.SetActive(false);
            inGameHUD.SetActive(false);
        }

        private void DiplayInGameUI()
        {
            menu.SetActive(false);
            inventory.SetActive(false);
            inGameHUD.SetActive(true);
        }

    }

