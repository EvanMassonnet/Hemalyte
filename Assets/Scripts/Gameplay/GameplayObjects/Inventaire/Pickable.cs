using System;
using TMPro;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(300)]
public class Pickable : IUsable
{

    public float Weight;

    public int Cost;

    public Sprite Icon;

    [Multiline]
    public int Description;

    public GameObject prefab;


}
