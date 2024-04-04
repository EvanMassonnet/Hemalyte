using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.GameplayObjects
{
    public class RagdollManager : MonoBehaviour
    {
        [SerializeField]
        Animator CharAnimator;
        public float KilledDestroyDelaySeconds = 3.0f;
        public Collider DefaultCollider;

        public Light DirLight;
        public GameObject BloodAttach;
        public Transform BloodPosition;
        public GameObject[] BloodFX;

        [SerializeField]
        NetworkLifeState m_NetworkLifeState;
        [SerializeField]
        NetworkHealthState m_NetworkHealthState;

        [Header("Rigidbody")]
        [SerializeField]
        Rigidbody LeftUpperArm_R;
        [SerializeField]
        Rigidbody LeftLowerArm_R;
        [SerializeField]
        Rigidbody RightUpperArm_R;
        [SerializeField]
        Rigidbody RightLowerArm_R;
        [SerializeField]
        Rigidbody LeftUpperLeg_R;
        [SerializeField]
        Rigidbody LeftLowerLeg_R;
        [SerializeField]
        Rigidbody RightUpperLeg_R;
        [SerializeField]
        Rigidbody RightLowerLeg_R;

        [Header("Collider")]
        [SerializeField]
        Collider LeftUpperArm_C;
        [SerializeField]
        Collider LeftLowerArm_C;
        [SerializeField]
        Collider RightUpperArm_C;
        [SerializeField]
        Collider RightLowerArm_C;
        [SerializeField]
        Collider LeftUpperLeg_C;
        [SerializeField]
        Collider LeftLowerLeg_C;
        [SerializeField]
        Collider RightUpperLeg_C;
        [SerializeField]
        Collider RightLowerLeg_C;

        private int effectIdx;

        // Start is called before the first frame update
        void Start()
        {
            m_NetworkLifeState.LifeState.OnValueChanged += OnLifeStateChanged;
            m_NetworkHealthState.HitPoints.OnValueChanged += OnHealthStateChanged;

            effectIdx = BloodFX.Length;
        }


        IEnumerator KilledDestroyProcess()
        {
            yield return new WaitForSeconds(KilledDestroyDelaySeconds);

            GameObject parent = transform.parent.parent.gameObject;
            transform.parent = null;
            Destroy(parent);
            Destroy(this);

        }

        void OnLifeStateChanged(LifeState previousValue, LifeState newValue)
        {
            //Debug.Log(previousValue + "   " + newValue);
            if (newValue == LifeState.Dead)
            {
                //Debug.Log("Player's parent: " + CharAnimator.transform.parent.name);
                GameObject parent = CharAnimator.transform.parent.gameObject;
                CharAnimator.transform.parent = null;
                Destroy(parent);
                //Destroy(CharAnimator);
                DefaultCollider.enabled = false;
                //CharAnimator.SetTrigger("Dead");
                //StartCoroutine(KilledDestroyProcess());
            }
        }

        void OnHealthStateChanged(int previousValue, int newValue)
        {

            var instance = Instantiate(BloodFX[Random.Range(0, effectIdx)], BloodPosition.position, Quaternion.Euler(0, 180, 0));

            //var settings = instance.GetComponent<BFX_BloodSettings>();
            //settings.LightIntensityMultiplier = DirLight.intensity;

            var attachBloodInstance = Instantiate(BloodAttach);
            var bloodT = attachBloodInstance.transform;
            bloodT.position = BloodPosition.position;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(BloodPosition.position + transform.forward);
            //bloodT.Rotate(90, 0, 0);
            //bloodT.transform.parent = nearestBone;
        }

    }
}


