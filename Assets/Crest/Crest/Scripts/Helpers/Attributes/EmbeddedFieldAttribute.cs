// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

namespace Crest
{
    using UnityEngine;

#if UNITY_EDITOR
    using Crest.EditorHelpers;
    using UnityEditor;
#endif

    public class EmbeddedFieldAttribute : MultiPropertyAttribute
    {
#if UNITY_EDITOR
        internal EmbeddedAssetEditor editor;
#endif

        public EmbeddedFieldAttribute()
        {
#if UNITY_EDITOR
            editor = new EmbeddedAssetEditor();
#endif
        }

#if UNITY_EDITOR
        internal override void OnGUI(Rect position, SerializedProperty property, GUIContent label, MultiPropertyDrawer drawer, bool isLast)
        {
            EmbeddedFieldAttribute embeddedAttribute = this;
            embeddedAttribute.editor.DrawEditorCombo(drawer, property, "asset");
        }

        // Removes space above embedded editor so the embedded editor replaces this drawer.
        internal override float? GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;
#endif
    }
}
