using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private GameObject effect;
    [SerializeField]
    private Light light;

    [SerializeField]
    private float minIntensity = 50;
    [SerializeField]
    private float maxIntensity = 3000;
    [SerializeField]
    private float speedEffect = 1;


    //private WorldClock clock;
    private bool isOn;

    private float currentTime;
    private void OnEnable()
    {
        //clock = GameObject.FindWithTag("World").GetComponent<WorldClock>();
        LightUp();
    }

    private void FixedUpdate()
    {
        if (isOn)
        {
            light.intensity = minIntensity + maxIntensity/2 + maxIntensity/2 * Mathf.Sin(Mathf.Sin(currentTime));
            currentTime += Time.deltaTime * speedEffect;
        }
    }
    private void LightUp()
    {
        isOn = !isOn;
        effect.SetActive(isOn);
        light.enabled = isOn;
    }
}
