using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public int rotationModifier;
    public int lineModifier;
    public bool activated = false;

    private float timer = 0;

    private void Update()
    {
        if (activated == true)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                activated = false;
                timer = 0;
            }
        }
    }
}
