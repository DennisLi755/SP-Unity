using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBullet : Bullet
{    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        GameObject player = PlayerInfo.Instance.gameObject;
        float angle = Mathf.Atan2(player.transform.position.y - gameObject.transform.position.y, 
            player.transform.position.x  - gameObject.transform.position.x);
        Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
