using RedDev.Game.Managers;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	[CreateAssetMenu(menuName = "Tiles/GreenBlockTile")]
	public class GreenBlockTile : BaseBlockTile
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
		}
		private void Destroy(Vector3Int place)
		{
			Core.Get<GameFieldManager>().DestroyTile(place);
		}

		public override void RefreshTile(Vector3Int position, ITilemap tilemap)
		{
			base.RefreshTile(position, tilemap);
		}

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
		}
	}
}