using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PickUp : MonoBehaviour
{
    Animator MyAnimator;
    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MyAnimator.SetBool("PU",true);
    }

    void PickedUp()
    {
        GameManager.instance.AddCoin();
        Destroy(gameObject);
    }
}
