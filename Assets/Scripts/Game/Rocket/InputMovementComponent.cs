using RedDev.Kernel.Actors;
using RedDev.Kernel.Interfaces;
using UnityEngine;

namespace RedDev.Game
{
	[ComponentData(typeof(PlayerInputData))]
	public class InputMovementComponent : ComponentActor, ITick
	{
		[SerializeField]
		private float _speed = 1f;

		private PlayerInputData _data;

		public override void OnSetup()
		{
			base.OnSetup();
			_data = _actor.GetData<PlayerInputData>();
		}

		public void Tick()
		{
			Debug.Log(_data.inputAxis);
			var axis = _data.inputAxis * _speed * Time.deltaTime;
			transform.Translate(axis.x, axis.y, 0f);
		}
	}
}