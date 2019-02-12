using UnityEngine;
using UnityEditor;

public static class CustomEditorShortcuts
{
    [MenuItem("Edit/Redo %#Z")]
    static void Redo()
    {
        Undo.PerformRedo();
    }
}