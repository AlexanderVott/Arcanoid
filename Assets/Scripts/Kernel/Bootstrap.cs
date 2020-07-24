using System.Collections;
using RedDev.Kernel.Managers;
using RedDev.Kernel.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedDev.Kernel
{
	public class Bootstrap : MonoBehaviour
	{
		[Header("Managers")]
		public BaseManager[] preloadManagers;
		
		[Header("Scenes")]
		[Tooltip("Сцены, необходимые на этапе инициализации проекта")]
		public SceneField[] scenesDependsOn;

		IEnumerator Start()
		{
			/*Core.Add<SystemEventsManager>();
			Core.Add<TerminalManager>();
			Core.Add<BundlesManager>();
			Core.Add<PrefsManager>();
			Core.Add<DBManager>();
			Core.Add<CoreScenesManager>();
			Core.Add<ContextManager>();*/
			
			foreach (var man in preloadManagers)
				Core.Add(man);

			Core.AttachManagers();

			yield return LoadingScenes();
		}

		private IEnumerator LoadingScenes()
		{
			if (scenesDependsOn.Length == 0)
				yield break;

			if (!Application.isEditor)
			{
				foreach (var scene in scenesDependsOn)
					yield return SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
			}
			else
			{
				foreach (var scene in scenesDependsOn)
				{
					var checkScene = SceneManager.GetSceneByName(scene.sceneName);
					if (checkScene.buildIndex < 0)
						yield return SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
				}
			}

			// по условиям в редакторе сцена start при тестировании идёт первой в списке иерархии и является активной.
			SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		}
	}
}