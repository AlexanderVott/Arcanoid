using UnityEditor;

namespace RedDev.Game.Tiles.Editor
{
	[CustomEditor(typeof(GreenBlockTile))]
	public class GreenBlockTileEditor : BaseBlockTileEditor
	{
		public new GreenBlockTile tile => target as GreenBlockTile;
		
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