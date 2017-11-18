using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    private float Speed
    {
        get
        {
            return animator.GetFloat("Speed");
        }
        set
        {
            animator.SetFloat("Speed", value, 0.25f, Time.deltaTime);
        }
    }
    private float Direction
    {
        get
        {
            return animator.GetFloat("Direction");
        }
        set
        {
            animator.SetFloat("Direction", value);
        }
    }
    public bool isbigger;
    public bool isdifferentside;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Speed == 0)
        {
            animator.SetBool("isTired", animator.GetBool("hasRun") && true);
        }
        else
        {
            animator.SetBool("hasRun", true);
            animator.SetBool("isTired", false);
            Vector3 nextposition = GetNextPosition(Speed, Direction);
            this.transform.forward = GetForward(nextposition);
            this.transform.position = nextposition;
        }
    }
    private void LateUpdate()
    {
        Direction = Input.GetAxis("Horizontal");
        Speed = Mathf.Abs(Direction) + Input.GetAxis("Vertical");
        if (animator.GetFloat("Speed") <= 0.1f)
        {
            animator.SetFloat("Speed", 0);
        }
    }

    public Vector3 GetNextPosition(float speed, float direction, float beginrad = -1 / 4f * Mathf.PI, float endrad = 5 / 4f * Mathf.PI)
    {
        speed = 0.02f * speed;
        float myDirection = 1f - direction;
        float rad = ((endrad - beginrad) * myDirection / 2f + beginrad);
        

        Vector3 nextstep = GetNextStep(speed, rad);
        return this.transform.position + nextstep;
    }
    private Vector3 GetNextStep(float _speed, float rad)
    {
        float nextx = _speed * Mathf.Cos(rad);
        float nextz = _speed * Mathf.Sin(rad);
        //这个是相对于物体坐标系的
        Vector3 nextstep = new Vector3(nextx, 0, nextz);
        Vector3 smallz = this.transform.forward;
        //y==0
        Vector3 smallx = new Vector3(smallz.z, 0, smallz.x * -1f);

        nextstep = Vector3.Dot(smallz, nextstep) * smallz + Vector3.Dot(smallx, nextstep) * smallx;
        


        return nextstep;
    }
    private Vector3 GetForward(Vector3 nextposition)
    {
        Vector3 nowforward = (nextposition - this.transform.position).normalized;
        return nowforward;

        Vector3 beforeforward = this.transform.forward;
        Vector3 blueaxis = new Vector3(0, 0, 1);
        bool isdifferentside = (Vector3.Cross(nowforward, blueaxis) != Vector3.Cross(beforeforward, blueaxis)) ? true : false;
        bool isbiggerinrad = (Vector3.Dot(nowforward, blueaxis) <= Vector3.Dot(beforeforward, blueaxis)) ? true : false;

        isbigger = isbiggerinrad;
        this.isdifferentside = isdifferentside;

        if (isdifferentside || isbiggerinrad) 
        {
            return nowforward;
        }
        return beforeforward;
    }
}
