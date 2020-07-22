using UnityEngine;

namespace RedDev.Kernel.Managers
{
	public abstract class BaseManager : MonoBehaviour
	{
		public bool initialized { get; protected set; } = false;

		public virtual void Attach()
		{
			initialized = true;
		}
	}
}