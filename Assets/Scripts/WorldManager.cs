using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class WorldManager : MonoBehaviour {
    private static List<GameObject> _cubes = new List<GameObject>();
    private static List<GameObject> _players = new List<GameObject>();
    [SerializeField]
    private GameObject _plane;
    [Header("Prefabs")]
    [SerializeField]
    private GameObject _brick;
    [SerializeField]
    private GameObject _player;
    [Header("Materials")]
    [SerializeField]
    private Material _myPlayer;
    [SerializeField]
    private Material _enemyPlayer;
    [Header("Stats")]
    [SerializeField]
    private int _level;
    [SerializeField]
    private float _playersSpeed;
    [SerializeField]
    private float DistanceForShoot;
    private float minX;
    private float maxX;
    public static bool isNeedRestart;
    public static List<GameObject> Cubes
    {
        get { return _cubes; }
    }

    public static List<GameObject> Players
    {
        get
        {
            return _players;
        }
    }

    // Use this for initialization
    private void Start () {
        Mesh planeMesh = _plane.GetComponent<MeshFilter>().mesh;
        minX = -planeMesh.bounds.size.x * _plane.transform.localScale.x / 2;
        maxX = planeMesh.bounds.size.x * _plane.transform.localScale.x / 2;
        _level = 1;
        RestartLevel();
    }

    private void RestartLevel()
    {
        isNeedRestart = false;
        for (int i = 0; i < _cubes.Count; i++)
        {
            Destroy(_cubes[i]);
        }
        for (int i = 0; i < _players.Count; i++)
        {
            Destroy(_players[i]);
        }
        _players.Clear();
        _cubes.Clear();
        AddPlayer("Person");
        for (int i = 0; i < _level; i++)
        {
            AddPlayer("AI");
        }
        for (int i = 0; i < _players.Count; i++)
        {
            AddCube();
        }
        AddMaterials();
        AddControllers();

    }
    private void AddControllers()
    {
        foreach (GameObject player in _players)
        {
            if (player.tag == "Person")
            {
                player.AddComponent<PlayerController>();
                player.GetComponent<PlayerController>().speed = _playersSpeed;
            }
            else
            {
                player.AddComponent<PlayerAI>();
                player.GetComponent<PlayerAI>().speed = _playersSpeed;
                player.GetComponent<PlayerAI>().distance = DistanceForShoot;
            }
        }
        foreach (GameObject cube in _cubes)
        {
            cube.AddComponent<CubesInformation>();
            cube.GetComponent<CubesInformation>().hasOwner = false;
        }
    }

    private void AddMaterials()
    {
        foreach (GameObject player in _players)
        {
            if (player.tag == "Person")
            {
                player.GetComponent<Renderer>().material = _myPlayer;
                player.name = "Player";
            }
            else
            {
                player.GetComponent<Renderer>().material = _enemyPlayer;
                player.name = "AI Player";
            }
        }
    }

    private void AddPlayer(string playerTag)
    {
        _players.Add((GameObject)Instantiate(_player, new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX)), Quaternion.identity));
        _players[_players.Count - 1].tag = playerTag;
    }

    // Update is called once per frame
    private void Update () {
        foreach(GameObject cube in _cubes)
        if (IsOutside(cube))
            UpdatePosition(cube);
        foreach (GameObject player in _players.ToList())
        {
            if ((player.tag == "Person") && IsOutside(player))
            {
                isNeedRestart = true;
                _level--;
                RestartLevel();
            }
            if (IsAllEnemiesOutisdeBoard())
            {
                isNeedRestart = true;
                _level++;
                RestartLevel();
            }
        }
    }
    private void AddCube()
    {
        _cubes.Add((GameObject)Instantiate(_brick, new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX)), Quaternion.identity));
    }
    private void UpdatePosition(GameObject obj)
    {
        obj.transform.position = new Vector3(Random.Range(minX, maxX), 5.5f, Random.Range(minX, maxX));
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private bool IsOutside(GameObject checkingObject)
    {
        return (checkingObject.transform.position.y <= -5) || (checkingObject.transform.position.y > 7);
    }
    private bool IsAllEnemiesOutisdeBoard()
    {
        int count = 0;
        foreach (GameObject player in _players)
            if(IsOutside(player)&&player.tag == "AI")
                count++;
        return (count == _players.Count - 1);
    }
}
