using RedDev.Kernel.Actors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RedDev.Game
{
	[ComponentData(typeof(PlayerInputData))]
	public class PlayerInputComponent : ComponentActor, ArcanoidInput.IPlayerActions
	{
		[SerializeField]
		private ArcanoidInput _input;

		private PlayerInputData _data;

		public override void OnSetup()
		{
			base.OnSetup();
			_data = _actor.GetData<PlayerInputData>();

			_input = new ArcanoidInput();
			_input.Player.SetCallbacks(this);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_input.Enable();
		}

		protected override void OnDisable()
		{
			_input.Disable();
			base.OnDisable();
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			Debug.Log($"Moving {context.ReadValue<Vector2>()}");
			_data.inputAxis = context.ReadValue<Vector2>();
		}
	}
}