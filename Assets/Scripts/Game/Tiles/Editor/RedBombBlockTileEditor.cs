using UnityEditor;

namespace RedDev.Game.Tiles.Editor
{
	[CustomEditor(typeof(RedBombBlockTile))]
	public class RedBombBlockTileEditor : BaseBlockTileEditor
	{
		public new RedBombBlockTile tile => target as RedBombBlockTile;
		
		public override void OnEnable()
		{
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			BeginChangeCheck();

			base.OnInspectorGUI();

			EndChangeCheck();
		}
	}
}