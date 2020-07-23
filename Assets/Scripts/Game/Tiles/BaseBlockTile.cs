using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	public class BaseBlockTile : Tile
	{
		[SerializeField]
		protected Sprite _defaultSprite;
		public Sprite defaultSprite => _defaultSprite;
		
		public Sprite[] _brokeSprites;

		public int health = 1;
		
		public virtual void Hit(BlockData block, Vector3Int cell)
		{
		}
	}
}