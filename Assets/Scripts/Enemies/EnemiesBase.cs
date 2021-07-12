using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class EnemiesBase : MonoBehaviour
{
    [SerializeField] Vector2[] PatrolPoints;
    [SerializeField] int MaxHP = 3;
    [Tooltip("Minimum and maximum value of coins to drop after enemy death")]
    [SerializeField] int MinCoins = 1, MaxCoins = 10;
    [Tooltip("Wait on patrol point after reaching it")]
    [SerializeField] float DelayOnPatrolPoint = 2f;
    [Tooltip("Walk speed")]
    [SerializeField] protected float Speed = 0.5f;
    [SerializeField] GameObject CoinPrefab;

    protected Animator MyAnimator;
    Collider2D MyColllider2D;

    int ActualHP;
    int PatrolPointGoing = 0;
    protected bool Hitted = false;
    bool Arrive = false;
    protected void Awake()
    {
        MyAnimator = GetComponent<Animator>();
        MyColllider2D = GetComponent<Collider2D>();
        ActualHP = MaxHP;
    }

    //check if patrol points are not in the ground
    void Start()
    {
#if UNITY_EDITOR
        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            PatrolPoints[i].y = transform.position.y;
            RaycastHit2D hit = Physics2D.BoxCast(PatrolPoints[i], Vector2.one, 0, Vector2.zero, 0f, LayerMask.GetMask("Default"));
            if (hit && hit.collider.tag == "Ground")
            {
                Debug.LogError("Patrol point nr: " + i + " in collider");
                EditorApplication.isPlaying = false;
            }
        }
#endif
    }

    //go betweend patrol points, wait after reaching actual patrol point or stay still of no patrol points defined
    protected void Update()
    {
        if (PatrolPoints.Length > 0)
        {
            int Dir=0;
            if (!Arrive)
            {
                if (PatrolPoints[PatrolPointGoing].x < transform.position.x)
                {
                    Dir = -1;
                    MyAnimator.SetInteger("HorizontalD", -1);
                }
                else if (PatrolPoints[PatrolPointGoing].x > transform.position.x)
                {
                    Dir = 1;
                    MyAnimator.SetInteger("HorizontalD", 1);
                }
            }
            float Dist = Mathf.Abs(PatrolPoints[PatrolPointGoing].x - transform.position.x);
            if (Dist <= 0.1f && MyAnimator.GetInteger("HorizontalD")!=0)
            {
                Arrive = true;
                StartCoroutine(WaitAndGoNext());
            }
            else if(!Hitted)
            {
                float step = Speed*Dir*Time.deltaTime;
                if (Dist < step)
                    step = Dist;
                transform.position += (Vector3)(Vector2.right * step);
            }
            else
            {
                MyAnimator.SetInteger("HorizontalD", 0);
            }
        }
        else
        {
            MyAnimator.SetInteger("HorizontalD", 0);
        }
    }

    //wait after reaching actual patrol point
    IEnumerator WaitAndGoNext()
    {
        MyAnimator.SetInteger("HorizontalD", 0);
        yield return new WaitForSeconds(DelayOnPatrolPoint);
        PatrolPointGoing = (PatrolPointGoing + 1) % PatrolPoints.Length;
        Arrive = false;
    }

    IEnumerator cor = null;
    //take damage
    public void TakingDMG(int val)
    {
        MyAnimator.SetBool("Hit",true);
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
        cor = StayAfterHit(0.25f);
        StartCoroutine(cor);
        ActualHP -= val;
        if (ActualHP <= 0)
        {
            MyColllider2D.enabled = false;
            MyAnimator.SetBool("Death", true);
        }
    }

    //stop enemy for a while after being hitted
    IEnumerator StayAfterHit(float Delay)
    {
        Hitted = true;
        yield return new WaitForSeconds(Delay);
        Hitted = false;
        cor = null;
    }

    //kill enemy
    void Death()
    {
        for(int i = 0; i < Random.Range(MinCoins, MaxCoins+1); i++)
        {
            Rigidbody2D coin = Instantiate(CoinPrefab, transform.position,Quaternion.identity).GetComponent<Rigidbody2D>();
            coin.AddForce(Vector2.up*5,ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    //draw patrol points after selecting enemy in editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < PatrolPoints.Length; i++)
        {
            PatrolPoints[i].y = transform.position.y;
            Gizmos.color = Color.blue;
            RaycastHit2D hit = Physics2D.BoxCast(PatrolPoints[i], Vector2.one, 0, Vector2.zero, 0f, LayerMask.GetMask("Default"));
            if(hit && hit.collider.tag == "Ground")
                Gizmos.color = Color.red;
            Handles.Label(PatrolPoints[i]-(Vector2.right/2.5f), "Patrol point nr: "+i);
            Gizmos.DrawWireCube(PatrolPoints[i], Vector3.one);
        }
    }
#endif
}
