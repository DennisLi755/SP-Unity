using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTutorialPattern : BulletPattern
{
    [SerializeField]
    private float staggerTime;
    [SerializeField]
    private float shootSpeed;

    new void Start() {
        base.Start();
        for (int i = 0; i < bullets.Length; i++) {
            bullets[i].Speed = 2/Mathf.Cos((bullets[i].Angle - 90) * Mathf.Deg2Rad);
        }
        StartCoroutine(StopAndStartShoot());

        IEnumerator StopAndStartShoot() {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < bullets.Length; i++) {
                bullets[i].Speed = 0f;
            }

            yield return new WaitForSeconds(1f);
            ShootBullet(0);
            ShootBullet(1);

            yield return new WaitForSeconds(staggerTime);
            ShootBullet(2);
            ShootBullet(3);

            yield return new WaitForSeconds(staggerTime);
            ShootBullet(4);
        }
    }

    new void Update() {
        base.Update();
    }

    private void ShootBullet(int index) {
        float angle = Mathf.Atan2(player.transform.position.y - bullets[index].gameObject.transform.position.y, 
            player.transform.position.x  - bullets[index].gameObject.transform.position.x);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        bullets[index].Speed = shootSpeed;
        bullets[index].Direction = direction;   
    }
}
