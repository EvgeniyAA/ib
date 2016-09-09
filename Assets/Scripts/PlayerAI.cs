using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerAI : MonoBehaviour
    {
        public float speed;
        public float distance;
        private StateType _stateType;
        private GameObject _player;
        private GameObject _aimedCube;
        private readonly List<GameObject> _cubes = new List<GameObject>();
        private Rigidbody _rb;
        private float _movement;

        private bool IsCubeOwned
        {
            get { return _aimedCube.GetComponent<CubesInformation>().hasOwner; }
            set { _aimedCube.GetComponent<CubesInformation>().hasOwner = value; }
        }

        private void Start()
        {
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
            _stateType = StateType.SearchForCube;
        }

        private void Update()
        {
            Debug.Log(_stateType);
            switch (_stateType)
            {
                case StateType.Nothing:
                    break;
                case StateType.SearchForCube:
                    SearchForCube();
                    break;
                case StateType.RotateToCube:
                    RotateToCube();
                    break;
                case StateType.MoveToCube:
                    MoveToCube();
                    break;
                case StateType.PickUpCube:
                    _movement = 0;
                    break;
                case StateType.RotateToPlayer:
                    RotateToPlayer();
                    break;
                case StateType.MoveToPlayer:
                    MoveToPlayer();
                    break;
                case StateType.MakeAShot:
                    MakeAShot();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            } 
        }

        private void SearchForCube()
        {
            IEnumerable<GameObject> freeCubes =
                _cubes.Where(gameObject => !gameObject.GetComponent<CubesInformation>().hasOwner);
            if (freeCubes.Any())
            {
                _aimedCube = freeCubes.Minimum(gameObject => Vector3.Distance(gameObject.transform.position, transform.position));
                _stateType = StateType.RotateToCube;
            }
        }

        private void RotateToCube()
        {
            if (!IsCubeOwned)
            {
                var aimedPos = new Vector3(_aimedCube.transform.position.x - transform.position.x, 0, _aimedCube.transform.position.z - transform.position.z);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimedPos), 10);
                var aimedPos2 = new Vector3(transform.position.x, 0, transform.position.z);
                var cubePos = new Vector3(_aimedCube.transform.position.x, 0, _aimedCube.transform.position.z);
                if (Vector3.Angle(transform.forward, cubePos - aimedPos2) <= 5)
                    _stateType = StateType.MoveToCube;
            }
            else
            {
                _stateType = StateType.SearchForCube;
            }
        }

        private void MoveToCube()
        {
            if (!IsCubeOwned)
            {
                if (Vector3.Angle(transform.forward, _aimedCube.transform.position - transform.position) > 10&&Vector3.Distance(transform.forward, _aimedCube.transform.position)>10)
                    _stateType = StateType.RotateToCube;
                _movement = 1;
                Move();
            }
            else _stateType = StateType.SearchForCube;
        }

        private void RotateToPlayer()
        {
            var aimedPos = new Vector3(_player.transform.position.x - transform.position.x, 0, _player.transform.position.z - transform.position.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimedPos), 3);
            _aimedCube.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
            if (Vector3.Angle(transform.forward, _player.transform.position - transform.position) <= 1f)
                _stateType = StateType.MoveToPlayer;

        }

        private void MoveToPlayer()
        {
            _movement = 1;
            if (distance < Vector3.Distance(transform.position, _player.transform.position))
                Move();
            else
                _stateType = StateType.MakeAShot;
        }


        private IEnumerator WaitAndFire()
        {
            _movement = 0;
            _stateType = StateType.Nothing;
            yield return new WaitForSeconds(1.5f);
            MakeAShot();
        }

        private void MakeAShot()
        {
            if (Vector3.Angle(transform.forward, _player.transform.position - transform.position) <= 1f)
            {
                _stateType = StateType.Nothing;
                Destroy(gameObject.GetComponent<SpringJoint>());
                //_aimedCube.GetComponent<Rigidbody>().isKinematic = false;
                _aimedCube.GetComponent<Rigidbody>().velocity = transform.forward*speed/5;
                _aimedCube.GetComponent<Rigidbody>().mass = 50;
                _aimedCube.GetComponent<CubesInformation>().hasOwner = false;
                _stateType = StateType.SearchForCube;
            }
            else _stateType = StateType.RotateToPlayer;
        }

        private void Move()
        {
            _rb.AddForce(transform.forward * speed * _movement);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CubesInformation cubesInformation = collision.gameObject.GetComponent<CubesInformation>();
            if (cubesInformation == null)
                return;
            if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Box") && !cubesInformation.hasOwner)
            {
                _stateType = StateType.PickUpCube;
                collision.gameObject.transform.position = transform.forward + new Vector3(transform.position.x, 5.5f, transform.position.z);
                gameObject.AddComponent<SpringJoint>();
                gameObject.GetComponent<SpringJoint>().connectedBody = collision.rigidbody;
                collision.gameObject.GetComponent<Rigidbody>().mass = 0.1f;
                //collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
                cubesInformation.hasOwner = true;
                _stateType = StateType.RotateToPlayer;
            }
            else if (cubesInformation.hasOwner)
            {
                _stateType = StateType.SearchForCube;
            }
        }
    }
}
