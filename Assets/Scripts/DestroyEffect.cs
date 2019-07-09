using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{

    private float time_count;
    public float duration = 1.6f;
    // Use this for initialization
    void Start()
    {
        time_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time_count += Time.deltaTime;
        if (time_count > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
