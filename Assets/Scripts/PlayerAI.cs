using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlayerAI : MonoBehaviour {

    public float Speed;
    public float Distance;
    private GameObject Player;
    private GameObject Cube1;
    private GameObject Cube2;
    private Rigidbody rb;
    private bool hasJoint;
    private float movement;
    // Use this for initialization
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {
        if ((Cube1 != null) && (!hasJoint) && (Cube1.transform.position.y < 6 && Cube1.transform.position.y > 5))
        {
            RotateToObject(Cube1);
            movement = 1;
        }
        else movement = 0;
        if(hasJoint)
        {
            if (Vector3.Angle(transform.forward, Player.transform.position - transform.position) > 1f)
            {
                RotateToObject(Player);       
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) > Distance)
                movement = 1;
            else
            {
                movement = 0;
                StartCoroutine(WaitAndFire());
            }
        }
        Move();
    }

    private IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(1.5f);
        MakeAShot();
    }

    private void MakeAShot()
    {
        Destroy(gameObject.GetComponent<FixedJoint>());
        Cube1.GetComponent<Rigidbody>().velocity = transform.forward * Speed * 5;
        Cube1.GetComponent<Rigidbody>().mass = 1;
        hasJoint = false;
    }

    private void RotateToObject(GameObject aimedOjbect)
    {
        var aimedPos = new Vector3(aimedOjbect.transform.position.x- transform.position.x, 0, aimedOjbect.transform.position.z- transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimedPos), 3);
        if (hasJoint)
            Cube1.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
    }

    private void Move()
    {
        rb.AddForce(transform.forward * Speed*movement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Box") &&
            !hasJoint)
        {
            collision.gameObject.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            hasJoint = true;
            collision.gameObject.GetComponent<Rigidbody>().mass = 0.1f;
        }
    }
}
