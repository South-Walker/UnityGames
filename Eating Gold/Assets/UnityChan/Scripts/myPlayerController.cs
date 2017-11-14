using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        float direction = Input.GetAxis("Horizontal");
        //查查看看
        animator.SetFloat("Direction", direction,0.25f, Time.deltaTime);
	}
}
