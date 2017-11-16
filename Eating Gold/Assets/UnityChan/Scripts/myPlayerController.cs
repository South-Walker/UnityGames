using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    public float watcherx;
    public float watchery;
    public float watcherz;
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
            Vector3 nextposition = GetNextPosition(speed, direction);
            this.transform.forward = (nextposition - this.transform.position).normalized;
            this.transform.position = nextposition;
        }
    }
    private void LateUpdate()
    {
        float direction = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(direction) + Input.GetAxis("Vertical");
        animator.SetFloat("Speed", speed, 0.25f, Time.deltaTime);
        animator.SetFloat("Direction", direction);
        if (animator.GetFloat("Speed") <= 0.1f)
        {
            animator.SetFloat("Speed", 0);
        }
    }
    
    public Vector3 GetNextPosition(float speed, float direction)
    {
        speed = 0.02f * speed;
        float myDirection = 1f - direction;
        float rad = (3f * Mathf.PI / (2f * 2f) * myDirection - 1 / 4f * Mathf.PI);

        watcherx = rad;

        Vector3 nextstep = GetNextStep(speed, rad);
        return this.transform.position + nextstep;
    }
    private Vector3 GetNextStep(float _speed, float rad)
    {
        float nextx = _speed * Mathf.Cos(rad);
        float nextz = _speed * Mathf.Sin(rad);
        Vector3 nextstep = new Vector3(nextx, 0, nextz);
        Vector3 smallz = this.transform.forward;
        //y==0
        Vector3 smallx = new Vector3(smallz.z * -1f, 0, smallz.x);

        nextstep = Vector3.Dot(smallz, nextstep) * smallz + Vector3.Dot(smallx, nextstep) * smallx;
        


        return nextstep;
    }
}
