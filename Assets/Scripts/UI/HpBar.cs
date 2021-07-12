using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] GameObject HeartPrefab;
    [Tooltip("Space between heart icons")]
    [SerializeField] float space;

    List<Animator> Hearts = new List<Animator>();

    //Set value of HB Bar
    public void SetVal(int ActualVal)
    {
        for(int i = 0; i < Hearts.Count; i++)
        {
            if (i < ActualVal)
                Hearts[i].SetBool("Lost",false);
            else
                Hearts[i].SetBool("Lost", true);
        }
    }

    //Set maximum value of HP Bar and adjust icons count
    public void SetMaxVal(int MV)
    {
        while (Hearts.Count > MV)
        {
            Destroy(Hearts.Last().gameObject);
        }
        while (Hearts.Count < MV) {
            Animator heart = Instantiate(HeartPrefab, transform).GetComponent<Animator>();
            heart.transform.localPosition += (Vector3)Vector2.right*((Hearts.Count > 0?Hearts.Last().transform.localPosition.x+space:0));
            Hearts.Add(heart);
        }
    }
}
