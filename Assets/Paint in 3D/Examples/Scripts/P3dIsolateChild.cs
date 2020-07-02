using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CustomEditor(typeof(P3dIsolateChild))]
	public class P3dIsolateChild_Editor : P3dEditor<P3dIsolateChild>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("When you call the Isolate method on this component, all child GameObjects will be deactivated, and the specified child will be activated.", MessageType.Info);
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This allows you to enable of child Gameobject.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dIsolateChild")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Isolate Child")]
	public class P3dIsolateChild : MonoBehaviour
	{
		public bool Isolated
		{
			get
			{
				var cachedTransform = transform;

				for (var i = cachedTransform.childCount - 1; i >= 0; i--)
				{
					var childGameObject = cachedTransform.GetChild(i).gameObject;

					if (childGameObject.activeSelf == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void Clear()
		{
			var cachedTransform = transform;

			for (var i = cachedTransform.childCount - 1; i >= 0; i--)
			{
				var childGameObject = cachedTransform.GetChild(i).gameObject;

				childGameObject.SetActive(false);
			}
		}

		public void Isolate(GameObject isolate)
		{
			var cachedTransform = transform;

			for (var i = cachedTransform.childCount - 1; i >= 0; i--)
			{
				var childGameObject = cachedTransform.GetChild(i).gameObject;

				childGameObject.SetActive(childGameObject == isolate);
			}
		}
	}
}