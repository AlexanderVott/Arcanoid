using System.Runtime.Remoting.Messaging;
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