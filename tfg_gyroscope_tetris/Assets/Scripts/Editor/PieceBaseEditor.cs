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

        if(piece.pieceBlocks != null)
        {
            for(int i = 0; i < piece.transform.childCount; i++)
            {
                SpriteRenderer block;
                if(piece.transform.GetChild(i).TryGetComponent(out block))
                {
                    if(block.sprite != piece.blockSprite)
                    {
                        EditorGUILayout.HelpBox("One or more of the blocks sprite does not match the pice block sprite", MessageType.Warning);
                        break;
                    }
                }
            }
        }

        if (piece.menuPreviewSprite == null)
        {
            EditorGUILayout.HelpBox("The piece is missing the sprite for the menu preview", MessageType.Warning);
        }

        base.OnInspectorGUI();
    }
}
