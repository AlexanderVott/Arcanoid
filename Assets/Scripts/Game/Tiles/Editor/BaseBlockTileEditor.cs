using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace RedDev.Game.Tiles.Editor
{
	public class BaseBlockTileEditor : UnityEditor.Editor
	{
		public virtual BaseBlockTile tile => target as BaseBlockTile;

		private SerializedProperty _sprite;
		private SerializedProperty _defaultSprite;
		private SerializedProperty _colliderType;
		private SerializedProperty _health;

		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			return PreviewUtil.RenderStaticPreview(tile.defaultSprite, width, height);
		}

		public virtual void OnEnable()
		{
			_health = serializedObject.FindProperty("health");
			_sprite = serializedObject.FindProperty("m_Sprite");
			_defaultSprite = serializedObject.FindProperty("_defaultSprite");
			_colliderType = serializedObject.FindProperty("m_ColliderType");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(_defaultSprite);
			_sprite.objectReferenceValue = _defaultSprite.objectReferenceValue;
			
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_colliderType);

			EditorGUILayout.PropertyField(_health);

			EditorGUILayout.Space();

			int count = EditorGUILayout.DelayedIntField("Number of broken tiles", tile._brokeSprites != null ? tile._brokeSprites.Length : 0);
			if (count < 0)
				count = 0;
			if (tile._brokeSprites == null || tile._brokeSprites.Length != count)
			{
				Array.Resize(ref tile._brokeSprites, count);
			}

			if (count == 0)
				return;

			EditorGUILayout.LabelField("Place broken tiles.");
			EditorGUILayout.Space();

			for (int i = 0; i < count; i++)
			{
				tile._brokeSprites[i] = (Tile)EditorGUILayout.ObjectField("Tile " + (i + 1), tile._brokeSprites[i], typeof(Tile), false, null);
			}
		}

		protected void BeginChangeCheck()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
		}

		protected void EndChangeCheck()
		{
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(tile);
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}