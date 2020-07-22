using System;
using System.Collections;
using UnityEngine;

namespace RedDev.Kernel.Actors
{
	[RequireComponent(typeof(Actor))]
	public class ComponentActor : MonoBehaviour
	{
		protected Actor _actor;

		public Action<ComponentActor, bool> onChangeActiveState;

		public virtual void SetActive(bool enable)
		{
			this.enabled = enable;
		}

		#region Unity methods
		protected virtual void Awake()
		{
			_actor = GetComponentInParent<Actor>();
			_actor.Add(this);
		}

		void Start()
		{
			OnStart();
		}

		void OnDestroy()
		{
			//OnDeattach();
			OnDestroyed();
		}

		protected virtual void OnEnable()
		{
			OnAttach();
			StartCoroutine(StartNextFrameEnable());
			onChangeActiveState?.Invoke(this, true);
		}

		private IEnumerator StartNextFrameEnable()
		{
			yield return new WaitForEndOfFrame();
			OnNextFrameAfterEnable();
		}

		protected virtual void OnNextFrameAfterEnable()
		{

		}

		protected virtual void OnDisable()
		{
			OnDeattach();
			onChangeActiveState?.Invoke(this, false);
		}
		#endregion

		#region Handlers
		public virtual void OnSetup() { }

		public virtual void OnStart() { }

		public virtual void OnDestroyed() { }

		public virtual void OnAttach() { }

		public virtual void OnDeattach() { }
		#endregion
	}
}