using System.Collections;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Unity.VisualScripting;
using UnityEngine;

public class PathActor : MonoBehaviour
{
    [SerializeField] private GameObject startingNode;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float movementSpeed;

    private Path path;
    private float movementSpeedProper;

    public GameObject CurrentPosition
    {
        get => currentPosition;
        private set => currentPosition = value;
    }

    private int edgeIndex;
    private bool isMoving = false;
    private GameObject currentPosition;

    private void Start()
    {
        transform.position = startingNode.transform.position;
        currentPosition = startingNode;
    }

    private void Update()
    {
        if (!isMoving && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform.CompareTag("PathfindingTargets"))
            {
                // raycast hit this gameobject
                edgeIndex = 0;
                isMoving = true;
                transform.position = currentPosition.transform.position;
                path = PathController.Singleton.GetPath(currentPosition, hit.transform.gameObject);
                currentPosition = hit.transform.gameObject;
                movementSpeedProper = movementSpeed * path.Distance.Meters;
            }
        }



        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            path = PathController.Singleton.GetPath(startingNode, endNode);
            transform.position = Helpers.RayTAStarPositionToVec3(path.Edges[0].Start.Position);
            edgeIndex = 0;
            currentPosition = endNode;
        }*/

        if (path != null)
        {
            if (path.Edges.Count > edgeIndex)
            {
                var currentEdge = path.Edges[edgeIndex];
                if (currentEdge != null &&
                    Vector3.Distance(transform.position, Helpers.RayTAStarPositionToVec3(currentEdge.End.Position)) < 0.01f)
                {
                    edgeIndex++;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, Helpers.RayTAStarPositionToVec3(currentEdge.End.Position),
                        movementSpeedProper * Time.deltaTime * 0.06f);
                }
            }
            else
            {
                path = null;
                isMoving = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            gameManager.EndGame();
            Destroy(gameObject);
        }
    }
}