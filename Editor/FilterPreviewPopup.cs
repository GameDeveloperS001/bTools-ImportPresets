using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bTools.ImportPresets
{
    public class FilterPreviewPopup : PopupWindowContent
    {
        const float charWidth = 6.5f;

        float height = 200;
        float width = 200;
        string affectedPaths;
        Vector2 scroll;


        // String, StringCount, LongestString
        public FilterPreviewPopup(float maxHeight, (string, int, int) affectedPathData)
        {
            this.height = Mathf.Min(maxHeight, affectedPathData.Item2 * EditorGUIUtility.singleLineHeight + 20);
            this.width = affectedPathData.Item3 * charWidth;
            this.affectedPaths = affectedPathData.Item1;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect rect)
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scroll))
            {
                scroll = scrollView.scrollPosition;
                EditorGUILayout.LabelField(affectedPaths, EditorStyles.helpBox);
            }
        }
    }
}