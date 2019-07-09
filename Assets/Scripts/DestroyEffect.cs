using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{

    private float time_count;
    // Use this for initialization
    void Start()
    {
        time_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time_count += Time.deltaTime;
        if (time_count > 1.6)
        {
            Destroy(this.gameObject);
        }
    }
}
