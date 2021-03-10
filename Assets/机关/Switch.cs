using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    public GameObject electric1;
    public GameObject electric2;
    public Collider2D col;
    public GameObject pb;

    private bool hitMessage;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        col = GetComponentInChildren<Collider2D>();
        hitMessage = false;
   //     hitMessage = pb.GetComponent<PrimaryBullet>().isHitMechanism;
    }

    // Update is called once per frame
    void Update()
    {
   //     hitMessage = pb.GetComponent<PrimaryBullet>().isHitMechanism;
   //     print(hitMessage);
        if (Input.GetKey(KeyCode.K) || hitMessage == true)
        {
            anim.SetBool("isTurn", true);
            electric1.SetActive(false);
            electric2.SetActive(false);
        }
        else
        {
            anim.SetBool("isTurn", false);
            electric1.SetActive(true);
            electric2.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Bullet")
        {
            hitMessage = true;
            print("!!!!!!!!!!!!!");
        }
    }


}
