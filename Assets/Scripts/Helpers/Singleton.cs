using RedDev.Kernel.Logs;
using UnityEngine;

namespace RedDev.Helpers {
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

		protected static T _instance;
		private static object _lock = new object();

		public static bool changingScene { get; set; } = false;
		private static bool _appIsQuitting = false;
		
		public static T instance {
			get {
				if (_appIsQuitting) {
					LogSystem.Warning($"[Singleton] Instance '{typeof(T)}' already destroyd on application quit. Won't create again - returning null.");
					return null;
				}

				lock (_lock) {
					if (_instance == null)
					{
						var type = typeof(T);
						_instance = (T)FindObjectOfType(type);

						if (FindObjectsOfType(type).Length > 1) {
							LogSystem.Error("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return _instance;
						}

						if (_instance == null) {
							GameObject singleton = new GameObject();
							singleton.name = type.Name;
							_instance = singleton.AddComponent<T>();
							
							LogSystem.instance.Log(AlertLevel.Silence, "Singleton", $"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created wih DontDestroyOnLoad.");
						} else {
							LogSystem.instance.Log(AlertLevel.Silence, "Singleton", $"[Singleton] Using instance already created: {_instance.gameObject.name}");
						}

						if (Application.isPlaying)
							DontDestroyOnLoad(_instance.gameObject);
					}

					return _instance;
				}
			}
		}

		public static bool IsQuittingOrChangingScene()
		{
			return _appIsQuitting || changingScene;
		}

		protected virtual void Initialization()
		{
		}

		protected virtual void Finalization()
		{
		}

		void Awake()
		{
			DontDestroyOnLoad(gameObject);
			Initialization();
		}

		void OnDestroy()
		{
			Finalization();
			_appIsQuitting = true;
		}
	}
}