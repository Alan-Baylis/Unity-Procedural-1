using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public bool isDead = false;
    bool isDying = false;

    public bool m_clearUnitOnDeath = true;
    public float m_corpseLifespan = 1;

    public Transform m_unitTransform;

    public float m_unitHeight = 1;
    public float m_unitSpeed = 5;

    private object m_currentTarget;

    public float m_distanceBuffer = 1;

    public int m_TeamID;

    public UnitController m_controller;

    public Rigidbody m_unitRigidbody;

	// Use this for initialization
	void Start () 
    {
        if (m_distanceBuffer == default(float))
        {
            m_distanceBuffer = m_unitHeight;
        }

        m_unitTransform = transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isDying)
        {
            if (!isDead)
            {
                Die();
            }
        }

        if (m_currentTarget != null)
        {
            MoveToTarget();
        }
        else
        {
            
        }
	}

    void MoveToTarget()
    {
        Vector3 locationToMoveTo = new Vector3();

        if(m_currentTarget is Unit)
        {
            locationToMoveTo = (m_currentTarget as Unit).m_unitTransform.position;
        }
        else
        {
            locationToMoveTo = (Vector3)m_currentTarget;
        }

        Vector3 distanceVector = (new Vector3(locationToMoveTo.x, m_unitTransform.position.y, locationToMoveTo.z) - m_unitTransform.position);
        float distanceToTarget = distanceVector.magnitude;

        if (distanceToTarget <= m_distanceBuffer)
        {
            m_unitRigidbody.velocity = new Vector3();
            Debug.DrawLine(m_unitTransform.position, m_unitTransform.position + (m_unitTransform.forward), Color.green);
            return;
        }

        m_unitRigidbody.AddForce(distanceVector.normalized * m_unitSpeed, ForceMode.VelocityChange);
        m_unitTransform.rotation = Quaternion.LookRotation(distanceVector);
        Debug.DrawLine(m_unitTransform.position, m_unitTransform.position + (m_unitTransform.forward), Color.red);
    }

    void FixedUpdate()
    {
        if (m_unitRigidbody.velocity.magnitude > m_unitSpeed)
        {
            m_unitRigidbody.velocity = m_unitRigidbody.velocity.normalized * m_unitSpeed;
        }
    }

    void AttackTarget(UnitAttackType attacktype, int index = 0)
    {

    }

    
    #region ControllerAPI
    public void SetTarget(Vector3 targetLoc)
    {
        m_currentTarget = targetLoc;
    }

    public void SetTarget(Unit targetUnit)
    {
        m_currentTarget = targetUnit;
    }

    public void Die()
    {
        isDying = true;
        isDead = true;

        if (m_clearUnitOnDeath)
        {
            Destroy(gameObject, m_corpseLifespan);
        }
    }

    public float GetFloor()
    {
        Ray originRay = new Ray(new Vector3(m_unitTransform.position.x, m_unitTransform.position.y + m_unitHeight, m_unitTransform.position.z), -transform.up);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(originRay, out hit, m_unitHeight * 100, 1 << 8))
        {
            return hit.point.y;
        }

        return int.MinValue;
    }
    #endregion
}
