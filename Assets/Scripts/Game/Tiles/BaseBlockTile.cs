using RedDev.Game.Managers;
using RedDev.Kernel.Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RedDev.Game.Tiles
{
	public class BaseBlockTile : Tile
	{
		[SerializeField]
		protected Sprite _defaultSprite;
		public Sprite defaultSprite => _defaultSprite;
		
		public Tile[] _brokeSprites;

		public int health = 1;
		
		public virtual void Hit(BlockData block, Vector3Int cell)
		{
		}

		public Tile GetBrokenSprite(Vector3Int position)
		{
			if (_brokeSprites.Length == 0)
				return null;
			var gamefield = Core.Get<GameFieldManager>();
			var cell = gamefield[position];

			int index = ((health - cell.health) * _brokeSprites.Length) / health;
			if (index < _brokeSprites.Length)
				return _brokeSprites[index];
			return null;
		}
	}
}