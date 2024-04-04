using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


	public class StarterAssetsInputs : MonoBehaviour
    {
        public GameObject menu;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public float zoom;
		public float cameraRotation;
		public bool jump;
        public bool roll;
        public bool cover;
        public bool meleeAttack;
        public bool walk;
        public bool crouch;
		public bool sprint;
		public bool flashLight;
		public bool pause;
		public bool inventory;
		public bool log;
		public bool use;
		public bool fire1;
		public bool fire2;
		public bool map;
        public bool toss;
        public event Action InventoryInputCall;
        public event Action MenuInputCall;
        public event Action<float> SwitchTabCall;
        public event Action LightCall;


		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		//public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnZoom(InputValue value)
		{
			ZoomInput(value.Get<float>());
		}

		public void OnCameraRotation(InputValue value)
		{
			CameraRotationInput(value.Get<float>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

        public void OnRoll(InputValue value)
        {
            RollInput(value.isPressed);
        }

        public void OnCover(InputValue value)
        {
            CoverInput(value.isPressed);
        }

        public void OnMeleeAttack(InputValue value)
        {
            MeleeAttackInput(value.isPressed);
        }

        public void OnWalk(InputValue value)
        {
            WalkInput(value.isPressed);
        }
        public void OnCrouch(InputValue value)
        {
            CrouchInput(value.isPressed);
        }

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}





		public void OnLog(InputValue value)
		{
			LogInput(value.isPressed);
		}

		public void OnUse(InputValue value)
		{
			UseInput(value.isPressed);
		}

		public void OnFire1(InputValue value)
		{
			Fire1Input(value.isPressed);
		}

		public void OnFire2(InputValue value)
		{
			Fire2Input(value.isPressed);
		}

		public void OnMap(InputValue value)
		{
            MapInput(value.isPressed);
		}

        public void OnInventory(InputValue value)
        {
            InventoryInputCall?.Invoke();
        }

        public void OnLight(InputValue value)
        {
            LightCall?.Invoke();
        }

        public void OnPause(InputValue value)
        {
            //menu.SetActive(!menu.active);
            MenuInputCall?.Invoke();
        }

        public void OnSwitchTab(InputValue value)
        {
            SwitchTabCall?.Invoke(value.Get<float>());
        }


#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}
		public void CameraRotationInput(float newCameraRotationDirection)
		{
			cameraRotation = newCameraRotationDirection;
		}

		public void ZoomInput(float newZoomDirection)
		{
			zoom = newZoomDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

        public void RollInput(bool newRollState)
        {
            roll = newRollState;
        }
        public void CoverInput(bool newCoverState)
        {
            cover = newCoverState;
        }
        public void MeleeAttackInput(bool newMeleeAttackState)
        {
            meleeAttack = newMeleeAttackState;
        }

        public void WalkInput(bool newWalkState)
        {
            if (newWalkState)
                walk = !walk;
        }
        public void CrouchInput(bool newCrouchState)
        {
            if (newCrouchState)
                crouch = !crouch;
        }
		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
        public void LogInput(bool newLogState)
		{
			log = newLogState;
		}
		public void UseInput(bool newUseState)
		{
			use = newUseState;
		}
		public void Fire1Input(bool newFire1State)
		{
			fire1 = newFire1State;
		}
		public void Fire2Input(bool newFire2State)
		{
			fire2 = newFire2State;
		}
		public void MapInput(bool newMapState)
		{
			map = newMapState;
		}

		/*private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}*/





	}

