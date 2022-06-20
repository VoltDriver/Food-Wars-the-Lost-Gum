using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor m_editor;

    public InspectorView()
    {

    }

    /// <summary>
    /// Updates the inspector view panel with the information about the currently selected NodeView.
    /// </summary>
    /// <param name="pNodeView">The currently selected NodeView</param>
    public void UpdateSelection(NodeView pNodeView)
    {
        // Clear any previous selection we had.
        Clear();

        // Destroying the previous editor before creating a new one.
        UnityEngine.Object.DestroyImmediate(m_editor);

        m_editor = Editor.CreateEditor(pNodeView.m_node);
        // Creating a house for all of what we created.
        // Because this is a visual element, it needs a "wrapper", which is that weird code passed to the constructor.
        IMGUIContainer container = new IMGUIContainer(() => {
            m_editor.OnInspectorGUI();
        });

        // Adding the container as a child of this visual element.
        Add(container);
    }
}
