using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    public float Speed
    {
        get
        {
            return animator.GetFloat("Speed");
        }
        set
        {
            animator.SetFloat("Speed", value);
        }
    }
    public float SmoothSetSpeed
    {
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
    public float Rotation = 0;
    private float minMoveSpeed = 0.1f;
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
            this.transform.rotation = Quaternion.Euler(0, Rotation, 0);
            this.transform.position = GetNextPosition(Speed, Direction, Mathf.PI / 4f, Mathf.PI / 4f * 3f);
        }
    }
    private void LateUpdate()
    {
        float newdirection = Input.GetAxis("Horizontal");
        Direction = newdirection;
        SmoothSetSpeed = Mathf.Abs(Direction) + 2 * Mathf.Abs(Input.GetAxis("Vertical"));
        if (Speed <= minMoveSpeed)
        {
            Speed = 0;
        }
        else if (Mathf.Abs(newdirection) >= Mathf.Abs(Direction) || newdirection * Direction < 0)
        {
            Rotation += newdirection;
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
        Vector3 front = this.transform.forward;
        Vector3 right = Vector3.Cross(new Vector3(0, 1, 0), front);
        //这个是相对于物体坐标系的
        Vector3 nextstep = nextz * front + nextx * right;
        return nextstep;
    }
}
