using RedDev.Game.Managers;
using RedDev.Kernel.Managers;
using UnityEngine;

namespace RedDev.Game.Tiles
{
	[CreateAssetMenu(menuName = "Tiles/GreenBlockTile")]
	public class GreenBlockTile : BaseBlockTile
	{
		public override void Hit(BlockData block, Vector3Int cell)
		{
			if (--block.health <= 0)
				Destroy(cell);
			else
			{
				block.breakeMap.SetTile(cell, GetBrokenSprite(cell));
				block.map.RefreshTile(cell);
			}
		}

		private void Destroy(Vector3Int place)
		{
			Core.Get<GameFieldManager>().DestroyTile(place);
		}
	}
}