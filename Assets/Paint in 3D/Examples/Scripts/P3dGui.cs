using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dGui))]
	public class P3D_Gui_Editor : P3dEditor<P3dGui>
	{
		protected override void OnInspector()
		{
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component is used in all example scenes to drive the GUI buttons.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dGui")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "GUI")]
	public class P3dGui : MonoBehaviour
	{
		public GameObject[] Options;

		public void ClickOption(int index)
		{
			if (Options != null)
			{
				for (var i = Options.Length - 1; i >= 0; i--)
				{
					Options[i].SetActive(i == index);
				}
			}
		}

		public void ClickReload()
		{
			LoadLevel(GetCurrentLevel());
		}

		public void ClickPrev()
		{
			var index = GetCurrentLevel() - 1;

				if (index < 0)
				{
					index = GetLevelCount() - 1;
				}

				LoadLevel(index);
		}

		public void ClickNext()
		{
			var index = GetCurrentLevel() + 1;

			if (index >= GetLevelCount())
			{
				index = 0;
			}

			LoadLevel(index);
		}

	#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		private static int GetCurrentLevel()
		{
			return Application.loadedLevel;
		}

		private static int GetLevelCount()
		{
			return Application.levelCount;
		}

		private static void LoadLevel(int index)
		{
			Application.LoadLevel(index);
		}
	#else
		private static int GetCurrentLevel()
		{
			return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		}

		private static int GetLevelCount()
		{
			return UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}

		private static void LoadLevel(int index)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
		}
	#endif
	}
}