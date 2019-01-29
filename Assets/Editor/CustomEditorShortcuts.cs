using UnityEngine;
using UnityEditor;

public static class CustomEditorShortcuts
{
    [MenuItem("Shortcuts/Redo %#Z")]
    static void Redo()
    {
        Undo.PerformRedo();
    }
}