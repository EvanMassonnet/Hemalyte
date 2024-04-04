using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public class CameraController : NetworkBehaviour
{
        public GameObject mainCamera;
        public Transform cinemachineCameraTarget;
        public Transform aimingCameraTarget;

        public float minCameraZoomDistance = 0.0f;
        public float maxCameraZoomDistance = 12.0f;
        public float defaultCameraZoomDistance = 12.0f;
        public float zoomSensitivity = 1.0f;
        public AnimationCurve rotationAnimationCurve;

        public Vector3 mouseWorldPosition;
        public float mouseAngle;

        //Control
        private StarterAssetsInputs _input;

        // cinemachine
        private CinemachineVirtualCamera _cinemachineCamera;
        private CinemachineFramingTransposer _cinemachine3RdPersonFollow;


        //Target
        private float _currentCameraDistance;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        //Animation
        private float _animState = 0.0f;
        private float _direction = 0;

        //Post processing
        //private Volume _Volume;
        //private Vignette _vignette;

        private Camera m_mainCamera;

        private Vector2 targetPosition = Vector2.zero;

        private int layer = 1 << 8;

        private PlayerInput _playerInput;


        // Start is called before the first frame update
        void Start()
        {
            _input = GameObject.FindWithTag("GameController").GetComponent<StarterAssetsInputs>();
            _playerInput = GameObject.FindWithTag("GameController").GetComponent<PlayerInput>();

            _cinemachineCamera = mainCamera.GetComponent<CinemachineVirtualCamera>();
            _cinemachine3RdPersonFollow = _cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            //_Volume =  GameObject.FindGameObjectWithTag("Post-Processing").GetComponent<Volume>();
            //VolumeProfile profile = _Volume.sharedProfile;
            /*if (!profile.TryGet<Vignette>(out var _vignette))
            {
                _vignette = profile.Add<Vignette>(false);
            }*/

            m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            if (!IsOwner)
            {
                mainCamera.SetActive(false);
                enabled = false;
            }

            _cinemachine3RdPersonFollow.m_CameraDistance = defaultCameraZoomDistance;

            _currentCameraDistance = _cinemachine3RdPersonFollow.m_CameraDistance;
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _cinemachineTargetPitch = cinemachineCameraTarget.transform.rotation.eulerAngles.x;

        }


        void LateUpdate()
        {

            if(_input.cameraRotation != 0 && _direction == 0)
            {
                _direction = _input.cameraRotation;
            }


            if( _input.zoom != 0){
                _currentCameraDistance = Mathf.Clamp(_currentCameraDistance + _input.zoom * zoomSensitivity / -100, minCameraZoomDistance, maxCameraZoomDistance);
                _cinemachine3RdPersonFollow.m_CameraDistance = _currentCameraDistance;
            }

            CameraPosition();
            CameraRotation();

            //rigTarget.position = mouseWorldPosition;
            //Debug.Log(mouseWorldPosition + "    " + _input.look);

        }


        private void CameraRotation()
        {
            if (_input.fire2)
            {
                if (_direction != 0 && _animState < 1.0F)
                {
                    _animState += 0.03f;
                    aimingCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,_cinemachineTargetYaw + rotationAnimationCurve.Evaluate(_animState) * -90 * _direction, 0.0f);

                }
                else if(_direction != 0)
                {
                    _cinemachineTargetYaw += -90 * _direction;
                    _direction = 0;
                    _animState = 0.0f;
                }
                else
                {
                    aimingCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,_cinemachineTargetYaw, 0.0f);
                }
            }
            else
            {
                if (_direction != 0 && _animState < 1.0F)
                {
                    _animState += 0.03f;
                    cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,_cinemachineTargetYaw + rotationAnimationCurve.Evaluate(_animState) * -90 * _direction, 0.0f);

                }
                else if(_direction != 0)
                {
                    _cinemachineTargetYaw += -90 * _direction;
                    _direction = 0;
                    _animState = 0.0f;
                }
                else
                {
                    cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,_cinemachineTargetYaw, 0.0f);
                }
            }


        }

        private void CameraPosition()
        {

            if (_input.fire2)
            {
                if (_playerInput.currentControlScheme == "KeyboardMouse")
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
                    if (Physics.Raycast(ray, out hit, 1000, layer))
                    {
                        //Debug.Log(ray.direction);
                        //Debug.DrawLine(ray.origin,  hit.point, Color.green);
                        mouseWorldPosition = hit.point - 1.8f * ray.direction;
                        //Debug.Log(hit.transform.name + "     " + hit.point);
                        aimingCameraTarget.position = Vector3.Lerp (hit.point, transform.position, 0.7f);
                    }

                    mouseAngle = Mathf.Atan2((mouseWorldPosition - transform.position).x, (mouseWorldPosition - transform.position).z) * Mathf.Rad2Deg + m_mainCamera.transform.eulerAngles.y;
                }
                else
                {
                    if (_input.look.x != 0 || _input.look.y != 0)
                    {
                        mouseAngle = Mathf.Atan2(_input.look.x, _input.look.y) * Mathf.Rad2Deg;
                    }
                    aimingCameraTarget.position = transform.position;
                }



                //aimingCameraTarget.position = new Vector3(aimingCameraTarget.position.x, cinemachineCameraTarget.position.y, aimingCameraTarget.position.z);
                _cinemachineCamera.m_Follow = aimingCameraTarget;
            }
            else
            {
                _cinemachineCamera.m_Follow = cinemachineCameraTarget;
            }

        }
    }

