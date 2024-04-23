using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KajObstacle : MonoBehaviour
{
    Animator anim;
    float strafe;
    Vector3 defAngle;
    Vector3 defRight;

    [SerializeField]
    float speed;
    [SerializeField]
    float strafeTime;


    void Awake()
    {
        anim = GetComponent<Animator>();
        defAngle = transform.rotation.eulerAngles;
        defRight = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        float decStr = Time.deltaTime / strafeTime;

        if (Input.GetKey(KeyCode.I))
        {
            strafe -= decStr;
            strafe = Mathf.Max(-1, strafe);
        }
        else if (Input.GetKey(KeyCode.P))
        {
            strafe += decStr;
            strafe = Mathf.Min(+1, strafe);
        }
        else
        {
            if (strafe > 0f)
            {
                strafe -= decStr;
                strafe = Mathf.Max(0, strafe);
            }
            else if (strafe < 0f)
            {
                strafe += decStr;
                strafe = Mathf.Min(0, strafe);
            }
        }

        anim.SetFloat("Strafe", strafe);
        transform.rotation = Quaternion.Euler(defAngle + new Vector3(0, 30*strafe, 0));
        transform.position = Vector3.MoveTowards(pos, pos + defRight, strafe * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.O))
        {
            anim.SetTrigger("Attack");
        }
    }
}
