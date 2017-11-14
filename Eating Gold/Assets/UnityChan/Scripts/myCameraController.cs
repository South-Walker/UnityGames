using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCameraController : MonoBehaviour {
    public GameObject FollowObject;
    public float Smooth;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 now = this.transform.position;
        Vector3 aim = FollowObject.transform.position;
        aim = aim - FollowObject.transform.forward * 12 + new Vector3(0, 2.5f, 0);
        this.transform.position = Vector3.Lerp(aim, now, Time.deltaTime * Smooth);
        this.transform.rotation = Quaternion.Euler(5, 0, 0);
	}
}
