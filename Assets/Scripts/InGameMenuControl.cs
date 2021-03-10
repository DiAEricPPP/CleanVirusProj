using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenuControl : MonoBehaviour
{
    public GameObject pauseMenu, resumeButton, settingButton, backToMainMenuButton, quitButton;
    public GameObject settingPanel;
    public GameObject[] settingPanelButton;
    [Range(0.1f, 1f)]
    public float resumeGap;
    public GameObject endPanel;
    public Image stageEndMask;
    private float stageEndMaskAlpha = 0;
    public Text stageOverText;
    public List<Text> enemyDieCountText;
    //public GameObject endPanelBackButton;
    public Text stageUseTime;
    public Slider shipLeftHP;
    public Button nextStageButton;
    public LoadControl loadControl;
    public AudioClip buttonPressed;
    public AudioClip returnLastSelected;
    public AudioClip pauseAudio;
    //public GameObject losePanel;
    private GameObject currentSelected, lastSelected, currentPanel;
    private bool canPauseAgain = true;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        settingPanel.SetActive(false);
        endPanel.SetActive(false);
    }

    void LateUpdate()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
    }
    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            OnPauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            CloseCurrentPanel();
        }
        /* if(Input.GetKeyDown(KeyCode.R)){
            StartCoroutine(loadControl.LoadScene(PlayerPrefs.GetString("Save")));
        } */
    }

    void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            AudioSource.PlayClipAtPoint(returnLastSelected, Vector3.zero, 1f);
            currentPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
    }

    void OnPauseGame()
    {
        if (!pauseMenu.activeInHierarchy && canPauseAgain == true)
        {
            AudioSource.PlayClipAtPoint(pauseAudio, Vector3.zero, 1f);
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton);
            Time.timeScale = 0;
        }
    }
    public void OnSetting()
    {
        //Debug.Log("Setting");
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 1f);
        settingPanel.SetActive(true);
        currentPanel = settingPanel;
        lastSelected = settingButton;
        //EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingPanelButton[0]);
        //currentSelected = firstSettingPanelButton;
    }

    IEnumerator Resume()
    {
        canPauseAgain = false;
        yield return new WaitForSecondsRealtime(resumeGap);
        Time.timeScale = 1;
        canPauseAgain = true;
    }
    public void OnResumeGame()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 1f);
        pauseMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        settingPanel.SetActive(false);
        StartCoroutine(Resume());
        //Time.timeScale = 1;
    }

    public void OnBackToMainMenu()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 1f);
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OnQuit()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 1f);
        Time.timeScale = 1;
        Application.Quit();
    }

    //private 

    public IEnumerator StageEnd(bool clear)
    {
        yield return StartCoroutine(loadControl.FadeOut(stageEndMask, stageEndMaskAlpha));
        Time.timeScale = 0;
        //先初始化结算界面的输赢不同部分的功能，按钮功能/文字表示等
        if (clear)
        {
            stageOverText.text = "Stage Clear!!!";
            nextStageButton.GetComponentInChildren<Text>().text = "下一关";
            if (PlayerPrefs.GetString("Save") == "Stage2")
            {
                nextStageButton.interactable = false;
            }
            else
            {
                nextStageButton.interactable = true;
                string currentStageNum = PlayerPrefs.GetString("Save").Substring(5);//获取当前储存进度的关卡编号，即当前关卡编号
                int.TryParse(currentStageNum, out int num);
                int nextStageNum = ++num;
                string nextStage = "Stage" + nextStageNum.ToString();
                PlayerPrefs.SetString("Save", nextStage);
                Debug.Log("Save Next");
            }
        }
        else
        {
            stageOverText.text = "oops Failed...";
            nextStageButton.GetComponentInChildren<Text>().text = "再试一次";
        }
        //然后初始化共通数据显示
        shipLeftHP.value = BattleControl.Instance.shipBase.HP;
        int seconds = (int)Time.timeSinceLevelLoad;
        stageUseTime.text = "时间 : " + FormatTime(seconds);
        foreach (Text t in enemyDieCountText)
        {
            int index = enemyDieCountText.IndexOf(t);
            string enemyName = BattleControl.Instance.enemyNames[index];
            t.text = enemyName + " * " + BattleControl.Instance.EnemyDieCount[enemyName];
        }
        endPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(nextStageButton.gameObject);
    }
    private string FormatTime(int timeAsSeconds)
    {
        int hours = timeAsSeconds / 3600;
        //string hh = hours < 10 ? "0" + hours : hours.ToString();
        int minutes = (timeAsSeconds - hours * 3600) / 60;
        //string mm = minutes < 10 ? "0" + minutes : minutes.ToString();
        int seconds = timeAsSeconds - hours * 3600 - minutes * 60;
        //string ss = seconds < 10 ? "0" + seconds : seconds.ToString();

        //return string.Format("{0}:{1}:{2}", hh, mm, ss);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
    public void OnNextStageButtonClick()
    {
        AudioSource.PlayClipAtPoint(buttonPressed, Vector3.zero, 0.5f);
        Time.timeScale = 1;
        StartCoroutine(loadControl.LoadScene(PlayerPrefs.GetString("Save")));
    }
}
