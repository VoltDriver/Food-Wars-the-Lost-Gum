using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer renderer;
    public float spriteChangeTime;
    private float timer;
    private int spriteIndex;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        timer = 0f;
        spriteIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spriteChangeTime)
        {
            if (spriteIndex == sprites.Length - 1)
            {
                Destroy(gameObject);
                return;
            }
            renderer.sprite = sprites[++spriteIndex];
            timer = 0f;
        }

    }
}
