using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimationCondition(string conditionName, bool value)
    {
        if(!animator) animator = GetComponent<Animator>();
        animator.SetBool(conditionName, value);
    }

    public void SetAnimationCondition(string conditionName, float value)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetFloat(conditionName, value);
    }

    public void SetAnimationCondition(string conditionName, int value)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetInteger(conditionName, value);
    }

    public void SetAnimationCondition(string conditionName)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetTrigger(conditionName);
    }
}
