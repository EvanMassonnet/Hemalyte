using System;
using System.Collections.Generic;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// This represents a "charge-across-the-screen" attack. The character deals damage to every enemy hit.
    /// </summary>
    /// <remarks>
    /// It's called "Trample" instead of "Charge" because we already use the word "charge"
    /// to describe "charging up" an attack.
    /// </remarks>
    [CreateAssetMenu(menuName = "BossRoom/Actions/Roll Action")]
    public partial class RollAction : Action
    {
        //public StunnedAction StunnedActionPrototype;

        /// <summary>
        /// This is an internal indicator of which stage of the Action we're in.
        /// </summary>
        private enum ActionStage
        {
            Roll,
            Complete,   // ending action
        }


        /// <summary>
        /// Our ActionStage, as of last Update
        /// </summary>
        private ActionStage m_PreviousStage;


        public override bool OnStart(ServerCharacter serverCharacter)
        {
            m_PreviousStage = ActionStage.Roll;


            serverCharacter.physicsWrapper.Transform.LookAt(m_Data.Direction);

            // reset our "stop" trigger (in case the previous run of the trample action was aborted due to e.g. being stunned)
            if (!string.IsNullOrEmpty(Config.Anim2))
            {
                serverCharacter.serverAnimationHandler.NetworkAnimator.ResetTrigger(Config.Anim2);
            }
            // start the animation sequence!
            if (!string.IsNullOrEmpty(Config.Anim))
            {
                serverCharacter.serverAnimationHandler.NetworkAnimator.SetTrigger(Config.Anim);
            }

            serverCharacter.clientCharacter.RecvDoActionClientRPC(Data);
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            m_PreviousStage = default;
        }

        private ActionStage GetCurrentStage()
        {
            float timeSoFar = Time.time - TimeStarted;
            if (timeSoFar < Config.ExecTimeSeconds)
            {
                return ActionStage.Roll;
            }
            if (timeSoFar < Config.DurationSeconds)
            {
                return ActionStage.Roll;
            }
            return ActionStage.Complete;
        }

        public override bool OnUpdate(ServerCharacter clientCharacter)
        {
            ActionStage newState = GetCurrentStage();
            if (newState != m_PreviousStage && newState == ActionStage.Roll)
            {
                // we've just started to charge across the screen! Anyone currently touching us gets hit
                //SimulateCollisionWithNearbyFoes(clientCharacter);

                clientCharacter.Movement.StartForwardCharge(Config.MoveSpeed, Config.DurationSeconds - Config.ExecTimeSeconds);
            }

            m_PreviousStage = newState;
            return newState != ActionStage.Complete;
        }

        public override void Cancel(ServerCharacter serverCharacter)
        {
            if (!string.IsNullOrEmpty(Config.Anim2))
            {
                serverCharacter.serverAnimationHandler.NetworkAnimator.SetTrigger(Config.Anim2);
            }
        }

        public override void End(ServerCharacter serverCharacter)
        {
            base.End(serverCharacter);
            serverCharacter.serverAnimationHandler.NetworkAnimator.ResetTrigger(Config.Anim);
        }
    }
}
