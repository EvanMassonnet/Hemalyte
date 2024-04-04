
using StarterAssets;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(400)]
    public class UIInventoryHandler : MonoBehaviour
    {
        [SerializeField] UIRadialMenu m_UIRadialPrefab;

        // spawned in world (only one instance of this)
        UIRadialMenu m_UIRadial;

        bool m_UIRadialActive;
        private Transform m_CanvasTransform;

        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;



        //acces a l'inventaire


        private void OnEnable()
        {
            _input = GameObject.FindWithTag("GameController").GetComponent<StarterAssetsInputs>();
            _playerInput = GameObject.FindWithTag("GameController").GetComponent<PlayerInput>();

            var canvasGameObject = GameObject.FindWithTag("GameCanvas");
            if (canvasGameObject)
            {
                m_CanvasTransform = canvasGameObject.transform;
            }

            Assert.IsNotNull(m_CanvasTransform);

            SpawnUIState();

            //_input.InventoryInputCall += InventoryCall;
        }

        void SpawnUIState()
        {
            m_UIRadial = Instantiate(m_UIRadialPrefab, m_CanvasTransform);
            // make in world UI state draw under other UI elements
            m_UIRadial.transform.SetAsFirstSibling();
            //m_UIStateRectTransform = m_UIState.GetComponent<RectTransform>();
            //m_UIRadial.HideInventory();
            //m_UIRadial.DisplayInventory();
        }

        private void OnDestroy()
        {
            if (m_UIRadial != null)
            {
                Destroy(m_UIRadial.gameObject);
            }
            //_input.InventoryInputCall -= InventoryCall;
        }

        private void Update()
        {
            if (m_UIRadial.isDisplay)
            {
                if (_playerInput.currentControlScheme == "KeyboardMouse")
                {
                    var centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
                    Vector2 relative = Input.mousePosition;
                    relative -= centerOfScreen;
                    float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                    m_UIRadial.centerIndicator.rotation = Quaternion.Euler(0, 0, -angle);
                }
                else
                {
                    float angle = Mathf.Atan2(_input.look.x, _input.look.y) * Mathf.Rad2Deg;
                    m_UIRadial.centerIndicator.rotation = Quaternion.Euler(0, 0, -angle);
                }

            }

        }

        void InventoryCall()
        {
            if (m_UIRadialActive)
            {
                m_UIRadial.HideInventory();
            }
            else
            {
                m_UIRadial.DisplayInventory();
            }

            m_UIRadialActive = !m_UIRadialActive;
        }
    }

