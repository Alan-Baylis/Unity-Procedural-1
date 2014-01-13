using UnityEngine;
using System;
using System.Collections.Generic;

public class InputSystem : MonoBehaviour
{

    #region InputAPI
    private bool m_isUp;

    private bool m_isDown;

    private bool m_isLeft;

    private bool m_isRight;

    private bool m_isForward;

    private bool m_isBack;

    private bool m_isButton1;

    private bool m_isButton2;

    private bool m_isButton3;

    private Dictionary<string, float> m_axis;

    private Vector3 m_cursorLoc;

    public bool UP
    {
        get
        {
            return m_isUp;
        }
    }

    public bool DOWN
    {
        get
        {
            return m_isDown;
        }
    }

    public bool LEFT
    {
        get
        {
            return m_isLeft;
        }
    }

    public bool RIGHT
    {
        get
        {
            return m_isRight;
        }
    }

    public bool FORWARD
    {
        get
        {
            return m_isForward;
        }
    }

    public bool BACK
    {
        get
        {
            return m_isBack;
        }
    }

    public bool BUTTON_1
    {
        get
        {
            return m_isButton1;
        }
    }

    public bool BUTTON_2
    {
        get
        {
            return m_isButton2;
        }
    }

    public bool BUTTON_3
    {
        get
        {
            return m_isButton3;
        }
    }

    public Vector3 CURSOR_LOCATION
    {
        get
        {
            return m_cursorLoc;
        }
    }

    public float GetAxis(AxisIndex type)
    {
        if (!m_axis.ContainsKey(type.ToString()))
        {
            Debug.LogError(string.Format("No Axis {0} found!", type));
            return 0;
        }

        return m_axis[type.ToString()];
    }

    public enum AxisIndex
    {
        HORIZONTAL,
        VERTICAL,
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN
    }
    #endregion

    #region input options
    public enum InputSystemOption
    {
        KEYBOARD,
        TOUCH,
        GAMEPAD
    }

    public InputSystemOption m_activeInputOption = InputSystemOption.KEYBOARD;
    public delegate void UpdateInputMethod();
    public UpdateInputMethod UpdateInputs;

    #endregion

    #region KeyboardMappings
    public KeyCode m_keyboardUp;
    public KeyCode m_keyboardDown;
    public KeyCode m_keyboardLeft;
    public KeyCode m_keyboardRight;
    public KeyCode m_keyboardForward;
    public KeyCode m_keyboardBack;

    public KeyCode m_mouse1;
    public KeyCode m_mouse2;
    public KeyCode m_mouse3;

    public List<KeyCode> m_keyboardAxis;
    public List<AxisIndex> m_mouseAxis;
    #endregion

    #region GamepadMappings
    #endregion

    #region TouchMappings
    #endregion


    void Awake()
    {
        Init(m_activeInputOption);
    }

    // Use this for initialization
	void Start () 
    {   
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateInputs();
	}
    
    private void Init(InputSystemOption activeInputs)
    {
        m_axis = new Dictionary<string,float>();
        switch (m_activeInputOption)
        {
            case InputSystemOption.KEYBOARD:
                m_cursorLoc = new Vector3();
                foreach (KeyCode axisKey in m_keyboardAxis)
                {
                    m_axis.Add(axisKey.ToString(), 0);
                }

                foreach (AxisIndex mouseAxis in m_mouseAxis)
                {
                    m_axis.Add(mouseAxis.ToString(), 0);
                }

                UpdateInputs = UpdateKeyboard;
                break;

            case InputSystemOption.GAMEPAD:
                UpdateInputs = UpdateGamepad;
                break;

            case InputSystemOption.TOUCH:
                UpdateInputs = UpdateTouch;
                break;
        }
    }

    public string GetStringKey(InputSystemOption iso, KeyCode key)
    {
        return GetStringKey(iso, key.ToString());
    }

    public string GetStringKey(InputSystemOption iso, AxisIndex key)
    {
        return GetStringKey(iso, key.ToString());
    }

    public string GetStringKey(InputSystemOption iso, string key)
    {
        return string.Format("{0}_{1}", iso, key);
    }

    #region update Methods
    private void UpdateKeyboard()
    {
        m_isUp = Input.GetKey(m_keyboardUp);
        m_isDown = Input.GetKey(m_keyboardDown);
        m_isForward = Input.GetKey(m_keyboardForward);
        m_isBack = Input.GetKey(m_keyboardBack);
        m_isLeft = Input.GetKey(m_keyboardLeft);
        m_isRight = Input.GetKey(m_keyboardRight);

        m_isButton1 = Input.GetKey(m_mouse1);
        m_isButton2 = Input.GetKey(m_mouse2);
        m_isButton3 = Input.GetKey(m_mouse3);

        m_cursorLoc = Input.mousePosition;

        foreach (KeyCode axis in m_keyboardAxis)
        {
            m_axis[axis.ToString()] = (Input.GetKey(axis) ? 1 : 0);
        }

        foreach (AxisIndex mouseAxis in m_mouseAxis)
        {
            try
            {
                m_axis[mouseAxis.ToString()] = Input.GetAxis(GetStringKey(m_activeInputOption, mouseAxis));
            }
            catch
            {
                Debug.LogError(string.Format("No Axis '{0}' found! Please ensure that there is an input axis set up called '{0}' within Edit->Project Settings->Input", GetStringKey(m_activeInputOption, mouseAxis)));
            }
        }
    }

    private void UpdateGamepad()
    {
    }

    private void UpdateTouch()
    {
    }
    #endregion

}
