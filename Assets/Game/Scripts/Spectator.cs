using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float force;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    public void Jump()
    {
        if (IsGrounded())
        {
            //  rigidbody.AddForce(Vector3.up * force);
            force = (float)Random.Range(350, 550) / 100.0f;
            rigidbody.velocity = Vector3.up * force;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down , transform.localScale.x );
    }
}
