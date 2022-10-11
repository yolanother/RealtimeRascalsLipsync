using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTalkingState : StateMachineBehaviour
{
    [SerializeField] private string parameterName;
    [SerializeField] private int minValue = 0;
    [SerializeField] private int maxValue = 1;
    private int _paramHash;

    private void OnEnable()
    {
        _paramHash = Animator.StringToHash(parameterName);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(_paramHash, Random.Range(minValue, maxValue));
    }
}