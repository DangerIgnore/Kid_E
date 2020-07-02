using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dFollow))]
	public class P3dFollow_Editor : P3dEditor<P3dFollow>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				DrawDefault("target");
			EndError();
			DrawDefault("offset");
			DrawDefault("tilt");
			DrawDefault("dampening");
		}
	}
}
#endif

namespace PaintIn3D
{
	/// <summary>This component makes the current gameObject follow the specified camera..</summary>
	[ExecuteInEditMode]
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dFollow")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Follow")]
	public class P3dFollow : MonoBehaviour
	{
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		public Vector3 Offset { set { offset = value; } get { return offset; } } [SerializeField] private Vector3 offset;
		
		public Vector3 Tilt { set { tilt = value; } get { return tilt; } } [SerializeField] private Vector3 tilt;
		
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = 10.0f;

		protected virtual void LateUpdate()
		{
			if (target != null)
			{
				var position = target.TransformPoint(offset);
				var rotation = target.rotation * Quaternion.Euler(tilt);
				var t        = P3dHelper.DampenFactor(dampening, Time.deltaTime);

				transform.position = Vector3.Lerp(transform.position, position, t);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, t);
			}
		}
	}
}