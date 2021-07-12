using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PickUp : MonoBehaviour
{
    Collider2D MyCollider2D;
    Animator MyAnimator;
    Rigidbody2D MyRigidbody2D;

    private void Awake()
    {
        MyCollider2D = GetComponent<Collider2D>();
        MyAnimator = GetComponent<Animator>();
        MyRigidbody2D = GetComponent<Rigidbody2D>();
    }

    //Play animation of picking up coin and disable collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            MyRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            MyCollider2D.enabled = false;
            MyAnimator.SetBool("PU", true);
        }
    }

    //Process picking up coin
    void PickedUp()
    {
        GameManager.instance.AddCoin();
        Destroy(gameObject);
    }
}
