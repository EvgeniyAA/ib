using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAI : MonoBehaviour {

    public float speed;
    public float distance;
    private GameObject _player;
    private GameObject aimedCube;
    private readonly List<GameObject> _cubes = new List<GameObject>();
    private Rigidbody _rb;
    private bool _hasJoint;
    private float _movement;
    // Use this for initialization
    public void Start()
    {
        Restart();
    }

    // Update is called once per frame
    public void Update()
    {
        if (WorldManager.isNeedRestart)
            Restart();
        if (!_hasJoint)
        {
            aimedCube = SearchNearestObject(_cubes);
            RotateToObject(aimedCube);
            _movement = 1;
        }
        else
            _movement = 0;
        if(_hasJoint)
        {
            if (Vector3.Angle(transform.forward, _player.transform.position - transform.position) > 1f)
            {
                RotateToObject(_player);       
            }
            else if (Vector3.Distance(transform.position, _player.transform.position) > distance)
                _movement = 1;
            else
            {
                _movement = 0;
                StartCoroutine(WaitAndFire());
            }
        }
        Move();
    }

    private void Restart()
    {
        Debug.Log("restart");
            _cubes.Clear();
            _hasJoint = false;
            _player = null;
            _rb = GetComponent<Rigidbody>();
            foreach (GameObject cube in WorldManager.Cubes)
            {
                _cubes.Add(cube);
            }
            foreach (GameObject player in WorldManager.Players)
            {
                if (player.tag == "Person")
                    _player = player;
            }
    }
    private GameObject SearchNearestObject(List<GameObject> gameObjects)
    {
        GameObject nearestObj = new GameObject("");
        nearestObj.transform.position = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
        foreach (GameObject obj in gameObjects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < Vector3.Distance(transform.position, nearestObj.transform.position))
            {
                nearestObj = obj;
            }
        }
        return nearestObj;
    }

    private IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(1.5f);
        MakeAShot();
    }

    private void MakeAShot()
    {
        Destroy(gameObject.GetComponent<FixedJoint>());
        aimedCube.GetComponent<Rigidbody>().velocity = transform.forward * speed * 5;
        aimedCube.GetComponent<Rigidbody>().mass = 1;
        _hasJoint = false;
    }

    private void RotateToObject(GameObject aimedOjbect)
    {
        var aimedPos = new Vector3(aimedOjbect.transform.position.x- transform.position.x, 0, aimedOjbect.transform.position.z- transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimedPos), 3);
        if (_hasJoint)
            aimedCube.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
    }

    private void Move()
    {
        _rb.AddForce(transform.forward * speed*_movement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Box") &&
            !_hasJoint)
        {
            collision.gameObject.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
            gameObject.AddComponent<FixedJoint>();
            gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
            _hasJoint = true;
            collision.gameObject.GetComponent<Rigidbody>().mass = 0.1f;
        }
    }
}
