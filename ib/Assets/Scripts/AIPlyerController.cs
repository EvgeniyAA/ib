using UnityEngine;
using System.Collections;


public class AIPlyerController : MonoBehaviour
{
    public GameObject OtherPlayer;
    public GameObject Plane;
    private GameObject AI;
    private float minX;
    private float maxX;
    // Use this for initialization
    public void Start()
    {
        Mesh planeMesh = Plane.GetComponent<MeshFilter>().mesh;
        minX = -planeMesh.bounds.size.x*Plane.transform.localScale.x/2;
        maxX = planeMesh.bounds.size.x*Plane.transform.localScale.x/2;
        AI =(GameObject)Instantiate(OtherPlayer, new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX)), Quaternion.identity);
    }

    // Update is called once per frame
    public void Update()
    {
        if (AI.transform.position.y <= 0)
            AI.transform.position= new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX));
    }
}
