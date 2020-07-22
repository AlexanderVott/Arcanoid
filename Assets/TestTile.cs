using UnityEngine;
using UnityEngine.Tilemaps;

namespace AVott.DemoProj.Tiles
{
	public class TestTile : Tile
	{
		public Sprite NormalBlock;
		public Sprite BrokenBlock;
		
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
			//if (tileData.)
		}
	}
}