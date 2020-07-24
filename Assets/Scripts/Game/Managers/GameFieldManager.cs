using System.Collections.Generic;
using RedDev.Game.Tiles;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Managers
{
	public class GameFieldManager : BaseManager
	{
		public static readonly string GAMEFIELD_TAG = "Gamefield";
		public static readonly string BREAKEFIELD_TAG = "Breakefield";

		private GridInformation _info;

		private Dictionary<Vector3Int, BlockData> _typesTiles = new Dictionary<Vector3Int, BlockData>();
		private List<Vector3Int> _deletedCells = new List<Vector3Int>();

		public BlockData this[Vector3Int cell] => _typesTiles.ContainsKey(cell) ? _typesTiles[cell] : null;

		public Tilemap tilemap { get; private set; }
		public Tilemap breakemap { get; private set; }

		public int counterDestroyedBlocks { get; private set; } = 0;

		public override void Attach()
		{
			base.Attach();
		}

		public void RegisterFields(Tilemap tilemap, Tilemap breakemap)
		{
			this.tilemap = tilemap;
			this.breakemap = breakemap;
		}

		public void Clear()
		{
			_typesTiles.Clear();
			_deletedCells.Clear();
			counterDestroyedBlocks = 0;
		}

		public void Add(BaseBlockTile tile, Vector3Int place)
		{
			if (!_typesTiles.ContainsKey(place))
				_typesTiles.Add(place, new BlockData(tilemap, breakemap, tile, place));
		}

		public void Remove(Vector3Int place)
		{
			_typesTiles.Remove(place);
		}

		public void DestroyTile(Vector3Int place)
		{
			if (!_deletedCells.Contains(place))
			{
				counterDestroyedBlocks++;
				_deletedCells.Add(place);
			}
		}
		
		void LateUpdate()
		{
			var list = new List<Vector3Int>(_deletedCells);
			_deletedCells.Clear();
			foreach (var place in list)
			{
				var tile = this[place];
				if (tile != null)
				{
					tilemap.SetTile(place, null);
					breakemap.SetTile(place, null);
				}
				Remove(place);
			}
			_deletedCells.Clear();
		}
	}
}