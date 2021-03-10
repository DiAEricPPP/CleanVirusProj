using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GameControl : MonoBehaviour
{
    public GameObject[] character;
    public GameObject ship;
    //private Vector2 lastFrameShipPosition;
    private float graphUpdateTime = 0f;


    //private int num = 0;
    class ControlData
    {
        public int enabled = 0;
        public string control = null;

    }
    Dictionary<GameObject, ControlData> controlMap = new Dictionary<GameObject, ControlData>();
    // Start is called before the first frame update
    void Start()
    {
        //TODO:后续内容游戏的初始化
        //lastFrameShipPosition = ship.GetComponent<Rigidbody2D>().position;
        //AstarPath.active.Scan();
        controlMap.Add(character[0], new ControlData());
        controlMap.Add(character[1], new ControlData());
        controlMap.Add(character[2], new ControlData());
        controlMap.Add(character[3], new ControlData());

        foreach (GameObject o in character)
        {
            controlMap[o].enabled = PlayerPrefs.GetInt(o.name);
            if (controlMap[o].enabled == 1)
            {
                o.SetActive(true);
                controlMap[o].control = PlayerPrefs.GetString(o.name + "Control", null);
                BindCharacterToController(o, controlMap[o].control);
            }
            else
            {
                o.SetActive(false);
            }
        }

        foreach (GameObject o in character)
        {
            if (o != null)
            {
                PlayerPrefs.SetInt(o.name, 0);
                PlayerPrefs.SetString(o.name + "Control", null);
            }
        }
    }

    void LateUpdate()
    {
        graphUpdateTime += Time.deltaTime;
        if (graphUpdateTime >= AstarPath.active.graphUpdateBatchingInterval && ship.transform.hasChanged)
        {
            AstarPath.active.UpdateGraphs(ship.GetComponent<Collider2D>().bounds);
            graphUpdateTime = 0f;
        }
        //lastFrameShipPosition = ship.GetComponent<Rigidbody2D>().position;
        /* num++;
        Debug.Log("Now" + num); */
        //Debug.Log("Now" + ship.transform.position);
        //Debug.Log(Time.timeSinceLevelLoad);

    }

    void BindCharacterToController(GameObject character, string controlName)
    {//后续增删按键可以手动更改此处
        Player playerController = character.GetComponent<Player>();
        switch (controlName)
        {
            case "Submit":
                {
                    playerController.horizontalAxis = "Horizontal";
                    playerController.verticalAxis = "Vertical";
                    playerController.crossX = "CrossX1";
                    playerController.crossY = "CrossY1";
                    playerController.JoystickA = KeyCode.Joystick1Button0;
                    playerController.JoystickB = KeyCode.Joystick1Button1;
                    playerController.JoystickX = KeyCode.Joystick1Button2;
                    break;
                }

            case "Submit2":
                {
                    playerController.horizontalAxis = "Horizontal2";
                    playerController.verticalAxis = "Vertical2";
                    playerController.crossX = "CrossX2";
                    playerController.crossY = "CrossY2";
                    playerController.JoystickA = KeyCode.Joystick2Button0;
                    playerController.JoystickB = KeyCode.Joystick2Button1;
                    playerController.JoystickX = KeyCode.Joystick2Button2;
                    /* playerController.JoystickA = KeyCode.Space;
                    playerController.JoystickB = KeyCode.Q;
                    playerController.JoystickX = KeyCode.E; */
                    break;
                }

            case "Submit3":
                {
                    playerController.horizontalAxis = "Horizontal3";
                    playerController.verticalAxis = "Vertical3";
                    playerController.crossX = "CrossX3";
                    playerController.crossY = "CrossY3";
                    playerController.JoystickA = KeyCode.Joystick3Button0;
                    playerController.JoystickB = KeyCode.Joystick3Button1;
                    playerController.JoystickX = KeyCode.Joystick3Button2;
                    break;
                }

            case "Submit4":
                {
                    playerController.horizontalAxis = "Horizontal4";
                    playerController.verticalAxis = "Vertical4";
                    playerController.crossX = "CrossX4";
                    playerController.crossY = "CrossY4";
                    playerController.JoystickA = KeyCode.Joystick4Button0;
                    playerController.JoystickB = KeyCode.Joystick4Button1;
                    playerController.JoystickX = KeyCode.Joystick4Button2;
                    break;
                }

            default:
                break;
        }
    }



}
