using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMover : MonoBehaviour
{
    public float Amplitude = 1.0f;
    public float Frequency = 1.0f;

    private Vector3 origin;
    private float offset;


    // Use this for initialization
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        offset = Mathf.Sin(Time.time * Frequency * 4.0f) * Amplitude;
        transform.position = origin + Vector3.right * offset;
    }
}
