using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VoxelWorld : MonoBehaviour {

    public TerrainChunk[, ,] m_allChunks;

    public int m_chunkMaxX = 16;
    public int m_chunkMaxZ = 16;
    public int m_chunkMaxY = 16;

    public GameObject m_terrainChunkPrefab;

    public TerrainChunk nextChunk;
    public TerrainChunk currentChunk;

    public int m_worldWidth = 64;
    public int m_worldHeight = 32;
    public int m_worldDepth = 64;

    public WorldMap m_map;

    public Transform m_playerPOS;
    public float m_farClip;

    private List<TerrainChunk> m_visibleArea;

	// Use this for initialization
	void Start () {
        GenerateVoxelWorld(m_worldWidth, m_worldHeight, m_worldDepth);
	}
	
	// Update is called once per frame
	void Update () {
        RenderTerrain(m_playerPOS.position);
	}

    public void GenerateVoxelWorld(int width, int height, int depth)
    {
        m_worldWidth = width;
        m_worldHeight = height;
        m_worldDepth = depth;
        GenerateFlatTerrain();

        ClearChunks();
        int numChunksX = (int)Mathf.Ceil((float)m_worldWidth / m_chunkMaxX);
        int numChunksZ = (int)Mathf.Ceil((float)m_worldDepth / m_chunkMaxZ);
        int numChunksY = (int)Mathf.Ceil((float)m_worldHeight / m_chunkMaxY);
        m_allChunks = new TerrainChunk[numChunksX,numChunksY,numChunksZ];

        for (int x = 0; x < numChunksX; x++)
        {
            for (int z = 0; z < numChunksZ; z++)
            {
                for (int y = 0; y < numChunksY; y++ )
                {
                    GameObject NewTerrainChunk = Instantiate(m_terrainChunkPrefab) as GameObject;
                    NewTerrainChunk.transform.parent = transform;
                    NewTerrainChunk.transform.localPosition = new Vector3(0,0,0);
                    TerrainChunk tc = NewTerrainChunk.GetComponent<TerrainChunk>();
                    tc.m_map = m_map;
                    tc.m_VoxelStartIndex = new Vector3(x * m_chunkMaxX, y * m_chunkMaxY, z * m_chunkMaxZ);
                    tc.m_chunkMaxX = m_chunkMaxX;
                    tc.m_chunkMaxY = m_chunkMaxY;
                    tc.m_chunkMaxZ = m_chunkMaxZ;

                    m_allChunks[x, y, z] = tc;
                }
            }
        }
    }

    public void RenderTerrain(Vector3 playerPos)
    {
        if (m_visibleArea == null)
        {
            m_visibleArea = new List<TerrainChunk>();
        }

        TerrainChunk[] newChunks = GetVisibleChunks(playerPos);
        foreach (TerrainChunk tc in newChunks)
        {
            if (m_visibleArea.Contains(tc))
            {
                continue;
            }

            tc.m_isActive = true;
            m_visibleArea.Add(tc);
        }

        TerrainChunk[] nonVisibleAreas = m_visibleArea.Where(x => !newChunks.Contains(x)).ToArray();
        foreach (TerrainChunk tc in nonVisibleAreas)
        {
            m_visibleArea.Remove(tc);
            tc.m_isActive = false;
        }

    }

    public TerrainChunk[] GetVisibleChunks(Vector3 location)
    {
        List<TerrainChunk> tcReturns = new List<TerrainChunk>();
        foreach (Collider c in Physics.OverlapSphere(location, m_farClip, 1 << 8))
        {
            TerrainChunk tc = c.GetComponent<TerrainChunk>();
            if (tc != null)
            {
                tcReturns.Add(tc);
            }
        }
        return tcReturns.ToArray();
    }

    public TerrainChunk GetChunkAt(Vector3 location)
    {
        int locX = (int)location.x;
        int locY = (int)location.y;
        int locZ = (int)location.z;

        int chunkX = (int)Mathf.Ceil(locX / m_chunkMaxX);
        int chunkY = (int)Mathf.Ceil(locY / m_chunkMaxY);
        int chunkZ = (int)Mathf.Ceil(locX / m_chunkMaxZ);

        if (chunkX < (int)Mathf.Ceil(m_worldWidth / m_chunkMaxX) &&
            chunkY < (int)Mathf.Ceil(m_worldHeight / m_chunkMaxY) &&
            chunkZ < (int)Mathf.Ceil(m_worldDepth / m_chunkMaxZ))
        {
            return m_allChunks[chunkX, chunkY, chunkZ];
        }
        return null;
    }

    public void ClearChunks()
    {
        if (m_allChunks == null)
        {
            return;
        }

        foreach (TerrainChunk tc in m_allChunks)
        {
            Destroy(tc.gameObject);
        }
    }

    public void GenerateFlatTerrain()
    {
        m_map = new WorldMap(m_worldWidth, m_worldHeight, m_worldDepth);

        for (int x = 0; x < m_worldWidth; x++)
        {
            for (int z = 0; z < m_worldDepth; z++)
            {
                for (int y = 0; y < m_worldHeight; y++)
                {
                    Voxel newVox = new Voxel();
                    newVox.m_hash = m_map.GetHash(x, y, z);
                    if (y >= m_worldHeight-1)
                    {
                        newVox.m_voxelType = TerrainType.GRASS;
                        newVox.m_voxelShape = TerrainShape.NORMAL;
                    }
                    else if (y < m_worldHeight && y > m_worldHeight - 10)
                    {
                        newVox.m_voxelType = TerrainType.DIRT;
                        newVox.m_voxelShape = TerrainShape.NORMAL;
                    }
                    else
                    {
                        newVox.m_voxelType = TerrainType.STONE;
                        newVox.m_voxelShape = TerrainShape.NORMAL;
                    }
                    m_map.SetVoxel(x, y, z, newVox);
                    //Debug.Log(string.Format("Adding Voxel to {0},{1},{2}", x, y, z));
                }
            }
        }
    }
}
