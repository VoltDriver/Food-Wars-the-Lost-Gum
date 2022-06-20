using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFloatingEffectControl : MonoBehaviour
{

    private float time = 0;
    private float scale = 0.05f;
    private float speed = 4f;
    private float originalY;
    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // use sin to make item has a floating apperance
        time += Time.deltaTime;
        float yMovementAmount = Mathf.Sin(time * speed) * scale;

        transform.position = new Vector3(transform.position.x, originalY+yMovementAmount, transform.position.z);
    }
}
