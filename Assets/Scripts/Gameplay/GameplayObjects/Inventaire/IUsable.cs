using System;
using TMPro;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(300)]

public class IUsable : MonoBehaviour
{
    GameObject m_newUISpawn;

    public bool displayName;
    public bool displayButton;
    public bool displayAction;

    public String namePick;
    public String buttonPick;
    public String actionPick;

    public GameObject pickUI;

    public GameObject UIName;
    public TextMeshProUGUI UINameText;

    public GameObject UIButton;
    public TextMeshProUGUI UIButtonText;

    public GameObject UIAction;
    public TextMeshProUGUI UIActionText;

    private RectTransform m_newUISpawnRectTransform;
    private bool m_UIStateActive;

    [Tooltip("World space vertical offset for positioning.")]
    [SerializeField]
    float m_VerticalWorldOffset;

    [Tooltip("Screen space vertical offset for positioning.")]
    [SerializeField]
    float m_VerticalScreenOffset;

    Transform m_TransformToTrack;

    ClientAvatarGuidHandler m_ClientAvatarGuidHandler;
    NetworkAvatarGuidState m_NetworkAvatarGuidState;

    Camera m_Camera;
    Transform m_CanvasTransform;

    Vector3 m_VerticalOffset;
    Vector3 m_WorldPos;


        void Awake()
        {
            var cameraGameObject = GameObject.FindWithTag("MainCamera");
            m_Camera = cameraGameObject.GetComponent<Camera>();

            var canvasGameObject = GameObject.FindWithTag("GameCanvas");
            m_CanvasTransform = canvasGameObject.transform;

            m_VerticalOffset = new Vector3(0f, m_VerticalScreenOffset, 0f);

            TrackGraphicsTransform(transform.gameObject);

            SpawnUIState();



        }

        void OnDisable()
        {
            if (!displayButton)
            {
                return;
            }

            if (m_ClientAvatarGuidHandler)
            {
                m_ClientAvatarGuidHandler.AvatarGraphicsSpawned -= TrackGraphicsTransform;
            }
        }

        void SpawnUIState()
        {
            UINameText.text = namePick;
            UIButtonText.text = buttonPick;
            UIActionText.text = actionPick;

            m_newUISpawn = Instantiate(pickUI, m_CanvasTransform);
            // make in world UI state draw under other UI elements
            m_newUISpawn.transform.SetAsFirstSibling();
            m_newUISpawnRectTransform = m_newUISpawn.GetComponent<RectTransform>();

            UIName.SetActive(false);
            UIButton.SetActive(false);
            UIAction.SetActive(false);
            m_newUISpawn.SetActive(false);
            m_UIStateActive = false;


        }


        void TrackGraphicsTransform(GameObject graphicsGameObject)
        {
            m_TransformToTrack = graphicsGameObject.transform;
        }

        /// <remarks>
        /// Moving UI objects on LateUpdate ensures that the game camera is at its final position pre-render.
        /// </remarks>
        void LateUpdate()
        {
            if (m_UIStateActive && m_TransformToTrack)
            {
                // set world position with world offset added
                m_WorldPos.Set(m_TransformToTrack.position.x,
                    m_TransformToTrack.position.y + m_VerticalWorldOffset,
                    m_TransformToTrack.position.z);

                m_newUISpawnRectTransform.position = m_Camera.WorldToScreenPoint(m_WorldPos) + m_VerticalOffset;
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("MainPlayer"))
            {
                if (displayName)
                {
                    UIName.SetActive(true);
                }

                if (displayButton)
                {
                    UIButton.SetActive(true);
                }

                if (displayAction)
                {
                    UIAction.SetActive(true);
                }

                m_newUISpawn.SetActive(true);
                m_UIStateActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            UIName.SetActive(false);
            UIButton.SetActive(false);
            UIAction.SetActive(false);
            m_newUISpawn.SetActive(false);
            m_UIStateActive = false;
        }
}
