using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVarBeforeState : StateMachineBehaviour
{
    [SerializeField] string VarName;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(VarName, false);
    }
}
