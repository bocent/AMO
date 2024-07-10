using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
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
        animator.SetFloat(conditionName, value, 0.2f, Time.deltaTime);
    }

    public void SetAnimationCondition(string conditionName, int value)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.SetInteger(conditionName, value);
    }

    public float SetAnimationCondition(string conditionName)
    {
        if (gameObject.activeInHierarchy)
        {
            if (!animator) animator = GetComponent<Animator>();
            animator.SetTrigger(conditionName);
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        return 0f;
    }

    public void SetIdleAnimation(string conditionName, float time)
    {
        if (gameObject.activeInHierarchy)
        {
            if (!animator) animator = GetComponent<Animator>();
            animator.Play("Idle", 0, time);
            animator.SetTrigger(conditionName);
            Debug.LogWarning("set acc idle animation " + DateTime.Now.Ticks);
        }
    }
}
