using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToWhite : MonoBehaviour
{
    public float fadeOutTime = 6;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut(spriteRenderer));
    }

    // set alpha of white screen slowly higher to mimic a fade to white
    IEnumerator FadeOut(SpriteRenderer sprite)
    {
        Color tempColor = sprite.color;

        while(tempColor.a < 1)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;
            sprite.color = tempColor;
            yield return null;
        }
        sprite.color = tempColor;
    }
}
