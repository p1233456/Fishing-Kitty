using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Rode : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform rodeTransform;

    bool stunMove = false;
    public void Move(Vector3 fishPosition)
    {
        if(stunMove)
            return;
        Vector2 dir = new Vector2(fishPosition.x - rodeTransform.position.x,
            fishPosition.z - rodeTransform.position.z);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rodeTransform.rotation = Quaternion.Euler(36, 0, angle - 90);
    }

    public void StunRight()
    {
        animator.SetLayerWeight(1,1f);
        animator.SetTrigger("Stun");
    }

    public void StunLeft()
    {
        animator.SetLayerWeight(1,0f);
        animator.SetTrigger("Stun");
    }

    public void StartStunMove()
    {
        stunMove = true;
    }

    public void EndStunMove()
    {
        stunMove = false;
    }
}
