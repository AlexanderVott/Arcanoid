using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	public class BlockData
	{
		public Vector3Int localPlace { get; set; }
		public BaseBlockTile baseTile { get; set; }
		public int health { get; set; }

		public Tilemap map { get; private set; }
		public Tilemap breakeMap { get; private set; }

		public BlockData(Tilemap map, Tilemap breakeMap, BaseBlockTile tile, Vector3Int place)
		{
			this.map = map;
			this.breakeMap = breakeMap;
			baseTile = tile;
			health = tile.health;
			localPlace = place;
		}
	}
}