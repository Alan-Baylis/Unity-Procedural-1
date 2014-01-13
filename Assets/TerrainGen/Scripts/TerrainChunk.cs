using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TerrainChunk : MonoBehaviour {

    public bool m_isActive = false;
    public bool m_redrawMesh = false;

    public Vector2 m_chunkIndex;
    private List<Vector3> m_verts;
    private List<Vector3> m_normals;
    private List<Vector2> m_uvs;
    private List<int> m_tris;

    public Vector3 m_VoxelStartIndex;

    public WorldMap m_map;

    public MeshFilter m_meshFilter;

    public MeshCollider m_meshCollider;

    private Mesh m_mesh;

    public KeyCode debugRedrawKey = KeyCode.F5;

    public KeyCode debugRegenKey = KeyCode.F6;

    public int m_chunkMaxX;
    public int m_chunkMaxZ;
    public int m_chunkMaxY;

    #region Reference Variables

    Vector2 N_00 = new Vector2(0f, 0f);
    Vector2 N_10 = new Vector2(1f, 0f);
    Vector2 N_01 = new Vector2(0f, 1f);
    Vector2 N_11 = new Vector2(1f, 1f);

    #endregion

    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            if (m_mesh == null || m_redrawMesh)
            {
                m_redrawMesh = false;
                StartCoroutine(ReDrawTerrain());
            }
        }
        else
        {
            m_mesh = null;
            m_meshFilter.mesh = m_mesh;
            m_meshCollider.sharedMesh = m_mesh;
        }
	}

    

    public IEnumerator ReDrawTerrain()
    {
        m_mesh = new Mesh();

        RenderSection((int)m_VoxelStartIndex.x, (int)m_VoxelStartIndex.y, (int)m_VoxelStartIndex.z);
        yield return null;
    }

    private void RenderSection(int voxStartX, int voxStartY, int voxStartZ)
    {
        List<Vector3> allVerts = new List<Vector3>(m_chunkMaxX*8);

        List<Vector3> allNormals = new List<Vector3>(m_chunkMaxX*8);

        List<Vector2> allUVs = new List<Vector2>(m_chunkMaxX*8);

        List<int> allTris = new List<int>(m_chunkMaxX*36);
        List<List<int>> subMatTris = new List<List<int>>();
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            subMatTris.Add(new List<int>());
        }

        for(int x = voxStartX; x < voxStartX + m_chunkMaxX; x++)
        {
            for(int z = voxStartZ; z < voxStartZ + m_chunkMaxZ; z++)
            {
                for (int y = voxStartY; y < voxStartY + m_chunkMaxY; y++)
                {
                    Mesh VoxMesh = GenerateVoxMesh(m_map, x, y, z);

                    Voxel vox = m_map.GetVoxel(x, y, z);
                    if (vox != null)
                    {
                        subMatTris[(int)vox.m_voxelType].AddRange(GetOffsetTris(allVerts.Count, VoxMesh));
                        allTris.AddRange(GetOffsetTris(allVerts.Count, VoxMesh));
                        allVerts.AddRange(GetOffsetVertices(new Vector3(1, 1, 1), VoxMesh, new Vector3(x, y, z)));
                        allNormals.AddRange(VoxMesh.normals);
                        allUVs.AddRange(VoxMesh.uv);
                    }
                }
            }
        }

        //Debug.Log(allVerts.Count);

        /*
        Vector3[] verts = new Vector3[allVerts.Count];
        int[] tris = new int[allTris.Count];
        Vector2[] uvs = new Vector2[allUVs.Count];
        Vector3[] normals = new Vector3[allNormals.Count];

        
        print("length of Verts" + verts.Length.ToString());
        print("length of tris" + tris.Length.ToString());
        print("length of uvs" + uvs.Length.ToString());
        print("length of normals" + normals.Length.ToString());
        
 
        AppendList<Vector3>(ref verts, allVerts);
        //print("finished Adding all verts");
        AppendList<Vector3>(ref normals, allNormals);
        //print("finished Adding all normals");
        AppendList<Vector2>(ref uvs, allUVs);
        //print("finished Adding all UVs");
        AppendList<int>(ref tris, allTris);
        //print("finished Adding all Tris");
        */

        Vector3[] verts = allVerts.ToArray();
        int[] tris = allTris.ToArray();
        Vector2[] uvs = allUVs.ToArray();
        Vector3[] normals = allNormals.ToArray();

        m_mesh.vertices = verts;
        m_mesh.triangles = tris;
        m_mesh.uv = uvs;
        m_mesh.normals = normals;

        m_mesh.subMeshCount = renderer.materials.Length;
        for (int i = 0; i < subMatTris.Count; i++)
        {
            if (subMatTris[i].Count > 0)
            {
                m_mesh.SetTriangles(subMatTris[i].ToArray(), i);
            }
        }

        m_mesh.RecalculateBounds();
        m_mesh.Optimize();
        m_mesh.RecalculateNormals();
        m_meshFilter.mesh = m_mesh;
        m_meshCollider.sharedMesh = m_mesh;
    }

    private void AppendList<T>(ref T[] baseArray, List<T[]> joinArray)
    {
        int index = 0;
        foreach (T[] subArray in joinArray)
        {
            foreach (T item in subArray)
            {
                baseArray[index] = item;
                index++;
            }
        }
    }

    private void AppendList<T>(ref T[] baseArray, T[] joinArray)
    {
        int index = 0;
        foreach (T item in joinArray)
        {
            baseArray[index] = item;
            index++;
        }
    }

    private void AppendList<T>(ref T[] baseArray, List<T> joinArray)
    {
        int index = 0;
        foreach (T item in joinArray)
        {
            baseArray[index] = item;
            index++;
        }
    }

    private Mesh GenerateVoxMesh(WorldMap map, int x, int y, int z)
    {
        Voxel vox = map.GetVoxel(x, y, z);
        if (vox != null)
        {
            bool bTop = map.GetVoxel(x, y + 1, z) != null;
            bool bBottom = map.GetVoxel(x, y - 1, z) != null;
            bool bLeft = map.GetVoxel(x - 1, y, z) != null;
            bool bRight = map.GetVoxel(x + 1, y, z) != null;
            bool bFront = map.GetVoxel(x, y, z + 1) != null;
            bool bBack = map.GetVoxel(x, y, z - 1) != null;

            return GenerateCube(bTop, bBottom, bLeft, bRight, bFront, bBack);
        }
        return new Mesh();
    }

    private Mesh GenerateCube(bool blockedTop, bool blockedBottom, bool blockedLeft, bool blockedRight, bool blockedFront, bool blockedBack)
    {
        float length = 1;
        float width = 1;
        float height = 1;

        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        
        List<Vector3> verts = new List<Vector3>(26);
        //List<Vector3> norms = new List<Vector3>(26);
        List<Vector2> uv = new List<Vector2>(26);
        List<int> tris = new List<int>(36);

        int FaceCount = 0;
        if (!blockedBottom)
        {
            verts.AddRange( new Vector3[]{p0, p1, p2, p3});
            //norms.AddRange(new Vector3[]{V_DOWN, V_DOWN, V_DOWN, V_DOWN});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }

        if (!blockedLeft)
        {
            verts.AddRange( new Vector3[]{p7, p4, p0, p3});
            //norms.AddRange(new Vector3[]{V_LEFT, V_LEFT, V_LEFT, V_LEFT});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }

        if (!blockedFront)
        {
            verts.AddRange( new Vector3[]{p4, p5, p1, p0});
            //norms.AddRange(new Vector3[]{V_FRONT, V_FRONT, V_FRONT, V_FRONT});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }

        if (!blockedBack)
        {
            verts.AddRange( new Vector3[]{p6, p7, p3, p2});
            //norms.AddRange(new Vector3[]{V_BACK, V_BACK, V_BACK, V_BACK});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }

        if (!blockedRight)
        {
            verts.AddRange( new Vector3[]{p5, p6, p2, p1});
            //norms.AddRange(new Vector3[]{V_RIGHT, V_RIGHT, V_RIGHT, V_RIGHT});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }

        if (!blockedTop)
        {
            verts.AddRange( new Vector3[]{p7, p6, p5, p4});
            //norms.AddRange(new Vector3[]{V_UP, V_UP, V_UP, V_UP});
            uv.AddRange(new Vector2[]{N_11, N_01, N_00, N_10});
            tris.AddRange(
                   new int[] {3 + (4 * FaceCount), 1 + (4 * FaceCount), 0 + (4 * FaceCount),
	                          3 + (4 * FaceCount), 2 + (4 * FaceCount), 1 + (4 * FaceCount)});
            FaceCount++;
        }
        int[] triangles = tris.ToArray();
        Vector3[] vertices = verts.ToArray();
        //Vector3[] normales = norms.ToArray();
        Vector2[] uvs = uv.ToArray();
        
        Mesh returnMesh = new Mesh();
        returnMesh.vertices = vertices;
        //returnMesh.normals = normales;
        returnMesh.uv = uvs;
        returnMesh.triangles = triangles;

        returnMesh.RecalculateBounds();
        returnMesh.Optimize();
        returnMesh.RecalculateNormals();

        return returnMesh;
    }

    public Vector3[] GetOffsetVertices(Vector3 voxelScale, Mesh voxelMesh, Vector3 chunkLocation)
    {
        List<Vector3> offsetVerts = new List<Vector3>(voxelMesh.vertices);
        float xOffset = chunkLocation.x * voxelScale.x;
        float yOffset = chunkLocation.y * voxelScale.y;
        float zOffset = chunkLocation.z * voxelScale.z;
        for (int i = 0; i < offsetVerts.Count; i++)
        {
            offsetVerts[i] = new Vector3(offsetVerts[i].x + xOffset, offsetVerts[i].y + yOffset, offsetVerts[i].z + zOffset);
        }

        return offsetVerts.ToArray();
    }

    public int[] GetOffsetTris(int triOffset, Mesh voxelMesh)
    {
        List<int> offsetTris = new List<int>(voxelMesh.triangles);
        for (int i = 0; i < offsetTris.Count; i++)
        {
            offsetTris[i] += triOffset;
        }

        return offsetTris.ToArray();
    }
}
