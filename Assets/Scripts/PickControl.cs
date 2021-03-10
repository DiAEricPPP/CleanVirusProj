using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PickControl : MonoBehaviour
{
    public GameObject[] character;
    public LoadControl loadScreen;
    private GameObject currentSelected;
    private bool waitForNewPlayer = false;
    private StandaloneInputModule eventSystemInputModule;
    private bool isLoading = false;
    public struct PlayerController
    {
        public string horizontal;
        public string vertical;
        public string submit;
        public string cancel;
        public ColorBlock colorBlock;
    };
    public static PlayerController player1, player2, player3, player4;
    //player234仅代表玩家设备编号，不代表选人顺序

    string[] axisName = { "Submit2", "Submit3", "Submit4", "Submit" };

    //此字典记录对应操作方式，即player，是否选过人
    public Dictionary<string, bool> havePlayerPicked = new Dictionary<string, bool>();
    public AudioClip pickAudio;

    void Awake()
    {
        eventSystemInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
        /* PlayerPrefs.SetInt("Scientist", 0);
        PlayerPrefs.SetInt("Nurse", 0);
        PlayerPrefs.SetInt("Military", 0);
        PlayerPrefs.SetInt("Civilian", 0); */

        MatchPlayerToController();
        havePlayerPicked.Add("Submit", false);
        havePlayerPicked.Add("Submit2", false);
        havePlayerPicked.Add("Submit3", false);
        havePlayerPicked.Add("Submit4", false);
        //ChangeControlToPlayer(player1);
    }

    void Start()
    {
        ChangeControlToPlayer(player1);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            //GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
        CheckNewPlayerInput();
        /* if (Input.GetKeyDown(KeyCode.Joystick1Button7) && !isLoading)
        {
            isLoading = true;
            StartCoroutine(loadScreen.LoadScene(PlayerPrefs.GetString("Save")));
        } */
    }


    public void PlayerPick()
    {
        AudioSource.PlayClipAtPoint(pickAudio, Vector3.zero, 0.5f);
        GameObject pickedCharacter = EventSystem.current.currentSelectedGameObject;
        string name = pickedCharacter.name;
        foreach (GameObject o in character)
        {
            if (o.name == name)
            {
                o.GetComponent<Button>().interactable = false;
                string currentController = eventSystemInputModule.submitButton;
                havePlayerPicked[currentController] = true;

                PlayerPrefs.SetInt(o.name, 1);
                PlayerPrefs.SetString(o.name + "Control", currentController);

                waitForNewPlayer = true;
                //Debug.Log(waitForNewPlayer);
            }
        }
    }



    void ChangeControlToPlayer(PlayerController player)
    {
        waitForNewPlayer = false;
        eventSystemInputModule.horizontalAxis = player.horizontal;
        eventSystemInputModule.verticalAxis = player.vertical;
        eventSystemInputModule.submitButton = player.submit;
        eventSystemInputModule.cancelButton = player.cancel;

        foreach (GameObject o in character)
        {
            Button button = o.GetComponent<Button>();
            if (button.interactable == true)
            {
                button.colors = player.colorBlock;
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(o);
                }
            }
        }
    }

    void CheckNewPlayerInput()
    {
        if (waitForNewPlayer)
        {
            if (Input.GetAxis("Submit2") > 0 && !havePlayerPicked["Submit2"])
            {
                ChangeControlToPlayer(player2);
                return;
            }
            if (Input.GetAxis("Submit3") > 0 && !!havePlayerPicked["Submit3"])
            {
                ChangeControlToPlayer(player3);
                return;
            }
            if (Input.GetAxis("Submit4") > 0 && !!havePlayerPicked["Submit4"])
            {
                ChangeControlToPlayer(player4);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button7) && !isLoading)
            {
                isLoading = true;
                //Debug.Log("Suc");
                StartCoroutine(loadScreen.LoadScene(PlayerPrefs.GetString("Save")));
            }
        }
    }
    void MatchPlayerToController()
    {
        player1.horizontal = "Horizontal";
        player1.vertical = "Vertical";
        player1.submit = "Submit";
        player1.cancel = "Cancel";
        player1.colorBlock = ColorBlock.defaultColorBlock;
        player1.colorBlock.selectedColor = Color.green;
        /* player1.colorBlock.highlightedColor=Color.white;
        player1.colorBlock.disabledColor=Color.grey;
        player1.colorBlock.pressedColor=Color.grey;
        player1.colorBlock.normalColor=Color.white; */

        player2.horizontal = "Horizontal2";
        player2.vertical = "Vertical2";
        player2.submit = "Submit2";
        player2.cancel = "Cancel2";
        player2.colorBlock = ColorBlock.defaultColorBlock;
        player2.colorBlock.selectedColor = Color.red;

        player3.horizontal = "Horizontal3";
        player3.vertical = "Vertical3";
        player3.submit = "Submit3";
        player3.cancel = "Cancel3";
        player3.colorBlock = ColorBlock.defaultColorBlock;
        player3.colorBlock.selectedColor = Color.cyan;


        player4.horizontal = "Horizontal4";
        player4.vertical = "Vertical4";
        player4.submit = "Submit4";
        player4.cancel = "Cancel4";
        player4.colorBlock = ColorBlock.defaultColorBlock;
        player4.colorBlock.selectedColor = Color.yellow;
    }

}
