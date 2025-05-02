using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnityEditor;
using UnityEngine;
using static PathNode;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-10)]
public class BoardManager : MonoBehaviour
{
    [SerializeField] private List<PathNode> nodes;
    [SerializeField] private List<NodeConnection> nodeConnections;
    [SerializeField] private Dictionary<PathNode, Node> pathNodeToNode;
    [SerializeField] private Dictionary<Node, PathNode> nodeToPathNode;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [Header("Destruction")]
    [SerializeField] private AnimationCurve destructionCurve;
    [SerializeField] private int maxGlitchesAtTime;
    [SerializeField, Range(0,1)] private float progressAtStart;
    [SerializeField] private float chanceToSeedGlitch = 0.1f;
    [SerializeField] private float startingTimeBetweenGlitches;
    [SerializeField] private float timeBetweenGlitches;
    [SerializeField] private float timeToExplode;

    [Header("Pickups")] 
    [SerializeField] private int maxPickupsAtTime;
    [SerializeField] private float startingTimeBetweenPickups;
    [SerializeField] private float timeBetweenPickups;
    [SerializeField] private float speedUpChance;
    [SerializeField] private float speedDownChance;
    [SerializeField] private float freezeChance;
    [SerializeField] private float coinChance;

    [Header("Feedback")] 
    [SerializeField] private MMF_Player playerFixGlitchFeedback;

    [Header("Create")]
    [SerializeField] private bool createMode;

    private HashSet<NodeConnection> currentConnections;
    private List<LineRenderer> lrs = new List<LineRenderer>();
    private HashSet<PathNode> explodedNodes = new HashSet<PathNode>();
    private Dictionary<PathNode, PathNode[]> nodeNeighbours = new Dictionary<PathNode, PathNode[]>();

    private Helpers.WeightedRandomList<PathNode.PickupType> weightedPickupsRandom;

    private void Awake()
    {
        weightedPickupsRandom =
            new Helpers.WeightedRandomList<PathNode.PickupType>(
                new float[] { speedUpChance, speedDownChance, freezeChance, coinChance, },
                new PathNode.PickupType[] { PickupType.SpeedUp, PickupType.SpeedDown, PickupType.Freeze, PickupType.Coin, });


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

        // Create neighbours dict
        foreach (var node in nodes)
        {
            var list = new List<PathNode>();
            foreach (var connection in nodeConnections)
            {
                if (connection.node1 == node)
                    list.Add(connection.node2);
                if (connection.node2 == node)
                    list.Add(connection.node1);
            }
            nodeNeighbours.Add(node, list.ToArray());
        }

        InvokeRepeating("SpawnGlitches", startingTimeBetweenGlitches, timeBetweenGlitches);
        InvokeRepeating("SpawnPickups", startingTimeBetweenPickups, timeBetweenPickups);
    }

    private void Start()
    {
        CreateGraph();
    }

    public void SetStateOfNodes(bool state, params PathNode[] nodes)
    {
        if (state)
        {
            explodedNodes.RemoveWhere(nodes.Contains);
        }
        else
        {
            explodedNodes.UnionWith(nodes);
        }

        currentConnections = nodeConnections.Where(x => !nodes.Contains(x.node1) && !nodes.Contains(x.node2))
            .ToHashSet();

        CreateGraph();
    }

    private void CreateGraph()
    {
        var maxAgentSeed = Velocity.FromKilometersPerHour(100);

        pathNodeToNode = new Dictionary<PathNode, Node>();

        foreach (var connection in currentConnections)
        {
            if (!pathNodeToNode.ContainsKey(connection.node1))
            {
                var node = new Node(new Position(connection.node1.transform.position.x,
                    connection.node1.transform.position.y));
                pathNodeToNode.Add(connection.node1, node);
            }

            if (!pathNodeToNode.ContainsKey(connection.node2))
            {
                var node = new Node(new Position(connection.node2.transform.position.x,
                    connection.node2.transform.position.y));
                pathNodeToNode.Add(connection.node2, node);
            }
        }

        foreach (var connection in currentConnections)
        {
            pathNodeToNode[connection.node1].Connect(pathNodeToNode[connection.node2], maxAgentSeed);
            pathNodeToNode[connection.node2].Connect(pathNodeToNode[connection.node1], maxAgentSeed);
        }
        nodeToPathNode = pathNodeToNode.ToDictionary(x => x.Value, x => x.Key);
    }

    public Path GetPath(PathNode startingNode, PathNode endingNode)
    {
        pathNodeToNode.TryGetValue(startingNode, out Node nodeA);
        pathNodeToNode.TryGetValue(endingNode, out Node nodeB);

        if (nodeA == null || nodeB == null || pathNodeToNode.Count == 0)
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

    private void SpawnGlitches()
    {
        var countOfGlitchesInProgress = nodes.Count(x => x.State == DestructorState.InProcess);
        var nodeCandidates = nodes.Where(x => x.State == PathNode.DestructorState.Neutral).Shuffle().OrderBy(GetNodeCorruption).ToList();
        if (countOfGlitchesInProgress >= maxGlitchesAtTime || nodeCandidates.Count == 0) return;

        var nodeBestCandidate = Random.value < chanceToSeedGlitch ? nodeCandidates[0] : nodeCandidates[^1];
        nodeBestCandidate.StartTimer(timeToExplode, progressAtStart);
    }

    private float GetNodeCorruption(PathNode pathNode)
    {
        var neighbours = nodeNeighbours[pathNode];

        return neighbours.Sum(neighbour => neighbour.CorruptionFraction);
    }

    public bool IsNodeInAnyPaths(PathNode pathNode)
    {
        var output = false;

        output |= ((IFollowTarget)player).CurrentPosition() == pathNode;

        if (enemy.Path != null)
        {
            output |= nodeToPathNode[(Node)enemy.Path.Edges[0].Start] != pathNode;
            foreach (var pathEdge in enemy.Path.Edges)
            {
                output |= nodeToPathNode[(Node)pathEdge.End] == pathNode;
                if (output)
                    return true;
            }
        }

        if (player.Path != null)
        {
            output |= nodeToPathNode[(Node)player.Path.Edges[0].Start] != pathNode;
            foreach (var pathEdge in player.Path.Edges)
            {
                output |= nodeToPathNode[(Node)pathEdge.End] == pathNode;
                if (output)
                    return true;
            }
        }

        return output;
    }

    private void SpawnPickups()
    {
        var nodeCandidates = nodes.Where(x => !IsNodeInAnyPaths(x) && x.State == PathNode.DestructorState.Neutral && !x.IsPickup).Shuffle().ToList();
        if (nodes.Count(x=>x.IsPickup) >= maxPickupsAtTime || nodeCandidates.Count == 0) return;

        var nodeBestCandidate = nodeCandidates[0];
        
        nodeBestCandidate.ShowPickup(weightedPickupsRandom.GetRandomItem());
    }

    private float DistanceToMovingActors(PathNode pathNode)
    {
        return Vector3.Distance(pathNode.transform.position, player.transform.position) +
               Vector3.Distance(pathNode.transform.position, enemy.transform.position);
    }

    public void OnPlayerStartFix(PathNode pathNode, float expectedTime, float percentOfDone)
    {
        playerFixGlitchFeedback.DurationMultiplier = expectedTime; 
        playerFixGlitchFeedback.PlayFeedbacks();
    }

    public void OnPlayerEndFix(PathNode pathNode)
    {
        if (playerFixGlitchFeedback.IsPlaying)
            playerFixGlitchFeedback.SkipToTheEnd();
    }
}
