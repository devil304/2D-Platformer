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
    [SerializeField] bool jump = false;
    [SerializeField] float JumpMaxTime = 2f;
    [SerializeField] float jumpTime = 0f;
    [SerializeField] float slowingJumpTime = 0;
    [SerializeField] float Speed = 1, JumpSpeed = 7.5f;
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
                if (grounded)
                {
                    jump = true;
                }
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
    private void FixedUpdate()
    {
        if(jump)
            MyAnimator.SetInteger("VerticalD", 1);
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position - (Vector2.up * (MyBoxCollider2D.size.y / 10)), MyBoxCollider2D.size * 0.9f, 0, Vector2.down, 0f, LayerMask.GetMask("Default"));
        if (hit && hit.collider.gameObject.tag == "Ground")
        {
            if (!grounded)
            {
                if (slowingJumpTime != 0)
                    slowingJumpTime = 0;
                if (jumpTime > 0)
                    jumpTime = 0;
            }
            grounded = true;
        }
        else
        {
            //Debug.Log(hit.collider.gameObject.name);
            grounded = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if ((jump || slowingJumpTime!=0) && jumpTime < JumpMaxTime)
        {
            jumpTime += Time.deltaTime;
        }
        if(jumpTime>= JumpMaxTime)
            jump = false;
        Vector2 velocity = MyRigidbody2D.velocity;
        if (jump)
        {
            if (jumpTime >= 0.08) {
                if (jumpTime >= (JumpMaxTime * (4f/5f))) {
                    slowingJumpEnd(velocity);
                }
                else
                {
                    velocity.y = JumpSpeed;
                }
            } else
                MyAnimator.SetBool("Jump", true);
        }else if (!jump && jumpTime > 0)
        {
            if (jumpTime < JumpMaxTime)
            {
                slowingJumpEnd(velocity);
            }
            if (jumpTime >= (slowingJumpTime + (JumpMaxTime / 5f)))
            {
                jumpTime = 0;
                slowingJumpTime = 0;
            }
        }
        if (!grounded)
        {
            if (MyRigidbody2D.velocity.y <= 0)
            {
                MyAnimator.SetInteger("VerticalD", -1);
            }
        }
        else
        {
            if (MyAnimator.GetInteger("VerticalD") != 0)
                MyAnimator.SetInteger("VerticalD", 0);
        }
        if (HorizontalDir != 0)
        {
            MyAnimator.SetInteger("HorizontalD", Mathf.RoundToInt(HorizontalDir));
            velocity.x = Speed * HorizontalDir;
        }
        else
        {
            if (grounded)
                velocity.x = 0;
        }
        //Debug.Log(velocity.x);
        MyRigidbody2D.velocity = velocity;
    }

    void slowingJumpEnd(Vector2 velocity)
    {
        if (slowingJumpTime == 0)
            slowingJumpTime = jumpTime;
        Debug.Log((1f - ((JumpMaxTime / 5f) - (jumpTime - (slowingJumpTime)))));
        velocity.y = JumpSpeed * (1f - ((JumpMaxTime / 5f) - (jumpTime - (slowingJumpTime))));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(MyBoxCollider2D)
            Gizmos.DrawWireCube(transform.position - (Vector3.up * (MyBoxCollider2D.size.y/10)), MyBoxCollider2D.size * 0.9f);
    }
}
