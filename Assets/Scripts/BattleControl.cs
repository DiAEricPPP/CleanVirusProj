using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleControl : MonoBehaviour
{
    private static BattleControl battleControl;


    public static BattleControl Instance
    {
        get
        {
            if (battleControl == null)
            {
                battleControl = (BattleControl)FindObjectOfType(typeof(BattleControl));
            }
            if (battleControl == null)
            {
                GameObject go = new GameObject();
                battleControl = go.AddComponent<BattleControl>();
            }
            return battleControl;
        }
    }
    public List<string> enemyNames;
    /* public GameObject endPanel;
    public Text stageOverText;
    public Text[] enemyDieCountText;
    //public GameObject endPanelBackButton;
    public Text stageUseTime;
    public Slider shipLeftHP;
    public Button nextStageButton;
    public LoadControl loadControl; */
    private Dictionary<string, int> enemyDieCount = new Dictionary<string, int>();
    public Dictionary<string, int> EnemyDieCount
    {
        get
        {
            return enemyDieCount;
        }
    }

    private int score;
    public BattleBase shipBase;
    public InGameMenuControl endMenu;
    //private float shipHP;

    void Start()
    {
        score = 0;
        /* if (gameObject.tag == "Ship")
        {
            shipBase = GetComponent<BattleBase>();
        }
        else
        {
            shipBase = GameObject.FindGameObjectWithTag("Ship").GetComponent<BattleBase>();
        }*/
        //shipBase.SetHP(100);
        foreach (string s in enemyNames)
        {
            enemyDieCount.Add(s, 0);
        }
    }

    void Update()
    {
        if (shipBase.HP <= 0)
        {
            StageEnd(false);
        }
    }
    public void ChangeScore(int score)
    {
        this.score += score;
    }

    public void DamageTo(GameObject go, float damage)
    {
        BattleBase bb = go.GetComponent<BattleBase>();
        if (bb != null)
        {
            bb.TakeDamage(damage);
        }
    }

    public void EnemyDie(string enemyObjectName)
    {
        string name = enemyObjectName.Split()[0];
        if (enemyNames.Contains(name))
        {
            ++enemyDieCount[name];
        }
        /* else
        {
            enemyDieCount.Add(name, 1);
        } */
    }

    /* public IEnumerator StageEnd(bool clear)
    {
        yield return StartCoroutine(loadControl.FadeOut());
        Time.timeScale = 0;
        if (clear)
        {
            stageOverText.text = "Stage Clear!!!";
            nextStageButton.GetComponentInChildren<Text>().text = "下一关";
            if (PlayerPrefs.GetString("Save") == "Stage3")
            {
                nextStageButton.interactable = false;
            }
            string currentStage = PlayerPrefs.GetString("Save");
            //int.TryParse(currentStage.Substring(5),int num);
        }
        else
        {
            stageOverText.text = "oops Failed...";
            nextStageButton.GetComponentInChildren<Text>().text = "再试一次";
        }
    } */

    public void StageEnd(bool clear)
    {
        StartCoroutine(endMenu.StageEnd(clear));
    }
}
