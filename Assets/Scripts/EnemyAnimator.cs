using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    // Script controling enemy sprites
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] RuntimeAnimatorController up;
    [SerializeField] RuntimeAnimatorController down;
    [SerializeField] RuntimeAnimatorController side;

    public void UpdateSprite (Vector2 move)
    {
        if (move.y > 0 && move.y > move.x) //If we're moving more up than left
        {
            animator.runtimeAnimatorController = up; //ToDo: Change this? Apparently swapping animators like this without a transition can cause that long Unity error
        }
        else if (move.y < 0 && move.y < move.x) //If we're moving more down than right
        {
            animator.runtimeAnimatorController = down;
        }
        else
        {
            animator.runtimeAnimatorController = side;
            spriteRenderer.flipX = move.x < 0;
        }
    }
}
