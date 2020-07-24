using RedDev.Game.Managers;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	[CreateAssetMenu(menuName = "Tiles/RedBombBlockTile")]
	public class RedBombBlockTile : BaseBlockTile
	{
		public override void Hit(BlockData block, Vector3Int cell)
		{
			if (block.health <= 0)
			{
				Destroy(cell);
				return;
			}
			if (--block.health == 0)
				Destroy(cell);
			else 
				block.map.RefreshTile(cell);

			var gamefield = Core.Get<GameFieldManager>();

			Vector3Int offsetCell;
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					offsetCell = cell + new Vector3Int(x, y, 0);
					var otherBlock = gamefield[offsetCell];
					if (otherBlock == null)
						continue;
					var tile = otherBlock.map.GetTile<BaseBlockTile>(offsetCell);
					if (tile != null)
						Destroy(offsetCell);
				}
			}
		}

		private void Destroy(Vector3Int place)
		{
			Core.Get<GameFieldManager>().DestroyTile(place);
		}

		public override void RefreshTile(Vector3Int position, ITilemap tilemap)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				base.RefreshTile(position, tilemap); 
				return;
			}
#endif
			var gamefield = Core.Get<GameFieldManager>();
			Vector3Int cell;
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					cell = position + new Vector3Int(x, y, 0);
					var block = gamefield[cell];
					if (block == null)
						continue;
					var tile = tilemap.GetTile<BaseBlockTile>(cell);
					if (tile != null)
						tile.Hit(block, cell);
					base.RefreshTile(cell, tilemap);
				}
			}
		}

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (_brokeSprites.Length == 0)
				return;
			var gamefield = Core.Get<GameFieldManager>();
			var cell = gamefield[position];

			int index = ((health - cell.health) * _brokeSprites.Length) / health;
			if (index < _brokeSprites.Length)
				tileData.sprite = _brokeSprites[index];
			else
				Debug.Log(index);
		}
	}
}