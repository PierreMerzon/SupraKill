using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Inicio del singleton básico
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("AudioManager is null!");
            }
            return instance;
        }
    }

    //Fin del Singleton básico, se referencia en el awake

    private void Awake()
    {
        instance = this;
    }

    //Variables

    [Header("General Data")]
    public int points;
    public int winPoints;
    public int score;
    public int maxScore;
    public int health;
    public int energy;

    public void PointsUp(int gain)
    {
        points += gain;
    }

    [Header("Game Status")]
    public bool gameCompleted = false;

}
