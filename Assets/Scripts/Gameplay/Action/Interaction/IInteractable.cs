using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

public interface IInteractable
{

    public bool Interact(ThirdPersonController interactor);

}
