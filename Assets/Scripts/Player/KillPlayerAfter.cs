using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerAfter : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.instance.Death();
    }
}
