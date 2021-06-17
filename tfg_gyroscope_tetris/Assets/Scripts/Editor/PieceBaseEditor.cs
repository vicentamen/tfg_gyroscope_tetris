using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PieceBase)), CanEditMultipleObjects]
public class PieceBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PieceBase piece = (PieceBase)target;

        if (piece.pieceBlocks == null || piece.pieceBlocks.Length < piece.transform.childCount)
            EditorGUILayout.HelpBox("The blocks list is empty or does not match the number of blocks inside the object", MessageType.Error);


        if (piece.menuPreviewSprite == null)
            EditorGUILayout.HelpBox("The piece is missing the image for the menu preview", MessageType.Warning);

        base.OnInspectorGUI();
    }
}
