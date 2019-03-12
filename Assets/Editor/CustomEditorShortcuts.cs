using UnityEngine;
using UnityEditor;

public static class CustomEditorShortcuts
{
	[MenuItem("Edit/Redo %#Z")]
	static void Redo()
	{
		Undo.PerformRedo();
	}

	[MenuItem("Edit/Show|Hide All 2D Colliders &c")]
	static void ShowHideAll2DColliders()
	{
		Physics2D.alwaysShowColliders = !Physics2D.alwaysShowColliders;
	}
}