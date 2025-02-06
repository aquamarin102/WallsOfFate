using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Transform transform;
    Vector3 position;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();
        position = transform.position;
    }

    void Update()
    {
        //Debug.Log(transform.velocity);
        if (transform.position != position)
        {
            //Debug.Log("change animation");
            animator.SetBool("IsWalk", true);
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }
        position = transform.position;
    }
}
