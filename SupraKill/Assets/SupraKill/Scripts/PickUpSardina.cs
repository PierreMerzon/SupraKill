using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSardina : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PJ")
        {
            Destroy(gameObject);
        }
    }
}
