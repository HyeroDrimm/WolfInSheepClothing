using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class BoardManager : MonoBehaviour
{
    [SerializeField] private List<PathNode> nodes;
    [SerializeField] private List<NodeConnection> nodeConnections;
    [SerializeField] private Dictionary<PathNode, Node> gameObjectToNode;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("Destruction")]
    [SerializeField] private AnimationCurve destructionCurve;
    [SerializeField] private float maxDestructionTime;
    [SerializeField] private int maxDestructorsAtTime;
    [SerializeField] private int newDestructorsAmount;
    [SerializeField] private float startingTimeBetweenDestructors;
    [SerializeField] private float timeBetweenDestructors;
    [SerializeField] private float timeToExplode;

    [Header("Create")]
    [SerializeField] private bool createMode;

    private HashSet<NodeConnection> currentConnections;
    private List<LineRenderer> lrs = new List<LineRenderer>();
    private HashSet<PathNode> explodedNodes;


    private void Awake()
    {
        currentConnections = nodeConnections.ToHashSet();

        if (createMode)
        {
            for (var i = 0; i < nodeConnections.Count; i++)
            {
                var connection = nodeConnections[i];
                var newGameObject = new GameObject($"LineRenderer{i}");
                newGameObject.transform.parent = transform;
                var lr = newGameObject.AddComponent<LineRenderer>();
                lrs.Add(lr);

                lr.startColor = Color.white;
                lr.endColor = Color.white;
                lr.widthMultiplier = 0.25f;
                lr.positionCount = 2;
                lr.SetPosition(0, connection.node1.gameObject.transform.position);
                lr.SetPosition(1, connection.node2.gameObject.transform.position);
            }
        }

        foreach (var pathNode in nodes)
        {
            pathNode.Setup(player, enemy, this);
        }

        player.Setup(this, enemy);
        enemy.Setup(this, player);

        InvokeRepeating("SpawnDestructors", startingTimeBetweenDestructors, timeBetweenDestructors);
    }

    private void Start()
    {
        CreateGraph();
    }

    public void SetStateOfNodes(bool state, params PathNode[] nodes)
    {
        // TODO add list of destroyed nodes
        if (state)
        {
            currentConnections.UnionWith(nodeConnections.Where(x => nodes.Contains(x.node1) || nodes.Contains(x.node2)));
        }
        else
        {
            currentConnections.RemoveWhere(x => nodes.Contains(x.node1) || nodes.Contains(x.node2));
        }

        CreateGraph();
    }

    private void CreateGraph()
    {
        var maxAgentSeed = Velocity.FromKilometersPerHour(100);

        gameObjectToNode = new Dictionary<PathNode, Node>();

        foreach (var connection in currentConnections)
        {
            if (!gameObjectToNode.ContainsKey(connection.node1))
            {
                var node = new Node(new Position(connection.node1.transform.position.x,
                    connection.node1.transform.position.y));
                gameObjectToNode.Add(connection.node1, node);
            }

            if (!gameObjectToNode.ContainsKey(connection.node2))
            {
                var node = new Node(new Position(connection.node2.transform.position.x,
                    connection.node2.transform.position.y));
                gameObjectToNode.Add(connection.node2, node);
            }
        }

        foreach (var connection in currentConnections)
        {
            gameObjectToNode[connection.node1].Connect(gameObjectToNode[connection.node2], maxAgentSeed);
            gameObjectToNode[connection.node2].Connect(gameObjectToNode[connection.node1], maxAgentSeed);
        }
    }

    public Path GetPath(PathNode startingNode, PathNode endingNode)
    {
        gameObjectToNode.TryGetValue(startingNode, out Node nodeA);
        gameObjectToNode.TryGetValue(endingNode, out Node nodeB);

        if (nodeA == null || nodeB == null || gameObjectToNode.Count == 0)
        {
            return null;
        }

        var maxAgentSeed = Velocity.FromKilometersPerHour(100);


        var pathFinder = new PathFinder();
        var path = pathFinder.FindPath(nodeA, nodeB, maximumVelocity: maxAgentSeed);

        Console.WriteLine($"type: {path.Type}, distance: {path.Distance}, duration {path.Duration}");

        return path;
    }

    [Serializable]
    public class NodeConnection
    {
        public PathNode node1;
        public PathNode node2;

        public NodeConnection(PathNode node1, PathNode node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }
    }

    private void OnDrawGizmos()
    {
        // if (output != null)
        // {
        //     foreach (var edge in output.Edges)
        //     {
        //         Gizmos.DrawLine(new Vector3(edge.Start.Position.X, edge.Start.Position.Y), new Vector3(edge.End.Position.X, edge.End.Position.Y));
        //     }
        // }
        // else
        {
            foreach (var node in nodeConnections)
            {
                if (node != null && node.node1 != null && node.node2 != null)
                {
                    Gizmos.color = new Color((MathF.Sin(node.node1.transform.position.x) + 1) / 2, (MathF.Sin(node.node1.transform.position.y) + 1) / 2, 0);
                    Gizmos.DrawLine(node.node1.transform.position, node.node2.transform.position);
                    Gizmos.DrawSphere(node.node1.transform.position, 0.2f);
                    Gizmos.DrawSphere(node.node2.transform.position, 0.2f);
                }
            }
        }
    }

#if UNITY_EDITOR

    [Button]
    public void MakeConnection()
    {
        var objects = Selection.gameObjects;
        if (objects.Length == 2 && objects[0].TryGetComponent(out PathNode pathNode1) && objects[1].TryGetComponent(out PathNode pathNode2))
        {
            Undo.RecordObject(this, "Add new connection");
            nodeConnections.Add(new NodeConnection(pathNode1, pathNode2));
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
    }
#endif
    public void AddNode(PathNode pathNode)
    {
        throw new NotImplementedException();
    }

    public void RemoveNode(PathNode pathNode)
    {
        throw new NotImplementedException();
    }

    private void SpawnDestructors()
    {
        var time = Mathf.Clamp01(Time.timeSinceLevelLoad / maxDestructionTime);

        var toSpawnNumber = 2 + Mathf.FloorToInt(destructionCurve.Evaluate(time) * newDestructorsAmount);

        var destructosAvaliable = nodes.Where(x => x.State != PathNode.DestructorState.On && x.State != PathNode.DestructorState.Exploded).ToList();
        var destructosOn = nodes.Where(x => x.State == PathNode.DestructorState.On).ToList();

        for (int i = 0; i < Mathf.Min(maxDestructorsAtTime - destructosOn.Count, toSpawnNumber); i++)
        {
            if (destructosAvaliable.Count != 0)
            {
                var newPickUp = destructosAvaliable.Random();
                newPickUp.StartTimer(timeToExplode);
            }
            else
            {
                break;
            }
        }
    }
}
