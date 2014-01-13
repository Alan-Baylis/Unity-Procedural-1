using UnityEngine;
using System.Collections.Generic;

public class LevelBounds : MonoBehaviour {

    public LineRenderer m_boundryLines;

    public int numSteps = 10;

	// Use this for initialization
	void Start () {

        Vector3[] points = GetLinePoints();

        m_boundryLines.SetVertexCount(points.Length);

        for (int index = 0; index < points.Length; index++)
        {
            m_boundryLines.SetPosition(index, points[index]);
        }
	}

    public Vector3[] GetLinePoints()
    {
        Bounds boundry = collider.bounds;

        List<Vector3> points = new List<Vector3>();

        float minX = boundry.center.x - (boundry.extents.x);
        float maxX = boundry.center.x + (boundry.extents.x);

        float minZ = boundry.center.z - (boundry.extents.z);
        float maxZ = boundry.center.z + (boundry.extents.z);

        float lastHeight = boundry.center.y - (boundry.extents.y);

        //Go MinX -> MaxX along MinZ
        float currentX = minX;
        float stepSize = (maxX - minX) / numSteps;
        for (int step = 0; step < numSteps; step++)
        {
            currentX = minX + (step * stepSize);
            float height = GetHeight(currentX, minZ);

            height = (height == int.MinValue ? lastHeight : height);
            lastHeight = (height == int.MinValue ? lastHeight : height);

            points.Add(new Vector3(currentX, height, minZ));
        }

        //Go MinZ -> MaxZ anong MaxX
        float currentZ = minZ;
        stepSize = ((maxZ - minZ) / numSteps);
        for (int step = 0; step < numSteps; step++)
        {
            currentZ = minZ + (step * stepSize);
            float height = GetHeight(maxX, currentZ);

            height = (height == int.MinValue ? lastHeight : height);
            lastHeight = (height == int.MinValue ? lastHeight : height);

            points.Add(new Vector3(maxX, height, currentZ));
        }

        //Go MaxX -> MinX along MaxZ
        currentX = maxX;
        stepSize = ((maxX - minX) / numSteps);
        for (int step = 0; step < numSteps; step++)
        {
            currentX = maxX - (step * stepSize);
            float height = GetHeight(currentX, maxZ);

            height = (height == int.MinValue ? lastHeight : height);
            lastHeight = (height == int.MinValue ? lastHeight : height);

            points.Add(new Vector3(currentX, height, maxZ));
        }

        //Go MaxZ -> MinZ along MinX
        currentZ = maxZ;
        stepSize = ((maxZ - minZ) / numSteps);
        for (int step = 0; step <= numSteps; step++)
        {
            currentZ = maxZ - (step * stepSize);
            float height = GetHeight(minX, currentZ);

            height = (height == int.MinValue ? lastHeight : height);
            lastHeight = (height == int.MinValue ? lastHeight : height);

            points.Add(new Vector3(minX, height, currentZ));
        }

        return points.ToArray();
    }

    public float GetHeight(float x, float z)
    {
        Ray originRay = new Ray(new Vector3(x, collider.bounds.center.y + collider.bounds.extents.y, z), -transform.up);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(originRay, out hit, collider.bounds.size.y *2, 1 << 8))
        {
            return hit.point.y;
        }

        return int.MinValue;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
