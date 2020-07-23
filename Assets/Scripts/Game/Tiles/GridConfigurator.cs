using RedDev.Game.Managers;
using RedDev.Kernel.Actors;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	public class GridConfigurator : Actor
	{
		protected override void OnStart()
		{
			base.OnStart();
			var gameField = Core.Get<GameFieldManager>();

			var tilemapObj = GameObject.FindGameObjectWithTag(GameFieldManager.GAMEFIELD_TAG);
			if (tilemapObj == null)
			{
				Debug.LogError($"Not found tilemap with tag {GameFieldManager.GAMEFIELD_TAG} for {gameObject.name}");
				return;
			}
			var tilemap = tilemapObj.GetComponent<Tilemap>();

			foreach (Vector3Int cell in tilemap.cellBounds.allPositionsWithin)
			{
				var tile = tilemap.GetTile<BaseBlockTile>(cell);
				if (tile == null)
					continue;
				gameField.Add(tilemap, tile, cell);
			}
		}
	}
}