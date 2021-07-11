using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    Animator MyAnimator;
    Rigidbody2D MyRigidbody2D;
    BoxCollider2D MyBoxCollider2D;
    SpriteRenderer MySpriteRenderer;

    float HorizontalDir = 0;
    float jumpTime = 0f;
    int ActualHP;
    int dashAnimCount = 0;
    bool jump = false;
    bool grounded = false;
    bool dashing = false;

    [SerializeField] float JumpMaxTime = 2f;
    [SerializeField] float KickBack = 3f;
    [SerializeField] float Speed = 1, JumpSpeed = 7.5f, DashSpeed = 10f;
    [SerializeField] int MaxHP = 3;
    [SerializeField] Slider HPBar;

    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();
        MyRigidbody2D = GetComponent<Rigidbody2D>();
        MyBoxCollider2D = GetComponent<BoxCollider2D>();
        MySpriteRenderer = GetComponent<SpriteRenderer>();

        HPBar.maxValue = MaxHP;
        HPBar.value = MaxHP;

        ActualHP = MaxHP;
    }

    void Start()
    {
        GameManager.instance.MainInputAsset.Main.Jump.performed += perf => {
            if(perf.phase == InputActionPhase.Performed)
            {
                if (grounded)
                {
                    jump = true;
                }
            }  
        };
        GameManager.instance.MainInputAsset.Main.Jump.canceled += canc => {
            if (canc.phase == InputActionPhase.Canceled)
            {
                jump = false;
            }
        };
        GameManager.instance.MainInputAsset.Main.Hotizontal.performed += perf => {
            if (perf.phase == InputActionPhase.Performed)
            {
                if (!dashing && !MyAnimator.GetBool("Hit"))
                    HorizontalDir = perf.ReadValue<float>();
                else
                    HorizontalDir = 0;
            }
        };
        GameManager.instance.MainInputAsset.Main.Hotizontal.canceled += canc => {
            if (canc.phase == InputActionPhase.Canceled)
            {
                HorizontalDir = 0;
                MyAnimator.SetInteger("HorizontalD", 0);
            }
        };
        GameManager.instance.MainInputAsset.Main.Dash.performed += perf => {
            if (perf.phase == InputActionPhase.Performed)
            {
                if (!dashing)
                    dashing=true;
            }
        };
        GameManager.instance.MainInputAsset.Main.Attack.performed += perf => {
            if (perf.phase == InputActionPhase.Performed)
            {
                if (!dashing && !MyAnimator.GetBool("Hit"))
                    MyAnimator.SetBool("Attack", true);
            }
        };
    }

    private void FixedUpdate()
    {
        if(jump)
            MyAnimator.SetInteger("VerticalD", 1);
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position - (Vector2.up * (MyBoxCollider2D.size.y / 10)), MyBoxCollider2D.size * 0.9f, 0, Vector2.down, 0f, LayerMask.GetMask("Default"));
        if (hit)
        {
            if (!grounded)
            {
                if (jumpTime > 0)
                    jumpTime = 0;
            }
            grounded = true;
        }
        else
            grounded = false;

        if (jump && jumpTime < JumpMaxTime)
            jumpTime += Time.deltaTime;
        if (jumpTime >= JumpMaxTime)
            jump = false;
        Vector2 velocity = MyRigidbody2D.velocity;
        if (jump)
        {
            if (jumpTime >= 0.08)
                velocity.y = JumpSpeed;
            else
                MyAnimator.SetBool("Jump", true);
        }
        else if (!jump && jumpTime > 0)
            jumpTime = 0;

        if (HorizontalDir != 0)
        {
            MyAnimator.SetInteger("HorizontalD", Mathf.RoundToInt(HorizontalDir));
            velocity.x = Speed * HorizontalDir;
        }
        else
            if (grounded)
            velocity.x = 0;
        
        if (dashing)
        {
            if (dashAnimCount == 0)
                MyRigidbody2D.AddForce(Vector2.right * (MySpriteRenderer.flipX ? -1 : 1) * DashSpeed, ForceMode2D.Impulse);
            //velocity.x = MyRigidbody2D.velocity.x;
            MyAnimator.SetBool("Dash", true);
        }
        else if(!MyAnimator.GetBool("Hit"))
        {
            MyRigidbody2D.velocity = velocity;
        }
        if (ActualHP <= 0 && !MyAnimator.GetBool("Hit"))
            MyAnimator.SetBool("Death", true);
    }

    void Update()
    {
        if (!grounded)
        {
            if (MyRigidbody2D.velocity.y <= 0)
                MyAnimator.SetInteger("VerticalD", -1);
        }
        else
        {
            if (MyAnimator.GetInteger("VerticalD") != 0)
                MyAnimator.SetInteger("VerticalD", 0);
        }
        if (dashAnimCount >= 3)
        {
            dashAnimCount = 0;
            dashing = false;
            MyAnimator.SetBool("Dash", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {

            MyAnimator.SetBool("Hit", true);
            MyRigidbody2D.AddForce((collision.collider.transform.position-transform.position)*-1*KickBack, ForceMode2D.Impulse);
            Hit();
        }
    }

    public void Attack()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.GetChild(0).position,Vector2.one,0,Vector2.zero);
        if (hit && hit.collider.tag == "Enemy")
            hit.collider.gameObject.SendMessage("TakingDMG", 1);
    }

    void Hit()
    {
        ActualHP--;
        HPBar.value = ActualHP;
    }

    public void Death()
    {
        GameManager.instance.MainInputAsset.Disable();
        Time.timeScale = 0;
        Destroy(gameObject);
    }

    public void AddDashAnimCount()
    {
        dashAnimCount++;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(MyBoxCollider2D)
            Gizmos.DrawWireCube(transform.position - (Vector3.up * (MyBoxCollider2D.size.y/10)), MyBoxCollider2D.size * 0.9f);
    }
#endif
}
