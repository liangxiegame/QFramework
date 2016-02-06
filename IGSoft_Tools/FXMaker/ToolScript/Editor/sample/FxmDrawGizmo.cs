using UnityEditor;
using UnityEngine;
/*
class GizmoTest
{
	/// The RenderLightGizmo function will be called if the light is not selected.
	/// The gizmo is drawn when picking.
	[DrawGizmo(GizmoType.Active | GizmoType.SelectedOrChild)]
	static void RenderLightGizmo(Light light, GizmoType gizmoType)
	{
		Debug.Log("RenderLightGizmo");

		Vector3 position = light.transform.position;
		// Draw the light icon
		// (A bit above the one drawn by the builtin light gizmo renderer)
		Gizmos.DrawIcon(position + Vector3.up, "Light Gizmo.tiff");

		// Are we selected? Draw a solid sphere surrounding the light
		if ((gizmoType & GizmoType.SelectedOrChild) != 0)
		{
			// Indicate that this is the active object by using a brighter color.
			if ((gizmoType & GizmoType.Active) != 0)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.red * 0.5F;
			Gizmos.DrawSphere(position, light.range);
		}
	}
}
*/
/*
// Draw the gizmo if it is selected or a child of the selection.
// This is the most common way to render a gizmo
[DrawGizmo (GizmoType.SelectedOrChild)]

// Draw the gizmo only if it is the active object.
[DrawGizmo (GizmoType.Active)]
*/
