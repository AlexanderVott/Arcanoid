using RedDev.Kernel.Actors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedDev.Game.Tiles
{
	public class DeadZoneTrigger : Actor
	{
		private const string BALL_TAG = "Ball";

		void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.gameObject.tag == BALL_TAG)
			{
				var scene = SceneManager.GetActiveScene();
				SceneManager.UnloadSceneAsync(scene);
				SceneManager.LoadScene(scene.name);
			}
		}
	}
}