using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = System.Random;

namespace Unity.Multiplayer.Samples.BossRoom
{
    public class CamShake : MonoBehaviour
    {
        public CinemachineImpulseSource ImpulseSource;

        private void OnEnable()
        {
            //Debug.Log("call destroy");
            ImpulseSource.GenerateImpulse();

        }

    }
}
