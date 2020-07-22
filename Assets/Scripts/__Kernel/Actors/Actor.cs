using System;
using System.Collections.Generic;
using System.Linq;
using RedDev.Kernel.Interfaces;
using RedDev.Kernel.Logs;
using UnityEngine;

namespace RedDev.Kernel.Actors
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ComponentDataAttribute: Attribute
	{
		private Type[] _types;
		public Type[] types => _types;

		public ComponentDataAttribute(params Type[] typesData)
		{
			_types = typesData;
		}
	}

	public interface IComponentData {}

	public class ComponentActorMeta
	{
		public ComponentActor behaviour;
		public object component;

		public ITick tick = null;
		public ITickFixed tickFixed = null;
		public ITickLate tickLate = null;
	}

	public class Actor : MonoBehaviour
	{
		private List<ITick> _ticks = new List<ITick>();
		private List<ITickFixed> _ticksFixed = new List<ITickFixed>();
		private List<ITickLate> _ticksLate = new List<ITickLate>();

		private List<ComponentActorMeta> _behaviours = new List<ComponentActorMeta>();
		private Dictionary<int, ComponentActorMeta> _components = new Dictionary<int, ComponentActorMeta>();
		private Dictionary<int, IComponentData> _data = new Dictionary<int, IComponentData>();

		private int _countTicks;
		private int _countTicksFixed;
		private int _countTicksLate;

		private int _countBehaviours;
		
		#region Ticks
		public void ProcessTick()
		{
			for (int i = 0; i < _countTicks; i++)
				_ticks[i].Tick();
		}

		public void ProcessTickFixed()
		{
			for (int i = 0; i < _countTicksFixed; i++)
				_ticksFixed[i].TickFixed();
		}

		public void ProcessTickLate()
		{
			for (int i = 0; i < _countTicksLate; i++)
				_ticksLate[i].TickLate();
		}
		#endregion

		protected virtual void Awake()
		{
			ConnectData();
			OnSetup();
		}

		void Start()
		{
			OnStart();
		}

		private void Update() => ProcessTick();

		private void FixedUpdate() => ProcessTickFixed();

		private void LateUpdate() => ProcessTickLate();

		#region Add/Remove/Get components
		private void OnChangeActiveStateComponent(ComponentActor beh, bool state)
		{
			var hash = beh.GetHashCode();
			if (_components.ContainsKey(hash))
			{
				var data = _components[hash];
				if (state)
				{
					if (data.tick != null && !_ticks.Contains(data.tick))
					{
						_ticks.Add(data.tick);
						_countTicks++;
					}
					if (data.tickFixed != null && !_ticksFixed.Contains(data.tickFixed))
					{
						_ticksFixed.Add(data.tickFixed);
						_countTicksFixed++;
					}
					if (data.tickLate != null && !_ticksLate.Contains(data.tickLate))
					{
						_ticksLate.Add(data.tickLate);
						_countTicksLate++;
					}
				}
				else
				{
					if (data.tick != null)
					{
						_ticks.Remove(data.tick);
						_countTicks--;
					}
					if (data.tickFixed != null)
					{
						_ticksFixed.Remove(data.tickFixed);
						_countTicksFixed--;
					}
					if (data.tickLate != null)
					{
						_ticksLate.Remove(data.tickLate);
						_countTicksLate--;
					}
				}
			}
			else
			{
				LogSystem.Print(AlertLevel.Error, "System", $"Cell for {beh.GetType().Name}:{hash} not found");
			}
		}

		private ComponentActorMeta AddComponent(ComponentActor beh)
		{
			beh.onChangeActiveState -= OnChangeActiveStateComponent;
			beh.onChangeActiveState += OnChangeActiveStateComponent;

			var compData = new ComponentActorMeta();
			compData.behaviour = beh;
			compData.component = beh;

			if (beh.enabled)
			{
				compData.tick = beh as ITick;
				compData.tickFixed = beh as ITickFixed;
				compData.tickLate = beh as ITickLate;

				if (compData.tick != null)
				{
					_ticks.Add(compData.tick);
					_countTicks++;
				}

				if (compData.tickFixed != null)
				{
					_ticksFixed.Add(compData.tickFixed);
					_countTicksFixed++;
				}

				if (compData.tickLate != null)
				{
					_ticksLate.Add(compData.tickLate);
					_countTicksLate++;
				}
			}

			_behaviours.Add(compData);
			_countBehaviours++;
			return compData;
		}

		private void ConnectData()
		{
			var attributes = Attribute.GetCustomAttributes(GetType()).Select(x => x as ComponentDataAttribute).Where(x => x != null);
			foreach (var attribute in attributes)
				foreach (var type in attribute.types)
					if (!_data.ContainsKey(type.GetHashCode()))
						_data.Add(type.GetHashCode(), Activator.CreateInstance(type) as IComponentData);
		}

		private void ConnectData<T>(T component) where T: ComponentActor
		{
			var attributes = Attribute.GetCustomAttributes(component.GetType()).Select(x=> x as ComponentDataAttribute).Where(x=>x != null);
			foreach (var attribute in attributes)
				foreach (var type in attribute.types)
					if (!_data.ContainsKey(type.GetHashCode()))
						_data.Add(type.GetHashCode(), Activator.CreateInstance(type) as IComponentData);
		}

		private T HandleAdd<T>(T component/*, Type desiredType = null*/) where T: ComponentActor
		{
			//var hash = desiredType == null ? component.GetHashCode() : desiredType.GetHashCode();
			var hash = component.GetHashCode();

			var container = AddComponent(component);

			ConnectData(component);
			
			component.OnSetup();
			//component.enabled = enabled;

			if (!_components.ContainsKey(hash))
				_components.Add(hash, container);

			return component;
		}

		public void Add<T>(T component) where T : ComponentActor
		{
			HandleAdd(component);
		}

		public T Add<T>(bool enableDefault) where T : ComponentActor
		{
			var component = gameObject.AddComponent<T>();
			return HandleAdd(component);
		}

		public T GetOrAdd<T>(bool enableDefault) where T : ComponentActor
		{
			var component = gameObject.GetComponent<T>();
			if (component == null)
				component = gameObject.AddComponent<T>();
			return HandleAdd(component);
		}

		public void Remove<T>(T component) where T: ComponentActorMeta
		{
			_behaviours.Remove(component);
			_countBehaviours--;
		}

		public T Get<T>(int index) where T : ComponentActor
		{
			return index >= 0 && index < _countBehaviours ? _behaviours[index] as T : null;
		}

		public T Get<T>() where T : ComponentActor
		{
			return _components.TryGetValue(typeof(T).GetHashCode(), out var obj)
				? (T) obj.behaviour
				: GetComponentInChildren<T>();
		}
		#endregion

		#region Get Data
		public T GetData<T>() where T: IComponentData
		{
			var hash = typeof(T).GetHashCode();
			if (_data.ContainsKey(hash))
				return (T)_data[hash];
			else
				LogSystem.Print(AlertLevel.Error, "Actor", $"Not found {typeof(T).Name} component data");
			return default(T);
		}
		#endregion

		#region Activation
		protected virtual void OnSetup()
		{
		}

		protected virtual void OnStart()
		{
			for (int i = 0; i < _countBehaviours; i++)
				_behaviours[i].behaviour.OnStart();
		}

		protected virtual void OnEnable()
		{
			//TODO: add to manager update

			/*for (int i = 0; i < _countBehaviours; i++)
			{
				_behaviours[i].OnEnabled();
			}*/
		}

		protected virtual void OnDisable()
		{
			//TODO: remove from manager update
			/*for (int i = 0; i < _countBehaviours; i++)
			{
				_behaviours[i].OnDisabled();
			}*/
		}
		#endregion
	}
}