using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform[] points;  //Array de puntos de posición hacia los que la plataforma se moverá
    [SerializeField] int startingPoint;  //Número para determinar el índice del punto de inicio de la plataforma
    private int i; //Índice del array
    // Start is called before the first frame update
    void Start()
    {
        //Setear la posición inicial de la plataforma a uno de los puntos, asignando a startingpoint un valor numérico
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; //Aumenta el índice, cambia de objetivo
            if (i == points.Length)  //Chequea si la plataforma ha llegado al último punto
            {
                i = 0; //Resetea el índice para que vuelva a empezar
            }
        }
        //Mueve la plataforma a la posición del punto guardado en el Array en el espacio con valor igual a "i"
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}
