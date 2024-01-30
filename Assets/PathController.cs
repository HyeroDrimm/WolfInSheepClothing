using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathController : MonoBehaviour
{
    public PathController Singleton;
    [SerializeField] private NodeConnection [] nodeConnections;
    private PathGraph<GameObject> graph;

    private void Awake ()
    {
        if (Singleton != null)
        {
            Debug.Log ($"Too many of {this.GetType ()} Deleting this one");
            Destroy (this);
        }
        else
        {
            Singleton = this;
        }
    }

    public PathRequestOutput GetPath(GameObject startingNode, GameObject endingNode)
    {
        var output = new PathRequestOutput ();

        return output;
    }

    public void CreateGraphFromNodesConnections()
    {
        graph = new PathGraph<GameObject> ();
        foreach (var node in nodeConnections)
        {
            graph.nodes.
        }
    }

    public class PathRequestOutput
    {
        public bool isValid;
        public List<GameObject> path;
    }


    [Serializable]
    public class NodeConnection
    {
        public GameObject node1;
        public GameObject node2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (var node in nodeConnections)
        {
            if (node != null && node.node1 != null && node.node2 != null)
            {
                Gizmos.DrawLine (node.node1.transform.position, node.node2.transform.position);
                Gizmos.DrawSphere (node.node1.transform.position, 0.2f);
                Gizmos.DrawSphere (node.node2.transform.position, 0.2f);
            }
        }
    }
}
