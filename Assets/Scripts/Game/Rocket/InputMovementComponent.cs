using RedDev.Kernel.Actors;
using RedDev.Kernel.Interfaces;
using UnityEngine;

namespace RedDev.Game
{
	[ComponentData(typeof(PlayerInputData))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class InputMovementComponent : ComponentActor, ITickFixed
	{
		[SerializeField]
		private float _speed = 1f;

		private PlayerInputData _inputData;
		private Rigidbody2D _body;

		public override void OnSetup()
		{
			base.OnSetup();
			_inputData = _actor.GetData<PlayerInputData>();
			_body = GetComponent<Rigidbody2D>();
		}
		
		public void TickFixed()
		{
			var axis = _inputData.inputAxis * _speed;
			axis.y = 0f;
			_body.velocity = axis;
		}
	}
}