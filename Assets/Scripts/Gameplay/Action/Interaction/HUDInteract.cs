using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

public class HUDInteract : MonoBehaviour, IInteractable
{

    [SerializeField]
    private GameObject UIToDisplay;

    public bool Interact(ThirdPersonController interactor)
    {
        Debug.Log("Interact with me !");
        if (UIToDisplay != null)
        {
            UIToDisplay.SetActive(true);
        }
        return true;
    }

}
