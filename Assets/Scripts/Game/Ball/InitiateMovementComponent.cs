using RedDev.Kernel.Actors;
using UnityEngine;

namespace RedDev.Game.Ball
{
	[ComponentData(typeof(RigidBodyData))]
	public class InitiateMovementComponent : ComponentActor
	{
		private RigidBodyData _bodyData;
		public float initiateSpeed = 1f;
		
		public override void OnStart()
		{
			base.OnStart();
			_bodyData = _actor.GetData<RigidBodyData>();
			_bodyData.body.velocity = Vector2.up * initiateSpeed;
		}
	}
}