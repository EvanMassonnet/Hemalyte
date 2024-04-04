using System;
using Unity.BossRoom.Gameplay.UserInput;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.BossRoom.Gameplay.Actions
{
    public partial class TargetAction
    {
        private GameObject m_TargetReticule;
        private ulong m_CurrentTarget;
        private ulong m_NewTarget;

        private const float k_ReticuleGroundHeight = 0.2f;

        public override bool OnStartClient(ClientCharacter clientCharacter)
        {
            base.OnStartClient(clientCharacter);
            clientCharacter.serverCharacter.TargetId.OnValueChanged += OnTargetChanged;
            clientCharacter.serverCharacter.GetComponent<ClientInputSender>().ActionInputEvent += OnActionInput;

            return true;
        }

        private void OnTargetChanged(ulong oldTarget, ulong newTarget)
        {
            m_NewTarget = newTarget;
        }

        public override bool OnUpdateClient(ClientCharacter clientCharacter)
        {
            return true;
        }

        /// <summary>
        /// Ensures that the TargetReticule GameObject exists. This must be done prior to enabling it because it can be destroyed
        /// "accidentally" if its parent is destroyed while it is detached.
        /// </summary>
        void ValidateReticule(ClientCharacter parent, NetworkObject targetObject)
        {
            if (m_TargetReticule == null)
            {
                //m_TargetReticule = Object.Instantiate(parent.TargetReticulePrefab);
            }

            bool target_isnpc = targetObject.GetComponent<ITargetable>().IsNpc;
            bool myself_isnpc = parent.serverCharacter.CharacterClass.IsNpc;
            bool hostile = target_isnpc != myself_isnpc;

            //m_TargetReticule.GetComponent<MeshRenderer>().material = hostile ? parent.ReticuleHostileMat : parent.ReticuleFriendlyMat;
        }

        public override void CancelClient(ClientCharacter clientCharacter)
        {
            GameObject.Destroy(m_TargetReticule);

            clientCharacter.serverCharacter.TargetId.OnValueChanged -= OnTargetChanged;
            if (clientCharacter.TryGetComponent(out ClientInputSender inputSender))
            {
                inputSender.ActionInputEvent -= OnActionInput;
            }
        }

        private void OnActionInput(ActionRequestData data)
        {
            //this method runs on the owning client, and allows us to anticipate our new target for purposes of FX visualization.
            if (GameDataSource.Instance.GetActionPrototypeByID(data.ActionID).IsGeneralTargetAction)
            {
                m_NewTarget = data.TargetIds[0];
            }
        }
    }
}

