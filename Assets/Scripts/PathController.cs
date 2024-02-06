using System;
using System.Collections;
using System.Collections.Generic;
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

    private Path output;

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
    }

    private void Start()
    {
        CreateGraph();
    }

    private void CreateGraph()
    {
        var maxAgentSeed = Velocity.FromKilometersPerHour(100);

        gameObjectToNode = new Dictionary<GameObject, Node>();

        foreach (var connection in nodeConnections)
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

        foreach (var connection in nodeConnections)
        {
            gameObjectToNode[connection.node1].Connect(gameObjectToNode[connection.node2], maxAgentSeed);
            gameObjectToNode[connection.node2].Connect(gameObjectToNode[connection.node1], maxAgentSeed);
        }
    }

    public Path GetPath(GameObject startingNode, GameObject endingNode)
    {
        gameObjectToNode.TryGetValue(startingNode, out Node nodeA);
        gameObjectToNode.TryGetValue(endingNode, out Node nodeB);

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
                    Gizmos.color = new Color((MathF.Sin(node.node1.transform.position.x)+1)/2, (MathF.Sin(node.node1.transform.position.y) + 1) / 2, 0);
                    Gizmos.DrawLine(node.node1.transform.position, node.node2.transform.position);
                    Gizmos.DrawSphere(node.node1.transform.position, 0.2f);
                    Gizmos.DrawSphere(node.node2.transform.position, 0.2f);
                }
            }
        }
    }
}
