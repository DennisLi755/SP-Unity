using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockBoss5Pattern : BulletPattern
{
    [SerializeField]
    private float shootSpeed;

    new void Start() {
        base.Start();
        StartCoroutine(StopAndStartShoot());

        IEnumerator StopAndStartShoot() {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < bullets.Length; i++) {
                bullets[i].Speed = 0f;
            }

            yield return new WaitForSeconds(0.7f);
            for (int i = 0; i < bullets.Length; i++) {
                ShootBullet(i);
            }
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
