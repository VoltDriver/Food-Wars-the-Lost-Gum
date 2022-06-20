using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionPhase1AnimationManager : MonoBehaviour
{

    private Animator _animator;

    //More to be added along with integer value:
    // 0 - idle
    // 1 - spawnAttack
    // 2 - prepareLeftSwipe
    // 3 - prepareRightSwipe
    // 4 - swipeAttack -> for both left and right
    // 5 - laugh
    // 6 - prepareCenter
    // 7 - rootAttack
    // 8 - charge
    // 9 - chargeInterrupted

    private string[] _triggerArray = {"idle", "spawnAttack", "prepareLeftSwipe", "prepareRightSwipe", "swipeAttack", "laugh", "prepareCenter", "rootAttack", "charge", "chargeInterrupted" };

    public string[] TriggerArray { get => _triggerArray; }
    //Animator Variables, to be changed by parent from states

    // Start is called before the first frame update
    void Start()
    {
        _animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTrigger(int t)
    {
        if (t < 0 || t > _triggerArray.Length - 1)
            throw new System.Exception("Onion Boss animator trigger request out of range");
        else
        {
            Debug.Log("Animator settting "+ _triggerArray[t]);
            _animator.SetTrigger(_triggerArray[t]);
            ResetOtherAnimationTriggers(t);
        }


    }
    private void ResetOtherAnimationTriggers(int t)
    {
        for (int i =0; i< _triggerArray.Length; i++)
        {
            if (i == t)
            {
                continue;
            }
            _animator.ResetTrigger(_triggerArray[i]);
        }
    }

}
