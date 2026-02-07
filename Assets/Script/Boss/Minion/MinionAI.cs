using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAI : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float changeDirTime = 2f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 4f;
    public float shootCooldown = 2f;

    private Vector2 moveDir;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        if (moveDir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            moveDir = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(changeDirTime);
        }
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootCooldown);

            if (!player) continue;

            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
        }
    }
}
