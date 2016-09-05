using UnityEngine;
using System.Collections;

public class CubeCreator : MonoBehaviour
{
    public GameObject Brick;
    public GameObject Plane;
    private GameObject cube1;
    private GameObject cube2;
    private float minX;
    private float maxX;
    public GameObject Cube1
    {
        get
        {
            return cube1;
        }
    }

    public GameObject Cube2
    {
        get { return cube2; }
    }

    void Start()
    {
        Mesh planeMesh = Plane.GetComponent<MeshFilter>().mesh;
        minX = -planeMesh.bounds.size.x*Plane.transform.localScale.x/2;
        maxX = planeMesh.bounds.size.x* Plane.transform.localScale.x/2;
        cube1 = (GameObject)Instantiate(Brick, new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX)), Quaternion.identity);
        cube1.name = "cube1";
        cube2 = (GameObject)Instantiate(Brick, new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX)), Quaternion.identity);
        cube2.name = "cube2";
    }

    void Update()
    {
        if (cube1.transform.position.y <= -5)
            UpdatePosition(cube1);
        if (cube2.transform.position.y <= -5)
            UpdatePosition(cube2);
    }

    void UpdatePosition(GameObject cube)
    {
        cube.transform.position=new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX));
    }
}


