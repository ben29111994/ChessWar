using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour
{
    public static Enviroment Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RandomTransform()
    {
        transform.Rotate(new Vector3(0f, Random.Range(0f, 360f), 0f));
    }
}
