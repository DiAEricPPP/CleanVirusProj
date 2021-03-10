using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenuControl : MonoBehaviour
{
    public GameObject startNewGameButton;
    public GameObject continueLastGameButton;
    public GameObject settingButton;
    public GameObject levelPanel;
    public GameObject[] levelPanelButton;
    public GameObject settingPanel;
    public GameObject firstSettingPanelButton;
    public GameObject aboutButton;
    public GameObject aboutPanel;
    private List<string> stage = new List<string> { "Stage1", "Stage2", "Stage3" };
    private const string CannonLevel = "CannonLevel";
    private const string SpecCannonLevel = "SpecCannonLevel";
    private GameObject currentSelected = null;//获取现时选择物体，屏蔽鼠标点击
    private GameObject currentPanel = null;
    private GameObject lastSelected = null;
    public AudioClip buttonPressed;
    public AudioClip returnLastSelected;
    public AudioClip enterGameAudio;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("Doctor", 0);
        PlayerPrefs.SetInt("Scientist", 0);
        PlayerPrefs.SetInt("Nurse", 0);
        PlayerPrefs.SetInt("Civilian", 0);

        //存储stage解锁与否，控制按钮显示
        foreach (string s in stage)
        {
            if (!PlayerPrefs.HasKey(s))
            {
                PlayerPrefs.SetInt(s, 0);
            }
        }
        PlayerPrefs.SetInt(stage[0], 1);
        PlayerPrefs.SetInt(stage[1], 1);
        //PlayerPrefs.SetInt(stage[2], 1);
        foreach (GameObject o in levelPanelButton)
        {
            if (PlayerPrefs.GetInt(stage[Array.IndexOf(levelPanelButton, o)]) != 1)
            {
                o.GetComponent<Button>().interactable = false;
                //o.SetActive(false);
            }
        }
        if (!PlayerPrefs.HasKey("Save"))
        {
            PlayerPrefs.SetString("Save", null);//用来保存当前游玩的关卡
        }
        if (PlayerPrefs.GetString("Save") != "")
        {
            string save = PlayerPrefs.GetString("Save");
            continueLastGameButton.transform.Find("Text").GetComponent<Text>().text = "从关卡" + save.Substring(5) + "继续";
            continueLastGameButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            continueLastGameButton.transform.Find("Text").GetComponent<Text>().text = "无游玩记录";
            continueLastGameButton.GetComponent<Button>().interactable = false;
        }
        levelPanel.SetActive(false);
        settingPanel.SetActive(false);
        aboutPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(startNewGameButton);
        //currentSelected = startNewGameButton;
        AudioSource.PlayClipAtPoint(enterGameAudio, Vector3.zero, 0.5f);
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
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            CloseCurrentPanel();
        }
        /* if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.SetString("Save", null);
            continueLastGameButton.transform.Find("Text").GetComponent<Text>().text = "无游玩记录";
            continueLastGameButton.GetComponent<Button>().interactable = false;

            PlayerPrefs.SetInt(stage[0], 1);
            PlayerPrefs.SetInt(stage[1], 0);
            PlayerPrefs.SetInt(stage[2], 0);
            foreach (GameObject o in levelPanelButton)
            {
                if (PlayerPrefs.GetInt(stage[Array.IndexOf(levelPanelButton, o)]) != 1)
                {
                    o.GetComponent<Button>().interactable = false;
                    //o.SetActive(false);
                }
            }
            PlayerPrefs.SetInt(CannonLevel, 1);
            PlayerPrefs.SetInt(SpecCannonLevel, 1);
        } */
    }

    public void ClearSave()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        PlayerPrefs.SetString("Save", null);
        continueLastGameButton.transform.Find("Text").GetComponent<Text>().text = "无游玩记录";
        continueLastGameButton.GetComponent<Button>().interactable = false;

        PlayerPrefs.SetInt(stage[0], 1);
        PlayerPrefs.SetInt(stage[1], 1);
        PlayerPrefs.SetInt(stage[2], 0);
        foreach (GameObject o in levelPanelButton)
        {
            if (PlayerPrefs.GetInt(stage[Array.IndexOf(levelPanelButton, o)]) != 1)
            {
                o.GetComponent<Button>().interactable = false;
                //o.SetActive(false);
            }
        }
        PlayerPrefs.SetInt(CannonLevel, 1);
        PlayerPrefs.SetInt(SpecCannonLevel, 1);
    }
    public void OnStartNewGame()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        levelPanel.SetActive(true);
        currentPanel = levelPanel;
        lastSelected = startNewGameButton;
        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(levelPanelButton[0]);
        //currentSelected = levelPanelButton[0];
        foreach (GameObject o in levelPanelButton)
        {
            if (Array.IndexOf(levelPanelButton, o) >= 1)
            {
                int last = Array.IndexOf(levelPanelButton, o) - 1;
                Navigation lastNavi = levelPanelButton[last].GetComponent<Button>().navigation;
                if (!o.GetComponent<Button>().interactable)
                {
                    lastNavi.selectOnRight = null;
                    levelPanelButton[last].GetComponent<Button>().navigation = lastNavi;
                }
                else
                {
                    lastNavi.selectOnRight = o.GetComponent<Button>();
                    levelPanelButton[last].GetComponent<Button>().navigation = lastNavi;
                }
            }
        }
    }

    public void OnSelectStage()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        string selectedStage = stage[Array.IndexOf(levelPanelButton, EventSystem.current.currentSelectedGameObject)];
        if (PlayerPrefs.HasKey(selectedStage))
        {
            PlayerPrefs.SetString("Save", selectedStage);
            SceneManager.LoadSceneAsync("Pick");
        }
    }

    public void OnContinueLastGame()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        //string save = PlayerPrefs.GetString("Save");
        SceneManager.LoadSceneAsync("Pick");
    }

    public void OnSetting()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        //Debug.Log("Setting");
        settingPanel.SetActive(true);
        currentPanel = settingPanel;
        lastSelected = settingButton;
        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSettingPanelButton);
        //currentSelected = firstSettingPanelButton;
    }

    public void OnAbout()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        aboutPanel.SetActive(true);
        currentPanel = aboutPanel;
        lastSelected = aboutButton;
        //Debug.Log("AboutOurGroup");
    }

    public void OnExitGame()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        Application.Quit();
    }

    void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            AudioSource.PlayClipAtPoint(returnLastSelected, Vector3.zero, 0.5f);
            currentPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(lastSelected);
            //currentSelected = lastSelected;
        }
    }
}
