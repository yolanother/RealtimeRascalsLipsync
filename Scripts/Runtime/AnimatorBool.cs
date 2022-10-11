using System;
using UnityEngine;

namespace _Project.Scripts
{
    public class AnimatorBool : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _stateName;
        private int _stateHash;

        private void Awake()
        {
            _stateHash = Animator.StringToHash(_stateName); 
        }

        public void SetAnimatorState(bool state)
        {
            _animator.SetBool(_stateHash, state);
        }
    }
}