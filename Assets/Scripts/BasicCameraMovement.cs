using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraMovement : MonoBehaviour
{
    public float movSpeed = 100, rotSpeed = 100;
    private float movSpeeddelta, rotSpeeddelta;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movSpeeddelta = movSpeed * Time.deltaTime; rotSpeeddelta = rotSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //if (Input.GetKey(KeyCode.S))
            //{
            //    transform.Rotate(Vector3.left, -rotSpeeddelta);
            //}

            //if (Input.GetKey(KeyCode.W))
            //{
            //    transform.Rotate(Vector3.left, rotSpeeddelta);
            //}

            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.up, -rotSpeeddelta);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.up, rotSpeeddelta);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.down * movSpeeddelta);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * movSpeeddelta);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * movSpeeddelta);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * movSpeeddelta);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * movSpeeddelta);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * movSpeeddelta);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.rotation = (Quaternion.identity);
        }
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus))
        {
            movSpeed += 0.5f;
        }
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            movSpeed -= 0.5f;
        }
    }
}
