using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


public class RadarInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private Animator animator;

    private int _animIDDeploy;
    private int _animIDRetract;

    public bool isDeploy;


    private void Awake()
    {
        _animIDDeploy = Animator.StringToHash("Deploy");
        _animIDRetract = Animator.StringToHash("Retract");
        isDeploy = !isDeploy;
        Interact(null);
        //isDeploy = false;
    }

    public bool Interact(ThirdPersonController interactor)
    {
        Debug.Log("interacted !");
        if (isDeploy)
        {
            Retract();
        }
        else
        {
            Deploy();
        }
        return true;
    }

    private void Retract()
    {
        Debug.Log("call retract");
        animator.SetTrigger(_animIDRetract);
        animator.ResetTrigger(_animIDDeploy);
        isDeploy = false;
    }

    private void Deploy()
    {
        Debug.Log("call deploy");
            animator.SetTrigger(_animIDDeploy);
            animator.ResetTrigger(_animIDRetract);
            isDeploy = true;
    }

}
