using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    /// <summary>
    /// Behavior tree this runner will execute.
    /// </summary>
    public BehaviourTree m_tree;

    private void Awake()
    {
        // Used to make it so that if we have multiple game objects, each having a 
        // BehaviourTreeRunner, and they all target the same tree, then they don't overlap
        // each other. They all have their own version of the tree.
        m_tree = m_tree.Clone();

        // Setting up the owner
        m_tree.Owner = this.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Failsafe for uninitialized tree
        if(m_tree.Owner == null)
        {
            // Setting up the owner
            m_tree.Owner = this.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Failsafe for uninitialized tree
        if (m_tree.Owner == null)
        {
            // Setting up the owner
            m_tree.Owner = this.gameObject;
        }

        m_tree.Update();

        // Loops the tree.
        m_tree.RestartTree();
    }
}
