using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField]
    private Obstacles[] obstacles = null;


    public void ShatterAllObstacles()
    {
        if (transform.parent != null)
        {
            transform.parent = null;
        }


        foreach (Obstacles item in obstacles)
        {
            item.Shatter();
        }

        StartCoroutine(RemoveAllShatterParts());


    }

    IEnumerator RemoveAllShatterParts()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    
}
