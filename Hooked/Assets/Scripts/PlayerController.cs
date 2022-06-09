using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool grounded = true;
    public bool wallHit = false;
    public bool move = false;

    public float speed = 1f;


    private int line = 2;
    private float offsetLine = 1.15f;

    private void Update()
    {
        /// pending detect input

        MoveFordward();
    }

    private void MoveFordward()
    {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

}
