using RedDev.Game.Managers;
using RedDev.Game.Tiles;
using RedDev.Kernel.Actors;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game
{
	public class TilesDestroyerComponent : ComponentActor
	{
		private Tilemap tilemap;
		[SerializeField]
		private string tagTileMap = GameFieldManager.GAMEFIELD_TAG;
		
		private GameFieldManager gamefield;

		public override void OnStart()
		{
			base.OnStart();
			var go = GameObject.FindGameObjectWithTag(tagTileMap);
			if (go == null)
			{
				Debug.LogError($"Not found tilemap with tag {tagTileMap} for {gameObject.name}");
				return;
			}
			tilemap = go.GetComponent<Tilemap>();

			gamefield = Core.Get<GameFieldManager>();
		}

		void OnCollisionEnter2D(Collision2D collision)
		{
			Vector3 hitPos = Vector3.zero;
			if (tilemap == null || tilemap.gameObject != collision.gameObject) 
				return;

			Vector3Int lastCell = default;
			foreach (var hit in collision.contacts)
			{
				hitPos.x = hit.point.x - 0.01f * hit.normal.x;
				hitPos.y = hit.point.y - 0.01f * hit.normal.y;
				var cell = tilemap.WorldToCell(hitPos);
				if (lastCell == cell)
					continue;
				lastCell = cell;
				var tile = tilemap.GetTile<BaseBlockTile>(cell);
				var block = gamefield[cell];
				if (tile == null || block == null) 
					continue;
				tile.Hit(block, cell);
			}
		}
	}
}