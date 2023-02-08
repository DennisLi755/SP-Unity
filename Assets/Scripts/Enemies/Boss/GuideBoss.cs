using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GuideBoss : Boss
{
    private Bullet[] bullets = new Bullet[5];
    [SerializeField]
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void ShootPatternBullet(GameObject pattern)
    {
        base.ShootPatternBullet(pattern);
        for (int i = 0; i < pat.transform.childCount; i++) {
            bullets[i] = pat.transform.GetChild(i).gameObject.GetComponent<Bullet>();
        }
        StartCoroutine(StopAndStartShoot());

        IEnumerator StopAndStartShoot() {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < pattern.transform.childCount; i++) {
                bullets[i].SetSpeed(0f);
            }
            yield return new WaitForSeconds(1f);
            ShootBullet(0);
            ShootBullet(1);

            yield return new WaitForSeconds(0.5f);
            ShootBullet(2);
            ShootBullet(3);

            yield return new WaitForSeconds(0.5f);
            ShootBullet(4);
        }
    }

    public void ShootBullet(int index) {
        float angle = Mathf.Atan2(player.transform.position.y - bullets[index].gameObject.transform.position.y, player.transform.position.x  - bullets[index].gameObject.transform.position.x);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        bullets[index].SetSpeed(3f);
        bullets[index].SetDirection(direction);   
    }
}
