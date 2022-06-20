using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BehaviorTreeEditor : EditorWindow
{
    [MenuItem("Window/BehaviorTreeEditor")]
    public static void OpenEditorWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    [MenuItem("BehaviorTree/BehaviorTreeEditor")]
    public static void OpenEditorWindowFromTreeMenu()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    public BehaviourTreeView m_treeView;
    public InspectorView m_inspectorView;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/BehaviourTrees/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root); // Prevents an intermediate object from being created when you clone the tree.

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UI/BehaviourTrees/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        // Queries an object for a specific element type.
        m_treeView = root.Q<BehaviourTreeView>();
        m_inspectorView = root.Q<InspectorView>();

        // Setting the callback for the tree as a whole to be linked to this editor's function.
        m_treeView.OnNodeSelected = OnNodeSelectionChange;

        // Manually calling OnSelectionChange to make sure that after we compile, the tree is loaded in properly.
        OnSelectionChange();
    }

    /// <summary>
    /// Callback for when we select a BehaviorTree Object inside the Project File View.
    /// </summary>
    private void OnSelectionChange()
    {
        // Verifies that the selected object is indeed a tree.
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (tree
            //             Check that prevents a bug from happening, when you create a new tree and it gets automatically selected but
            //              is not quite ready to be displayed. However, this check is ONLY available starting from Unity 2021.
            //            &&
            //            AssetDatabase.CanOpenAssetInEditor(m_tree.GetInstanceID())
            )
        {
            // Initalize the view of that tree inside our GUI.
            m_treeView.PopulateView(tree);
        }
    }

    /// <summary>
    /// Callback for when we select a new node.
    /// </summary>
    /// <param name="pNode">NodeView that was selected.</param>
    void OnNodeSelectionChange(NodeView pNode)
    {
        // We update the inspector view panel with information about the newly selected node.
        m_inspectorView.UpdateSelection(pNode);
    }
}