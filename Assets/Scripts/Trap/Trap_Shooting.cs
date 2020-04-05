﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Shooting : MonoBehaviour
{

    public Transform firePoint;
    public GameObject arrowPrefab;
    public Animator animator;
    
    public float arrowForce = 20f;
    public float delayTime = 1f;
    public float shootInterval;

	float currentDelayTime = 0f;

    void Start()
    {
        animator.SetFloat("Interval",shootInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skull_Delay")){
            if (currentDelayTime >= delayTime){
                animator.SetTrigger("Shoot");
            }else{
                currentDelayTime += Time.deltaTime;
            }
        }
    }

    void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * -1 * arrowForce, ForceMode2D.Impulse);
    }
}
