using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMap {

    private Dictionary<Vector3, Voxel> m_worldVoxels;

    public Vector3 m_worldSize;
    public int m_maxVoxels;

    public const int HASH_P1 = 100663319;
    public const int HASH_P2 = 201326611;
    public const int HASH_P3 = 50331653;

    public Vector3[] GetAllVoxels()
    {
        Vector3[] returnArray = new Vector3[m_worldVoxels.Keys.Count];
        m_worldVoxels.Keys.CopyTo(returnArray, 0);
        return returnArray;
    }

    public WorldMap()
    {
    }

    public WorldMap(Vector3 size)
    {
        Init(size);
    }

    public WorldMap(int x, int y, int z)
    {
        Init(new Vector3(x, y, z));
    }

    private void Init(Vector3 size, int? seed = null)
    {
        m_worldSize = size;
        m_maxVoxels = (int)(m_worldSize.x * m_worldSize.y * m_worldSize.z);
        m_worldVoxels = new Dictionary<Vector3, Voxel>(m_maxVoxels);

        SetWorldSeed(seed.HasValue ? seed.Value : Random.Range(int.MinValue, int.MaxValue));
    }

    private void SetWorldSeed(int newSeed)
    {
        Random.seed = newSeed;
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        Vector3 voxHash = GetHash(x, y, z);
        if (m_worldVoxels.ContainsKey(voxHash))
        {
            return m_worldVoxels[voxHash];
        }
        //Debug.Log(string.Format("No Voxel Found at {0}", voxHash));
        return null;
    }

    public void SetVoxel(int x, int y, int z, Voxel vox)
    {
        Vector3 voxHash = GetHash(x, y, z);
        if (m_worldVoxels.ContainsKey(voxHash))
        {
            m_worldVoxels[voxHash] = vox;
        }
        else
        {
            m_worldVoxels.Add(voxHash, vox);
        }
    }

    public Vector3 GetHash(Vector3 voxelLocation)
    {
        int x = (int)voxelLocation.x;
        int y = (int)voxelLocation.y;
        int z = (int)voxelLocation.z;

        return GetHash(x, y, z);
    }

    public Vector3 GetHash(int x, int y, int z)
    {
        return new Vector3(x, y, z);
    }


}
