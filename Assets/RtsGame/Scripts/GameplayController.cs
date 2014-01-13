using UnityEngine;
using System.Collections;

public class GameplayController : MonoBehaviour {

    public InputSystem m_inputSystem;

    public bool isActive = true;

    public UnitController m_commandingUnit;

	// Use this for initialization
	void Start () 
    {
        if (m_inputSystem == null)
        {
            isActive = false;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(isActive)
        {
            if (m_inputSystem.BUTTON_1)
            {
                object ClickLoc = GetRaycastTarget();
                if (ClickLoc is Unit)
                {
                    SetTarget( (ClickLoc as Unit).m_controller);
                }
                else
                {
                    if (m_commandingUnit != null && ClickLoc is Vector3)
                    {
                        m_commandingUnit.SetTarget((Vector3)ClickLoc);
                    }
                    else
                    {
                        SetTarget(null);
                    }
                }
            }
        }
	}

    public void SetTarget(UnitController target)
    {
        Debug.Log(string.Format("Setting Current Target to '{0}'", target));
        m_commandingUnit = target;
    }

    object GetRaycastTarget()
    {
        Ray originRay = Camera.main.ScreenPointToRay(m_inputSystem.CURSOR_LOCATION);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(originRay, out hit, 10000, 1 << 8 | 1 << 10))
        {
            Unit possibleTarget = hit.collider.GetComponent<Unit>();
            if (possibleTarget == null)
            {
                return hit.point;
            }
            return possibleTarget;
        }

        return null;
    
    }
}
