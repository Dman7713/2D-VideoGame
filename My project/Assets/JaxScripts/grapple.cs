using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapple : MonoBehaviour
{
    public Camera cam;
    public LineRenderer lr;

    public LayerMask grappleMask; // What You can grapple on to.
    public float moveSpeed = 2f;
    public float grappleLeanght = 10;

    public int maxPoints = 3;

    private Rigidbody2D rig;
    private List<Vector2> points = new List<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 MousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (MousePos - (Vector2)transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grappleLeanght, grappleMask);
            if (hit.collider != null)
            {
                Vector2 hitPoint = hit.point;
                points.Add(hitPoint);

                if(points.Count > maxPoints)
                {
                    points.RemoveAt(0);
                }
            }
        }

        if(points.Count > 0)
        {
            Vector2 moveTo = centroid(points.ToArray());
            rig.MovePosition(Vector2.MoveTowards(transform.position, moveTo, Time.deltaTime * moveSpeed));

            lr.positionCount = 0;
            lr.positionCount = points.Count * 2;
            for(int n=0,j=0; n < points.Count *2; n+=2, j++)
            {
                lr.SetPosition(n, transform.position);
                lr.SetPosition(n + 1, points[j]);
            }
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            Detatch();
        }

    }

    void Detatch() 
    {
        lr.positionCount = 0;
        points.Clear();
    }

    Vector2 centroid(Vector2[] points)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points) 
        {
            center += point;
        }
        center /= points.Length;
        return center;
    }
}
