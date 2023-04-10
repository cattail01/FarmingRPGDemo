using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHotReload : MonoBehaviour
{
    private SpriteRenderer sprite;

    private void Update()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
    }


}
