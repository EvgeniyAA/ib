using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    private Rigidbody rb;
    private Transform box;
    private bool hasJoint;
    // Use this for initialization
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void Update()
    {
        Move();
        if (hasJoint && Input.GetKey(KeyCode.Space))
            MakeAShot();
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Box") &&
            !hasJoint)
        {
            box = collision.gameObject.transform;
            box.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            hasJoint = true;
            box.GetComponent<Rigidbody>().mass = 0.1f;
        }
    }

    private void Move()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float speedRotation = 3;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.down*speedRotation);
            if(hasJoint)
                box.Rotate(Vector3.down*speedRotation);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up*speedRotation);
            if (hasJoint)
                box.Rotate(Vector3.up * speedRotation);
        }

        rb.AddForce(transform.forward*Speed*moveVertical);
    }

    private void MakeAShot()
    {
        Destroy(gameObject.GetComponent<FixedJoint>());
        box.gameObject.GetComponent<Rigidbody>().velocity = transform.forward*Speed*5;
        box.GetComponent<Rigidbody>().mass = 1;
        box = null;
        hasJoint = false;
    }

}
