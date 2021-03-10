using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadControl : MonoBehaviour
{
    public Slider loadSlider;
    public Text loadText;
    public Image mask;
    [SerializeField] private float alpha = 0;
    /* private bool freezeScene = true;
    public bool sceneFreezed
    {
        get
        {
            return freezeScene;
        }
    } */
    private bool fadingIn = true;//此bool解决fadein fadeout两个协程共同发生时的冲突（在选人时或许因为按键过快导致卡住必须返回）
    // Start is called before the first frame update
    void Start()
    {
        //mask.color = new Color(0.77f, 0.77f, 0.77f, 0);
        loadSlider.gameObject.SetActive(false);
        loadText.gameObject.SetActive(false);
        
        StartCoroutine(FadeIn(mask,alpha));//每次场景载入一开始都会先fadein 渐入
    }

    public IEnumerator LoadScene(string scene)
    {
        //Debug.Log("Load Suc");
        yield return StartCoroutine(FadeOut(mask,alpha));

        loadSlider.gameObject.SetActive(true);
        loadText.gameObject.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadSlider.value = operation.progress;
            loadText.text = operation.progress * 100 + "%";

            if (operation.progress >= 0.9f)
            {
                loadSlider.value = 1;
                loadText.text = "玩家1按A继续";

                if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }


    public IEnumerator FadeOut(Image mask,float alpha)
    {//先判断是否正在fadein 渐入，防止冲突死循环
        yield return StartCoroutine(new WaitWhile(() => fadingIn == true));
        alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime;
            mask.color = new Color(0.1f, 0.1f, 0.1f, alpha);
            yield return null;
        }
        //yield return null;
    }

    public IEnumerator FadeIn(Image mask,float alpha)
    {
        fadingIn = true;
        alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            mask.color = new Color(0.1f, 0.1f, 0.1f, alpha);
            yield return null;
        }
        fadingIn = false;//fadein结束后更改此bool，方便判断fadeout可否继续执行
    }
}
