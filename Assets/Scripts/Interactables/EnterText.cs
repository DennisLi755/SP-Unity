using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterText : TextboxInteraction
{
    // Update is called once per frame
    new void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(trigger.bounds.center, trigger.bounds.size, 0, Vector2.zero, 0, playerLayer);
        if (hit && !isActive) {
            isActive = true;
            //display prompt? depends on if we do it
            OnInteract();
        } else if (!hit && isActive) {
            isActive = false;
        }
    }
}
