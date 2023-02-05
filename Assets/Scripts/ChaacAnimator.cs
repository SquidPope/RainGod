using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaacAnimator : MonoBehaviour
{
    // Script controling sprite for character
    [SerializeField] Sprite down;
    [SerializeField] Sprite left;
    [SerializeField] Sprite right;
    [SerializeField] Sprite up;

    //ToDo: Special Chaac mode using the angry sprites

    public SpriteRenderer spriteRenderer;

    public void UpdateSprite(Vector2 move)
    {
        //ToDo: dead zone?
        if (move.y > 0)
        {
            //up
            spriteRenderer.sprite = up;
        }
        else if (move.y < 0)
        {
            //down
            spriteRenderer.sprite = down;
        }
        else if (move.x > 0)
        {
            spriteRenderer.sprite = right;
        }
        else if (move.x < 0)
        {
            spriteRenderer.sprite = left; //ToDo: Invert scale of left if we don't have left
        }
        else
        {
            //default to down?\
            Debug.Log("movement was 0 _ 0");
        }
    }
}
