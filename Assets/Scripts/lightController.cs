using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightController : MonoBehaviour
{
    [SerializeField] bool alwaysOn;
    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        if (alwaysOn)
        {
            animator.SetBool("LightOn", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !alwaysOn)
        {
            this.GetComponent<Animator>().SetBool("LightOn", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!alwaysOn)
        {
            this.GetComponent<Animator>().SetBool("LightOn", false);
        }
    }
}
