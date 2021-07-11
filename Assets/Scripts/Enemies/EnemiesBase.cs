using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class EnemiesBase : MonoBehaviour
{
    [SerializeField] Vector2[] PatrolPoints;
    [SerializeField] int MaxHP = 3;
    [SerializeField] float DelayOnPatrolPoint = 2f;
    [SerializeField] float Speed = 0.5f;

    Animator MyAnimator;

    int ActualHP;
    int PatrolPointGoing = 0;
    bool Hitted = false;
    bool Arrive = false;
    private void Awake()
    {
        MyAnimator = GetComponent<Animator>();

        ActualHP = MaxHP;
    }
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

    // Update is called once per frame
    void Update()
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
    }

    IEnumerator WaitAndGoNext()
    {
        MyAnimator.SetInteger("HorizontalD", 0);
        yield return new WaitForSeconds(DelayOnPatrolPoint);
        PatrolPointGoing = (PatrolPointGoing + 1) % PatrolPoints.Length;
        Arrive = false;
    }
    IEnumerator cor = null;
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
            MyAnimator.SetBool("Death", true);
    }

    IEnumerator StayAfterHit(float Delay)
    {
        Hitted = true;
        yield return new WaitForSeconds(Delay);
        Hitted = false;
        cor = null;
    }

    void Death()
    {
        Destroy(gameObject);
    }

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
