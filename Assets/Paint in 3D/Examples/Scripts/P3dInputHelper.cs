using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace PaintIn3D
{
	public static class P3dInputHelper
	{
		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		public static float ScaleFactor
		{
			get
			{
				var dpi = Screen.dpi;

				if (dpi <= 0)
				{
					dpi = 200.0f;
				}

				return 200.0f / dpi;
			}
		}

		public static bool AnyMouseButtonSet
		{
			get
			{
				for (var i = 0; i < 4; i++)
				{
					if (Input.GetMouseButton(i) == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, 1 << 5);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}

			return tempRaycastResults;
		}
	}
}