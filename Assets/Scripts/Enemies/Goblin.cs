using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : EnemiesBase
{
    Player player;

    [Tooltip("Minimum distance to player for detection")]
    [SerializeField] float MinPlayerDist = 3;
    [Tooltip("Minimum distance to player to attack")]
    [SerializeField] float AttackDist = 0.75f;

    private new void Awake()
    {
        base.Awake();
        player = Player.instance;
    }


    //Go after player if in distance, if not do basic enemy things
    private new void Update()
    {
        if (player!=null && Vector2.Distance(player.transform.position, transform.position) > MinPlayerDist)
            base.Update();
        else if(!MyAnimator.GetBool("Death"))
        {
            int Dir = 0;
            if (Vector2.Distance(player.transform.position, transform.position)>AttackDist)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    Dir = -1;
                    MyAnimator.SetInteger("HorizontalD", -1);
                }
                else if (player.transform.position.x > transform.position.x)
                {
                    Dir = 1;
                    MyAnimator.SetInteger("HorizontalD", 1);
                }
            }
            float Dist = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (Dist <= AttackDist && !MyAnimator.GetBool("Attack"))
            {
                if (player.transform.position.x < transform.position.x)
                {
                    MyAnimator.SetInteger("HorizontalD", -1);
                }
                else if (player.transform.position.x > transform.position.x)
                {
                    MyAnimator.SetInteger("HorizontalD", 1);
                }
                MyAnimator.SetBool("Attack", true);
            }
            else if (!Hitted)
            {
                float step = Speed * Dir * Time.deltaTime;
                if (Dist < step)
                    step = Dist;
                transform.position += (Vector3)(Vector2.right * step);
            }
        }
    }
}
