using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // Reference
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = pm.moveDir.x != 0 || pm.moveDir.y != 0;

        am.SetBool("Move Right", false);
        am.SetBool("Move Left", false);
        am.SetBool("Move Up", false);
        am.SetBool("Move Down", false);

        if (isMoving)
        {
            if (Mathf.Abs(pm.moveDir.x) > Mathf.Abs(pm.moveDir.y))
            {
                if (pm.moveDir.x > 0)
                {
                    am.SetBool("Move Right", true);
                    sr.flipX = false;
                }
                else
                {
                    am.SetBool("Move Left", true);
                    sr.flipX = true;
                }
            }
            else
            {
                if (pm.moveDir.y > 0)
                {
                    am.SetBool("Move Up", true);
                }
                else
                {
                    am.SetBool("Move Down", true);
                }
            }
        }
    }
}
