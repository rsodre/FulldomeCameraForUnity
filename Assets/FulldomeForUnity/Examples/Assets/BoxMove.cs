using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 pos = transform.localPosition;
		pos.y = Mathf.PerlinNoise( pos.x, pos.z + Time.time ) * 4.0f;
		transform.localPosition = pos;
    }
}
