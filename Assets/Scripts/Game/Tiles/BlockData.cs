using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	public class BlockData
	{
		public Vector3Int localPlace { get; set; }
		public BaseBlockTile baseTile { get; set; }
		public Tilemap map { get; set; }
		public int health { get; set; }

		public BlockData(Tilemap owner, BaseBlockTile tile, Vector3Int place)
		{
			baseTile = tile;
			health = tile.health;
			localPlace = place;
			map = owner;
		}
	}
}