using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AreAlliesNearbyNode : ActionNode
{
    public float m_range = 10f;
    public int m_minimumRequired = 1;
    public bool m_mushroomCountsAsHavingAllies = false;

    private AIAgent m_agent;


    protected override void OnStart()
    {
        // Possible that this sometimes react unfortunately to a change in the owner object.
        // If that ends up being the case, we need to make it so that the BehaviourTree owner change calls a "OwnerChange()" method on all children.
        m_agent = Owner.GetComponent<AIAgent>();

        if (m_agent == null)
        {
            throw new System.Exception(this.GetType().Name + ": No AIAgent associated with this node's owner.");
        }
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        var colliders = Physics2D.OverlapCircleAll(Owner.transform.position, m_range, LayerMask.GetMask("Characters"));

        int count = 0;
        bool mushroomFound = false;

        foreach (Collider2D collider in colliders)
        {
            // Can't count ourself.
            if (collider.gameObject == Owner)
                continue;

            Pea otherPea = collider.gameObject.GetComponent<Pea>();

            if(otherPea != null)
            {
                // This collider is a Pea.
                count++;
            }

            if(m_mushroomCountsAsHavingAllies)
            {
                Mushroom mush = collider.gameObject.GetComponent<Mushroom>();

                if(mush != null)
                {
                    mushroomFound = true;
                }
            }
        }

        // If we count mushrooms as the ultimate ally, and we found one, it's automatically a success.
        if(m_mushroomCountsAsHavingAllies)
        {
            if(mushroomFound)
            {
                return State.Success;
            }
        }

        // If enough allies are nearby...
        if(count >= m_minimumRequired)
        {
            // ... this is a success.
            return State.Success;
        }
        else
        {
            return State.Failure;
        }
    }
}
