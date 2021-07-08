using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Movement : MonoBehaviour
{
    Animator MyAnimator;
    Rigidbody2D MyRigidbody2D;
    BoxCollider2D MyBoxCollider2D;
    public bool grounded = false;
    [SerializeField] MainIput MainInputAsset;
    float HorizontalDir = 0;
    bool jump = false;
    [SerializeField] float JumpMaxTime = 2f;
    float jumpTime = 0f;
    [SerializeField] float Speed = 1, SpeedInAir = 0.5f, JumpSpeed = 7.5f, fallingSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = GetComponent<Animator>();
        MyRigidbody2D = GetComponent<Rigidbody2D>();
        MyBoxCollider2D = GetComponent<BoxCollider2D>();
        MainInputAsset = new MainIput();
        MainInputAsset.Enable();
        MainInputAsset.Main.Jump.performed += perf => {
            if(perf.phase == InputActionPhase.Performed)
            {
                if(grounded)
                    jump = true;
            }  
        };
        MainInputAsset.Main.Jump.canceled += canc => {
            if (canc.phase == InputActionPhase.Canceled)
            {
                jump = false;
            }
        };
        MainInputAsset.Main.Hotizontal.performed += perf => {
            if (perf.phase == InputActionPhase.Performed)
            {
                HorizontalDir = perf.ReadValue<float>();
                //Debug.Log(HorizontalDir);
            }
        };
        MainInputAsset.Main.Hotizontal.canceled += canc => {
            if (canc.phase == InputActionPhase.Canceled)
            {
                HorizontalDir = 0;
                MyAnimator.SetInteger("HorizontalD", 0);
                //Debug.Log(HorizontalDir);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (jump && jumpTime < JumpMaxTime)
        {
            jumpTime += Time.deltaTime;
        }
        else
            jump = false;
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position-(Vector2.up*(MyBoxCollider2D.size.y/4)), MyBoxCollider2D.size*0.9f,0,Vector2.down,1f,LayerMask.GetMask("Default"));
        if(hit && hit.collider.gameObject.tag == "Ground")
        {
            grounded = true;
            jumpTime = 0;
        }
        else
        {
            //Debug.Log(hit.collider.gameObject.name);
            grounded = false;
        }
        Vector2 velocity = MyRigidbody2D.velocity;
        if (jump)
            velocity.y = JumpSpeed;
        if (!grounded)
        {
            if (MyRigidbody2D.velocity.y > 0)
            {
                MyAnimator.SetInteger("VerticalD", 1);
            }
            else
            {
                MyAnimator.SetInteger("VerticalD", -1);
            }
            if (!jump)
            {
                velocity.y = -fallingSpeed;
            }
        }
        else
        {
            if (MyAnimator.GetInteger("VerticalD") != 0)
                MyAnimator.SetInteger("VerticalD", 0);
        }
        if (HorizontalDir != 0)
        {
            if (grounded)
            {
                MyAnimator.SetInteger("HorizontalD", Mathf.RoundToInt(HorizontalDir));
                velocity.x = Speed * HorizontalDir;
            }
            else
                velocity.x = SpeedInAir * HorizontalDir;
        }
        else
        {
            if (grounded)
                velocity.x = 0;
        }
        //Debug.Log(velocity.x);
        MyRigidbody2D.velocity = velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(MyBoxCollider2D)
            Gizmos.DrawWireCube(transform.position - (Vector3.up * (MyBoxCollider2D.size.y/4)), MyBoxCollider2D.size * 0.9f);
    }
}
