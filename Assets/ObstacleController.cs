using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    Animator anim;
    float strafe;
    Vector3 defAngle;
    Vector3 defRight;

    [SerializeField]
    float speed;
    [SerializeField]
    float strafeStartTime;
    [SerializeField]
    float range;
    [SerializeField]
    AttackFrequency attackFreq;


    bool shouldStrafeLeft = false;
    bool shouldStrafeRight = false;
    bool shouldAttack = false;

    Vector3 initialPosition;
    Vector3 leftPosition;
    Vector3 rightPosition;
    float lastSetToAttackDelta;
    bool dead = false;

    public enum AttackFrequency // your custom enumeration
    {
        None,
        Sides,
        Center,
        SidesAndCenter,
        Random
    }


    void Awake()
    {
        anim = GetComponent<Animator>();
        defAngle = transform.rotation.eulerAngles;
        defRight = transform.right;

        initialPosition = transform.position;
        leftPosition = initialPosition - transform.right * range / 2;
        rightPosition = initialPosition + transform.right * range / 2;
        if (range > 0)
        {
            shouldStrafeRight = Random.Range(0f, 1f) > 0.5f;
            shouldStrafeLeft = !shouldStrafeRight;
        }
    }

    private void Start()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.obstacles.Add(this); 
        }
        else if (LevelSelectSceneManager.instance == null)
        {
            Debug.LogError("LevelManager & LevelSelectSceneManager, both NOT FOUND!");
        }
    }


    void Update()
    {
        if (dead) return;

        if (PerformingAttack())
        {
            shouldAttack = false;
            lastSetToAttackDelta = 0;
            if (attackFreq == AttackFrequency.Random)
                lastSetToAttackDelta = -Random.Range(0f, 2f);
            return;
        }

        if (range == 0)
        {
            SetShouldAttack();
            return;
        }

        handleStrafe();
        handleAttacks();        
    }

    bool CloseToPosition(Vector3 pos)
    {
        return (transform.position - pos).sqrMagnitude < 0.1f;
    }

    bool NotAttackedJustNow()
    {
        return lastSetToAttackDelta > 1.8f;
    }

    bool PerformingAttack()
    {
        return anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Zombie Attack");
    }
    void handleStrafe()
    {
        Vector3 pos = transform.position;
        float decStr = Time.deltaTime / strafeStartTime;

        if (!shouldAttack && shouldStrafeLeft)
        {
            strafe -= decStr;
            strafe = Mathf.Max(-1, strafe);
        }
        else if (!shouldAttack && shouldStrafeRight)
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
        transform.rotation = Quaternion.Euler(defAngle + new Vector3(0, 30 * strafe, 0));
        transform.position = Vector3.MoveTowards(pos, pos + defRight, strafe * speed * Time.deltaTime);
    }

    void handleAttacks()
    {
        lastSetToAttackDelta += Time.deltaTime;

        Vector3 currentPos = transform.position;
        if (lastSetToAttackDelta > 1f && CloseToPosition(initialPosition))
        {
            CheckAttackOnCenter();
        }
        else if (shouldStrafeLeft)
        {
            if (CloseToPosition(leftPosition))
            {
                CheckAttackOnSide();
                shouldStrafeRight = true;
                shouldStrafeLeft = false;
            }
        }
        else if (shouldStrafeRight)
        {
            if (CloseToPosition(rightPosition))
            {
                CheckAttackOnSide();
                shouldStrafeLeft = true;
                shouldStrafeRight = false;
            }
        }

        CheckRandomAttack();
    }

    void CheckAttackOnSide()
    {
        if (attackFreq == AttackFrequency.Sides || attackFreq == AttackFrequency.SidesAndCenter)
        {
            if (NotAttackedJustNow()) SetShouldAttack();
        }
    }
    void CheckAttackOnCenter()
    {
        if (attackFreq == AttackFrequency.Center || attackFreq == AttackFrequency.SidesAndCenter)
        {
            if (NotAttackedJustNow()) SetShouldAttack();
        }

    }

    void CheckRandomAttack()
    {
        if (attackFreq == AttackFrequency.Random)
        {
            if (NotAttackedJustNow()) SetShouldAttack();
        }
    }
    void SetShouldAttack()
    {
        shouldAttack = true;
        lastSetToAttackDelta = 0;
        anim.SetTrigger("Attack");
    }

    public void SetDead()
    {
        dead = true;
        anim.SetTrigger("Dead");
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    /*void Update()
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Zombie Attack"))
            return;


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
        transform.rotation = Quaternion.Euler(defAngle + new Vector3(0, 30 * strafe, 0));
        transform.position = Vector3.MoveTowards(pos, pos + defRight, strafe * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.O))
        {
            anim.SetTrigger("Attack");
        }
    }*/

    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        Vector3 pos = transform.position + Vector3.up;
        Vector3 right = transform.right * range / 2;
        Gizmos.DrawLine(pos - right, pos + right);
        Gizmos.DrawWireSphere(pos - right, 0.5f);
        Gizmos.DrawWireSphere(pos + right, 0.5f);
    }
#endif
}
