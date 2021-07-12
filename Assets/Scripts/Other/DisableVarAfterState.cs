using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVarAfterState : StateMachineBehaviour
{
    [SerializeField] string VarName;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(VarName,false);
    }
}
