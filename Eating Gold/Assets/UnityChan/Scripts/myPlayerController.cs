using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPlayerController : MonoBehaviour {
    private Animator animator;
    private bool isSpeedLocked = false;
    private bool isDirectionLocked = false;
    private bool isRotatiomLocked = false;
    public float watcher;
    public bool watcher2;
    public float Speed
    {
        get
        {
            return animator.GetFloat("Speed");
        }
        set
        {
            if (!isSpeedLocked)
                animator.SetFloat("Speed", value);
        }
    }
    private float SmoothSetSpeed
    {
        set
        {
            if (!isSpeedLocked)
            {
                animator.SetFloat("Speed", value, 0.25f, Time.deltaTime);
            }
        }
    }
    private float Direction
    {
        get
        {
            if (isDirectionLocked)
                return 0;
            return animator.GetFloat("Direction");
        }
        set
        {
            if (!isDirectionLocked)
            {
                animator.SetFloat("Direction", value);
            }
        }
    }
    private float SmoothSetDirection
    {
        set
        {
            if (!isDirectionLocked)
            {
                animator.SetFloat("Direction", value, 0.25f, Time.deltaTime);
            }
        }
    }
    public bool willJump
    {
        set
        {
            animator.SetBool("willJump", value);
        }
        get
        {
            return animator.GetBool("willJump");
        }
    }
    public float Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            if (!isRotatiomLocked)
                _rotation = value;
        }
    }
    private float _rotation = 0;
    private float minMoveSpeed = 0.1f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //准备跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tryJump())
                LockWhenJumping();
        }
        else if (haveJumped())
        {
            UnlockWhenJumped();
        }
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
        //修改速度
        SmoothSetSpeed = Mathf.Abs(Direction) + 2 * Mathf.Abs(Input.GetAxis("Vertical"));
        if (Speed <= minMoveSpeed)
        {
            Speed = 0;
            return;
        }

        //修改方向
        watcher2 = isDirectionLocked;
        float newdirection = (isDirectionLocked) ? 0 : Input.GetAxis("Horizontal");
        watcher = newdirection;
        if (newdirection * Direction < 0)
        {
            SmoothSetSpeed = 0;
            newdirection = 0;
        }
        else
        {
            Rotation += newdirection;
        }
        Direction = newdirection;
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
        Vector3 nextstep = nextz * front + nextx * right;
        return nextstep;
    }
    public bool tryJump()
    {
        if (isJumping())
            return false;
        else if (!willJump)
        {
            willJump = true;
            return true;
        }
        return false;
    }
    private bool isJumping()
    {
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Jump")) 
        {
            return true;
        }
        return false;
    }
    private bool haveJumped()
    {
        var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Jump") && animatorStateInfo.normalizedTime > 0.8)
        {
            willJump = false;
            return true;
        }
        return false;
    }
    private void LockWhenJumping()
    {
        isSpeedLocked = true;
        isDirectionLocked = true;
        isRotatiomLocked = true;
    }
    private void UnlockWhenJumped()
    {
        isSpeedLocked = false;
        isDirectionLocked = false;
        isRotatiomLocked = false;
    }
}
