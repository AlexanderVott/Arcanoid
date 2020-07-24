using RedDev.Game.Managers;
using RedDev.Kernel.Bindings.Components.Base;
using RedDev.Kernel.Bindings.Properties;
using RedDev.Kernel.Bindings.Properties.Base;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace RedDev.Game.Contexts
{
	public class SimpleContext : MonoBehaviour, IBndTarget
	{
		public Property<int> scoreValue = new IntProperty();

		private GameFieldManager gamefield;

		public string GetName()
		{
			return gameObject.name;
		}

		void Start()
		{
			gamefield = Core.Get<GameFieldManager>();
		}

		void Update()
		{
			scoreValue.Value = gamefield.counterDestroyedBlocks;
		}
	}
}