using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject enemy2;
    //public GameObject enemy3;
    /* private ContactFilter2D cf;
    private Collider2D co;
    private Collider2D[] results = new Collider2D[1];
    //public ParticleSystem particleSystem;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(); */
    private BattleBase bb;
    private int testInt = 0;
    public Text tt;
    string a = "a";
    private enum DPad { Up, Down, Left, Right, NULL };
    private DPad DPadPressed;
    Dictionary<int, string> crossKey = new Dictionary<int, string>();
    void Start()
    {
        /* co = GetComponent<Collider2D>();
        cf.layerMask = LayerMask.GetMask("Alley"); */
        tt.text = "↑↑→↓←";
        bb = GetComponent<BattleBase>();
        crossKey.Add(1, "↑");
        crossKey.Add(2, "↓");
        crossKey.Add(3, "←");
        crossKey.Add(4, "→");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            //enemy2.SetActive(true);
            //enemy3.SetActive(true);
            /* Cal(testInt);
            Debug.Log(testInt); */
            /* string b="b";
            a=string.Concat(a,b);
            Debug.Log(a); */
            string cross = RandomQTEString();
            tt.text = cross;
        }
        /* if (Input.GetAxis("CrossX1") == -1)
        {
            Debug.Log("L");
        }
        if (Input.GetAxis("CrossX1") == 1)
        {
            Debug.Log("R");
        }
        if (Input.GetAxis("CrossY1") == 1)
        {
            Debug.Log("U");
        }
        if (Input.GetAxis("CrossY1") == -1)
        {
            Debug.Log("D");
        }
        if (Input.GetAxis("CrossY1") == 0)
        {
            Debug.Log("Y0");
        }
        if (Input.GetAxis("CrossX1") == 0)
        {
            Debug.Log("X0");
        } */
        //Debug.Log(Input.GetAxisRaw("CrossX1") + " " + Input.GetAxisRaw("CrossY1"));
        //Debug.Log(bb.HP);
        if (GetDPadButtonDown(DPad.Up))
        {
            Debug.Log("U");
        }
        if (GetDPadButtonDown(DPad.Down))
        {
            Debug.Log("D");
        }
        if (GetDPadButtonDown(DPad.Left))
        {
            Debug.Log("L");
        }
        if (GetDPadButtonDown(DPad.Right))
        {
            Debug.Log("R");
        }
    }
    bool GetDPadButtonDown(DPad button)
    {//在Update调用，同官方方法
        switch (button)
        {
            case DPad.Up:
                if (DPadPressed != DPad.Up && Input.GetAxisRaw("CrossY1") == 1)
                {
                    DPadPressed = DPad.Up;
                    return true;
                }
                break;
            case DPad.Down:
                if (DPadPressed != DPad.Down && Input.GetAxisRaw("CrossY1") == -1)
                {
                    DPadPressed = DPad.Down;
                    return true;
                }
                break;
            case DPad.Left:
                if (DPadPressed != DPad.Left && Input.GetAxisRaw("CrossX1") == -1)
                {
                    DPadPressed = DPad.Left;
                    return true;
                }
                break;
            case DPad.Right:
                if (DPadPressed != DPad.Right && Input.GetAxisRaw("CrossX1") == 1)
                {
                    DPadPressed = DPad.Right;
                    return true;
                }
                break;
        }
        if (Input.GetAxisRaw("CrossX1") == 0 && Input.GetAxisRaw("CrossY1") == 0)
        {
            DPadPressed = DPad.NULL;
        }
        return false;
    }
    private string RandomQTEString()
    {
        string result = null, part = null;
        for (int i = 0; i < 4; i++)
        {
            int ran = UnityEngine.Random.Range(1, 4);
            part = crossKey[ran];
            result = string.Concat(result, part);
        }
        return result;
    }
    void Cal(int toTest)
    {
        ++toTest;
        //testInt = toTest;
    }
    /* private void OnParticleCollision(GameObject other)
    {
        other.GetComponent<ParticleSystem>().GetCollisionEvents(gameObject, collisionEvents);
        //other.GetComponent<ParticleSystem>().
        Debug.Log("Hit");
        //ParticlePhysicsExtensions.GetCollisionEvents()
    } */


    /* void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "E_S")
        {
            // SendMessage("boom_e_s");
            GameObject.Find("螺旋菌").SendMessageUpwards("boom_e_s");

        }
        Destroy(gameObject);
        //Instantiate(boom, transform.position, Quaternion.identity);
    } */
}
