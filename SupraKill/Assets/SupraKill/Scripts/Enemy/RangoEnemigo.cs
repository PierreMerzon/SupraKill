using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangoEnemigo : MonoBehaviour
{
    public Animator ani;
    public EnemyController enemigo;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("PJ"))
        {
            ani.SetBool("run", false);
            ani.SetBool("attack", true);
            enemigo.atacando = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
