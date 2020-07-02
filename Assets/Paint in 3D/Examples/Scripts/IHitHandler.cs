using UnityEngine;

namespace PaintIn3D
{
	/// <summary>This allows components like P3dDragRaycast to call components like P3dPaintDecal.</summary>
	public interface IHitHandler
	{
		void HandleHit(Vector3 position, Vector3 normal, bool preview, float pressure);
	}
}