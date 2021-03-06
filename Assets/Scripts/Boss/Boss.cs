﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossAttack {
    Fireball, Meteor, Shield, Slow
}

public class Boss : MonoBehaviour
{
    public Animator animator;
    public Transform tf;
    public Rigidbody2D rb;

    public Transform player;
    public float health = 700f;
    public float maxHealth = 700f;

    public GameObject shield;

    public GameObject meteorPrefab;
    public GameObject fireballPrefab;
    public GameObject redBulletPrefab;
    public GameObject potionPrefab;

    float moveSpeed = 1.25f;

    float nextAttackTime = 2f;

    float shieldDuration = 5f;
    float currentShieldDuration = 0f;

    float playerCollisionTime = 0f;
    float playerCollisionInterval = 1.5f;

    float fireballForce = 8f * 0.0001f;

    float fireBulletInterval = 1f;
    float fireBulletTime = 1f;

    bool potionSpawned = false;

    Vector3[] meteorPos = {
        new Vector3(-40.4f, 40f, 0f),
        new Vector3(-45.8f, 42.7f, 0f),
        new Vector3(-45.7f, 47.7f, 0f),
        new Vector3(-39.1f, 49.1f, 0f),
        new Vector3(-33.0f, 51.5f, 0f),
        new Vector3(-29.1f, 48.4f, 0f),
        new Vector3(-33.5f, 44.7f, 0f),
        new Vector3(-30.2f, 40.6f, 0f),
        new Vector3(-35.9f, 39.6f, 0f),
        new Vector3(-38.8f, 42.8f, 0f)
    };


    // Unity Callback

    void Update()
    {
        if (health > 0)
        {
            nextAttackTime -= Time.deltaTime;
            currentShieldDuration -= Time.deltaTime;
            playerCollisionTime += Time.deltaTime;
            fireBulletTime += Time.deltaTime;

            if (nextAttackTime < 0f) {
                nextAttackTime = Random.Range(3f, 5f);
                AttackRandom();
            }

            if (health / maxHealth < 0.5 && fireBulletTime >= fireBulletInterval) {
                AttackBullet();
                fireBulletTime = 0f;
            }

            if (health / maxHealth < 0.5 && !potionSpawned) {
                potionSpawned = true;
                GameObject.Instantiate(potionPrefab, meteorPos[0], Quaternion.identity);
                GameObject.Instantiate(potionPrefab, meteorPos[4], Quaternion.identity);
                GameObject.Instantiate(potionPrefab, meteorPos[6], Quaternion.identity);
            }

            shield.SetActive(currentShieldDuration >= 0f);

            SetZPosition();
        }
        else
        {
            shield.SetActive(false);
            animator.SetTrigger("Death");
        }
    }

    void FixedUpdate()
    {
        if (health > 0) 
        {
            Vector3 dir = (player.position - tf.position).normalized;
            float speed = moveSpeed * ((health / maxHealth) > 0.5 ? 1f : 1.7f);
            rb.MovePosition(rb.position + new Vector2(dir.x, dir.y) * Time.fixedDeltaTime * speed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.name == "Fireball(Clone)")
        {
            TakeDamage(GameManager.instance.fireballPower);
        }
        else if (collision.collider.gameObject.name == "Sword(Clone)")
        {
            TakeDamage(GameManager.instance.swordPower);
        }
        else if (collision.collider.gameObject.name == "Player")
        {
            GameManager.instance.ReduceHealth(10);
            playerCollisionTime = 0f;
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.gameObject.name == "Player" && playerCollisionTime > playerCollisionInterval)
        {
            GameManager.instance.ReduceHealth(10);
            playerCollisionTime = 0f;
        }
    }


    // Z Position

    void SetZPosition()
    {
        Vector3 pos = transform.position;
        pos.z = (transform.position.y - 1 > player.position.y) ? 1 : -1;
        transform.position = pos;
    }


    // Attack

    void AttackRandom()
    {
        BossAttack attack = (BossAttack)Random.Range(0, 3);
        switch (attack)
        {
            case BossAttack.Fireball:
                AttackFireball();
                break;
            case BossAttack.Meteor:
                AttackMeteor();
                break;
            case BossAttack.Shield:
                OpenShield();
                AttackRandom();
                break;
            case BossAttack.Slow:
                AttackSlow();
                break;
            default:
                break;
        }
    }

    void AttackFireball()
    {
        animator.SetTrigger("Fire");
        Vector3 pos = GetComponent<Transform>().position;
        Vector3 attackPos = new Vector3(pos.x, pos.y - 1, pos.z);
        for (float x = -1f; x <= 1f; x += 1f)
        {
            for (float y = -1f; y <= 1f; y += 1f)
            {
                if (x == 0 && y == 0) continue;
                Vector2 lookDir = new Vector2(x, y);
                Vector2 attackPoint = new Vector2(attackPos.x + lookDir.x * 3, attackPos.y + lookDir.y * 3);
                GameObject fireball = Instantiate(fireballPrefab, attackPoint, Quaternion.identity);
                fireball.name = "Boss Fireball";
                Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
                fireball.GetComponent<Fireball>().isPlayerFireball = false;
                rb.AddForce(lookDir * fireballForce, ForceMode2D.Impulse);
            }
        }
        for (float x = -0.75f; x <= 1f; x += 1f)
        {
            for (float y = -0.75f; y <= 1f; y += 1f)
            {
                if (x == 0 && y == 0) continue;
                Vector2 lookDir = new Vector2(x, y);
                Vector2 attackPoint = new Vector2(attackPos.x + lookDir.x * 3, attackPos.y + lookDir.y * 3);
                GameObject fireball = Instantiate(fireballPrefab, attackPoint, Quaternion.identity);
                fireball.name = "Boss Fireball";
                Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
                fireball.GetComponent<Fireball>().isPlayerFireball = false;
                rb.AddForce(lookDir * fireballForce, ForceMode2D.Impulse);
            }
        }
    }

    void AttackMeteor()
    {
        animator.SetTrigger("Attack");
		GameObject.Instantiate(meteorPrefab, new Vector3(player.position.x, player.position.y, 0), Quaternion.identity);
        for (int i = 0; i < meteorPos.Length; i++)
        {
            int idx = Random.Range(0, meteorPos.Length);
            GameObject.Instantiate(meteorPrefab, meteorPos[idx], Quaternion.identity);
        }
    }

    void OpenShield()
    {
        currentShieldDuration = shieldDuration;
    }

    void AttackSlow()
    {
        animator.SetTrigger("Attack");
        PlayerMovement player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        player.SetMoveSpeed(0.7f, 8f);
    }

    void AttackBullet()
    {
        Vector3 dir = (player.position - tf.position).normalized;
        Vector2 dir2 = new Vector2(dir.x, dir.y);
        Vector3 pos = new Vector3(tf.position.x + (dir.x * 2), tf.position.y + (dir.y * 2), 0);
        GameObject bullet = GameObject.Instantiate(redBulletPrefab, pos, Quaternion.identity);
        R_bullet b = bullet.GetComponent<R_bullet>();
        b.SetMoveDirection(dir2);
    }

    void Death() {
        Destroy(gameObject);
        GameManager.instance.EndGame();
    }


    // Take Damage

    void TakeDamage(float damage)
    {
        health -= damage * ((currentShieldDuration >= 0f) ? 0.2f : 1f);
        health = health < 0 ? 0 : health;
    }
}
