using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Roy_T.AStar;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using Random = UnityEngine.Random;

public class PathController : MonoBehaviour
{
    public static PathController Singleton;
    [SerializeField] private NodeConnection[] nodeConnections;
    [SerializeField] private Dictionary<GameObject, Node> gameObjectToNode;
    [Header("Create")]
    [SerializeField] private bool createMode;
    [SerializeField] private Sprite circleSprite;

    private Path output;
    private List<NodeConnection> currentConnections;
    private HashSet<GameObject> allNodes = new();


    private List<LineRenderer> lrs = new List<LineRenderer>();

    private void Awake()
    {
        if (Singleton != null)
        {
            Debug.Log($"Too many of {this.GetType()} Deleting this one");
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }

        foreach (var connection in nodeConnections)
        {
            allNodes.Add(connection.node1);
            allNodes.Add(connection.node2);
        }

        currentConnections = nodeConnections.ToList();

        if (createMode)
        {
            for (var i = 0; i < nodeConnections.Length; i++)
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

            foreach (var node in allNodes)
            {
                node.AddComponent<SpriteRenderer>().sprite = circleSprite;
            }
        }
    }

    private void Start()
    {
        CreateGraph();
    }

    private void Update()
    {

    }

    public void SetStateOfNodes(bool state, params GameObject[] nodes)
    {
        foreach (var node in nodes)
        {
            node.SetActive(state);
        }
        if (state)
        {
            currentConnections = currentConnections
                .Concat(nodeConnections.Where(x => nodes.Contains(x.node1) || nodes.Contains(x.node2))).ToList();
        }
        else
        {
            currentConnections = currentConnections
                .Except(currentConnections.Where(x => nodes.Contains(x.node1) || nodes.Contains(x.node2))).ToList();
        }

        CreateGraph();
    }

    private void CreateGraph()
    {
        var maxAgentSeed = Velocity.FromKilometersPerHour(100);

        gameObjectToNode = new Dictionary<GameObject, Node>();

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

    public Path GetPath(GameObject startingNode, GameObject endingNode)
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
        public GameObject node1;
        public GameObject node2;
    }

    private void OnDrawGizmos()
    {
        if (output != null)
        {
            foreach (var edge in output.Edges)
            {
                Gizmos.DrawLine(new Vector3(edge.Start.Position.X, edge.Start.Position.Y), new Vector3(edge.End.Position.X, edge.End.Position.Y));
            }
        }
        else
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
}
