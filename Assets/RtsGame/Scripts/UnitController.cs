using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {

    public Unit m_possesedUnit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTarget(Vector3 targetLoc)
    {
        m_possesedUnit.SetTarget(targetLoc);
    }
}
