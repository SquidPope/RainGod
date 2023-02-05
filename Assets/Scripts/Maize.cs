using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthState {WitheredSprout, BurningSprout, Sprout, WitheredCorn, BurningCorn, Corn}
public class Maize : MonoBehaviour
{
    // Script to control the maize players can grow by watering

    //current growth level
    //timer?
    [SerializeField] Sprite[] growthSprites;
    [SerializeField] new SpriteRenderer renderer;
    GrowthState growth = GrowthState.Sprout;

    bool isWet = false;

    float timer = 0f;
    float burnTimerMax = 5f;
    float growthTimerMax = 7f;

    //On collision-
    //ENTER
    //enemy reduces growth level
    //rain attack increases it

    /*
    growth starts at 0
    hit by rain attack - wet
    timer
    end of timer, no longer wet but growth is 1
    repeat

    hit by enemy- cancel wet
    if not wet, burn
    if hit by rain, stop burning
    burn timer
    if burn timer ends, decrease growth
    growth can go negative?
    
    */
    void Start()
    {
        Growth = GrowthState.Sprout;
    }
    
    GrowthState Growth
    {
        get { return growth; }
        set
        {
            growth = value;
            //set art based on growth state
            renderer.sprite = growthSprites[(int)growth];
        }
    }

    bool IsBurning() { return Growth == GrowthState.BurningCorn || Growth == GrowthState.BurningSprout; }
    bool IsWithered() { return Growth == GrowthState.WitheredCorn || Growth == GrowthState.WitheredSprout; }

    void OnTriggerEnter2D(Collider2D other)
    {
        //find out if attack is rain type?
        if (other.tag == "Enemy")
        {
            //start burning
            //if already burning, decrease growth immediately
            if (IsBurning())
            {
                if (Growth == GrowthState.BurningSprout)
                    Growth = GrowthState.WitheredSprout;
                else if (Growth == GrowthState.BurningCorn)
                    Growth = GrowthState.WitheredCorn;
            }
            else if (!IsWithered())
            {
                if (Growth == GrowthState.Sprout)
                    Growth = GrowthState.BurningSprout;
                else if (Growth == GrowthState.Corn)
                    Growth = GrowthState.BurningCorn;
            }
            else if (Growth == GrowthState.WitheredCorn) //Withered sprout is technically the worst state possible
            {
                Growth = GrowthState.Sprout;
            }
        }
    }

    public void AttackHit(AttackType type)
    {
        if (type == AttackType.Rain)
        {
            if (IsBurning())
            {
                if (Growth == GrowthState.BurningSprout)
                    Growth = GrowthState.Sprout;
                else if (Growth == GrowthState.BurningCorn)
                    Growth = GrowthState.Corn;
            }
            else
            {
                isWet = true;
            }
        }
    }

    void Update()
    {
        if (IsBurning())
        {
            timer += Time.deltaTime;
            if (timer >= burnTimerMax)
            {
                if (Growth == GrowthState.BurningSprout)
                    Growth = GrowthState.WitheredSprout;
                else if (Growth == GrowthState.BurningCorn)
                    Growth = GrowthState.WitheredCorn;

                timer = 0f;
            }
        }
        else if (isWet && Growth != GrowthState.Corn)
        {
            timer += Time.deltaTime;

            if (timer >= growthTimerMax)
            {
                if (IsWithered())
                {
                    if (Growth == GrowthState.WitheredSprout)
                        Growth = GrowthState.Sprout;
                    else if (Growth == GrowthState.WitheredCorn)
                        Growth = GrowthState.Corn;

                    timer = 0f;
                }
            }
        }
    }

    //STAY
    //Player regains small amount of hp if growth level is max?
}
