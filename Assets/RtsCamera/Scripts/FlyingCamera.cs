using UnityEngine;
using System.Collections;

public class FlyingCamera : MonoBehaviour {

    public InputSystem m_inputSystem;

    public Camera m_attachedCamera;

    public Collider m_boundryBox;

    private Bounds m_bounds;

    public float m_cameraSpeed = 50;
    public float m_cameraSpeed_HIGH = 100;

    public float m_cameraLookSpeed = 2;

    public float m_minVertical = 25;
    public float m_maxVertical = 85;

    public float m_minVertical_HIGH = 50;
    public float m_maxVertical_HIGH = 85;

    public float m_minHeight = 1;
    public float m_maxHeight = 100;

    public float m_minXLoc = -100;
    public float m_maxXLoc = 100;

    public float m_minZLoc = -100;
    public float m_maxZLoc = 100;

    public bool isActive = true;

    private Rigidbody m_rigidBody;
    private Transform m_transform;

	// Use this for initialization
	void Start () 
    {
        m_rigidBody = rigidbody;
        m_transform = transform;
        if (m_inputSystem == null || m_rigidBody == null)
        {
            isActive = false;
        }

        m_bounds = m_boundryBox.bounds;

        m_minXLoc = m_bounds.center.x - (m_bounds.extents.x);
        m_maxXLoc = m_bounds.center.x + (m_bounds.extents.x);

        m_minHeight = m_bounds.center.y - (m_bounds.extents.y);
        m_maxHeight = m_bounds.center.y + (m_bounds.extents.y);

        m_minZLoc = m_bounds.center.z - (m_bounds.extents.z);
        m_maxZLoc = m_bounds.center.z + (m_bounds.extents.z);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isActive)
        {
            m_attachedCamera.enabled = true;
            MoveCamera();
            RotateCamera();
        }
        else
        {
            m_attachedCamera.enabled = false;
        }
	}

    private void MoveCamera()
    {
        float currentHeightPercent = (m_transform.position.y - m_minHeight) / (m_maxHeight - m_minHeight);
        float currentCameraSpeed = Mathf.Lerp(m_cameraSpeed, m_cameraSpeed_HIGH, currentHeightPercent);

        Vector3 cameraVelocity = new Vector3();
        if (m_inputSystem.FORWARD)
        {
            cameraVelocity += m_transform.forward;
        }
        if (m_inputSystem.BACK)
        {
            cameraVelocity += -m_transform.forward;
        }
        if (m_inputSystem.LEFT)
        {
            cameraVelocity += -m_transform.right;
        }
        if (m_inputSystem.RIGHT)
        {
            cameraVelocity += m_transform.right;
        }
        if (m_inputSystem.UP)
        {
            cameraVelocity += m_transform.up;
        }
        if (m_inputSystem.DOWN)
        {
            cameraVelocity += -m_transform.up ;
        }

        float zoomAxis = -m_inputSystem.GetAxis(InputSystem.AxisIndex.ZERO);
        if (zoomAxis > 0.5 || zoomAxis < -0.5)
        {
            cameraVelocity += m_transform.up * Mathf.RoundToInt(Mathf.Clamp(zoomAxis, -1, 1));
        }

        cameraVelocity *= currentCameraSpeed;

        cameraVelocity = ClampToBounds(cameraVelocity);

        m_rigidBody.velocity = cameraVelocity;
    }

    private Vector3 ClampToBounds(Vector3 origVelocity)
    {
        Vector3 newPos = m_transform.position;

        if (CheckXBounds() > 0 && origVelocity.x > 0)
        {
            newPos.x = m_maxXLoc;
            origVelocity.x = 0;
        }
        else if (CheckXBounds() < 0 && origVelocity.x < 0)
        {
            newPos.x = m_minXLoc;
            origVelocity.x = 0;
        }

        if (CheckYBounds() > 0)
        {
            newPos.y = m_maxHeight;
            if (origVelocity.y > 0)
            {
                origVelocity.y = 0;
            }
        }
        else if (CheckYBounds() < 0)
        {
            newPos.y = m_minHeight;
            if (origVelocity.y < 0)
            {
                origVelocity.y = 0;
            }
        }

        if (CheckZBounds() > 0 && origVelocity.z > 0)
        {
            newPos.z = m_maxZLoc;
            origVelocity.z = 0;
        }
        else if (CheckZBounds() < 0 && origVelocity.z < 0)
        {
            newPos.z = m_minZLoc;
            origVelocity.z = 0;
        }
        if (newPos != m_transform.position)
        {
            m_transform.position = newPos;
        }
        return origVelocity;
    }

    private int CheckXBounds()
    {
        if (m_transform.position.x < m_minXLoc)
        {
            return -1;
        }
        if (m_transform.position.x > m_maxXLoc)
        {
            return 1;
        }
        return 0;
    }

    private int CheckYBounds()
    {
        if (m_transform.position.y < m_minHeight)
        {
            return -1;
        }
        if (m_transform.position.y > m_maxHeight)
        {
            return 1;
        }
        return 0;
    }

    private int CheckZBounds()
    {
        if (m_transform.position.z < m_minZLoc)
        {
            return -1;
        }
        if (m_transform.position.z > m_maxZLoc)
        {
            return 1;
        }
        return 0;
    }

    private void RotateCamera()
    {
        float currentHeightPercent = (m_transform.position.y - m_minHeight) / (m_maxHeight - m_minHeight);
        float currentMinVert = Mathf.Lerp(m_minVertical, m_minVertical_HIGH, currentHeightPercent);
        float currentMaxVert = Mathf.Lerp(m_maxVertical, m_maxVertical_HIGH, currentHeightPercent);

        Vector3 eulerAngles = m_attachedCamera.transform.localEulerAngles;
        eulerAngles.y = 0;
        eulerAngles.z = 0;

        Vector3 go_eulerAngles = m_transform.eulerAngles;
        go_eulerAngles.z = 0;

        if (m_inputSystem.BUTTON_2)
        {
            eulerAngles.x += -Mathf.Clamp(m_inputSystem.GetAxis(InputSystem.AxisIndex.VERTICAL), -m_cameraLookSpeed, m_cameraLookSpeed);

            go_eulerAngles.y += Mathf.Clamp(m_inputSystem.GetAxis(InputSystem.AxisIndex.HORIZONTAL), -m_cameraLookSpeed, m_cameraLookSpeed);
        }
        eulerAngles.x = Mathf.Clamp(eulerAngles.x, currentMinVert, currentMaxVert);

        m_attachedCamera.transform.localEulerAngles = eulerAngles;
        m_transform.eulerAngles = go_eulerAngles;
    }

}
