using RedDev.Kernel.Actors;
using UnityEngine;

namespace RedDev.Game
{
	public class RigidBodyData : IComponentData
	{
		public Rigidbody2D body;
		public void Initialize(Actor actor)
		{
			body = actor.GetComponent<Rigidbody2D>();
		}
	}
}