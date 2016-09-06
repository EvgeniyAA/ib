using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {
    private static List<GameObject> _cubes = new List<GameObject>();
    private static List<GameObject> _players = new List<GameObject>();
    [Header("Prefabs")]
    [SerializeField]
    private GameObject _brick;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private GameObject _plane;
    [Header("Materials")]
    [SerializeField]
    private Material _myPlayer;
    [SerializeField]
    private Material _enemyPlayer;
    private int _level;

    private float minX;
    private float maxX;

    public static List<GameObject> cubes
    {
        get { return _cubes; }
    }

    public static List<GameObject> players
    {
        get
        {
            return _players;
        }
    }

    // Use this for initialization
    private void Start () {
        _level = 1;
        RestartLevel();
    }

    private void RestartLevel()
    {
        _players.Clear();
        _cubes.Clear();
        AddPlayer("Person");
        Mesh planeMesh = _plane.GetComponent<MeshFilter>().mesh;
        minX = -planeMesh.bounds.size.x * _plane.transform.localScale.x / 2;
        maxX = planeMesh.bounds.size.x * _plane.transform.localScale.x / 2;
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
                player.GetComponent<PlayerController>().Speed = 1000;
            }
            else
            {
                player.AddComponent<PlayerAI>();
                player.GetComponent<PlayerAI>().Speed = 1000;
                player.GetComponent<PlayerAI>().Distance = 15;
            }
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
        foreach (GameObject player in _players)
        {
            if ((player.tag == "Person") &&IsOutside(player) )
            {
                _level = 1;
                RestartLevel();
            }
            if (IsAllEnemiesOutisdeBoard())
            {
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
