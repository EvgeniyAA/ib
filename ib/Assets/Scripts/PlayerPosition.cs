using UnityEngine;
using System.Collections;

public class PlayerPosition : MonoBehaviour {
    public GameObject Plane;
    private float minX;
    private float maxX;
    // Use this for initialization
    public void Start () {
        Mesh planeMesh = Plane.GetComponent<MeshFilter>().mesh;
        minX = -planeMesh.bounds.size.x * Plane.transform.localScale.x / 2;
        maxX = planeMesh.bounds.size.x * Plane.transform.localScale.x / 2;
        SetNewPos();
    }
	
	// Update is called once per frame
	public void Update () {
	    if(transform.position.y<=0)
            SetNewPos();
	}

    private void SetNewPos()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX));
    }
}
