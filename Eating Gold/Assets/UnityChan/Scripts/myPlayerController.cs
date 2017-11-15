using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    public float watcher;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        float speed = animator.GetFloat("Speed");
        float direction = animator.GetFloat("Direction");
        if (speed == 0)
        {
            animator.SetBool("isTired", animator.GetBool("hasRun") && true);
        }
        else
        {
            animator.SetBool("hasRun", true);
            animator.SetBool("isTired", false);
            this.transform.position = GetNextPosition(speed, direction);
        }
    }
    private void LateUpdate()
    {
        float direction = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(direction) + Input.GetAxis("Vertical");
        animator.SetFloat("Speed", speed, 0.25f, Time.deltaTime);
        animator.SetFloat("Direction", direction, 0.25f, Time.deltaTime);
        if (animator.GetFloat("Speed") <= 0.1f)
        {
            animator.SetFloat("Speed", 0);
        }
    }
    
    public Vector3 GetNextPosition(float speed, float direction)
    {
        speed = 0.02f * speed;
        Vector3 front = this.transform.forward;

        float myDirection = 1 - direction;
        float rad = (1 / 2.0f * Mathf.PI * myDirection / 2.0f + 1 / 4.0f * Mathf.PI);
        float nextx = speed * Mathf.Cos(rad);
        float nextz = speed * Mathf.Sin(rad);
        Vector3 nextposition = new Vector3(nextx, 0, nextz);
        return this.transform.position + nextposition;
    }
}
