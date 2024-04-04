using Unity.Netcode;
using UnityEngine;

namespace Unity.Multiplayer.Samples.BossRoom
{
    public enum MindState
    {
        Normal,
        Vigilant,
        Fighting,
        Escape,
    }
    /// <summary>
    /// MonoBehaviour containing only one NetworkVariable of type LifeState which represents this object's life state.
    /// </summary>
    public class NetworkMindState : NetworkBehaviour
    {
        [SerializeField]
        NetworkVariable<MindState> m_MindState = new NetworkVariable<MindState>(BossRoom.MindState.Normal);

        public NetworkVariable<MindState> MindState => m_MindState;

    }
}
