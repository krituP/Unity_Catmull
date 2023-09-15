using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public LineRenderer lr;
    public GameObject controlPoint_prefab;

    public List<Vector2> controlPoints;

    public int substeps = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Add new control point
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            controlPoints.Add(new Vector2(worldPos.x, worldPos.y));

            var newControlPoint = Instantiate(controlPoint_prefab, worldPos, transform.rotation);
            newControlPoint.transform.parent = transform;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // Clear plot
            controlPoints.Clear();
            lr.positionCount = 0;
           
            
            //Debug.Log("Destroyed R");
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        DrawSpline();
    }

    private void DrawSpline()
    {

        //Debug.Log("Drawing spline");
        lr.positionCount = Mathf.Max(0, (controlPoints.Count - 3) * substeps);

        // Set initial point
        if (lr.positionCount > 0)
        {
            lr.positionCount++;
            Vector2 catmull_pt = CatmullRom.Evaluate(0, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3]);
            lr.SetPosition(0, new Vector3(catmull_pt.x, catmull_pt.y, 0f));
        }

        // Draw Catmull-Rom points
        for (int i = 0; i < controlPoints.Count - 3; i++)
        {
            for (int j = 1; j <= substeps; j++)
            {
                float t = (float)j / substeps;
                Vector2 catmull_pt = CatmullRom.Evaluate(t, controlPoints[i], controlPoints[i+1], controlPoints[i+2], controlPoints[i+3]);
                lr.SetPosition(i * substeps + j, new Vector3(catmull_pt.x, catmull_pt.y, 0f));
            }
        }
    }
}
