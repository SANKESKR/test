using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;//如果没有添加Box，这里的Collider就代表所有的碰撞盒了

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;//下蹲的时候减缓速度

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration=0.1f;
    public float crouchJumpBoost = 2.5f;
    float jumpTime;
    public float jumpHangingForce = 15f;


    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grapDistance = 0.4f;
    public float reachOffset = 0.7f;

    public LayerMask groundLayer;

    public float xVelocity;//判断x轴上的力的方向

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;
    //碰撞体的尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        playerHeight = coll.size.y;
        colliderStandOffset = coll.offset;
        colliderStandSize = coll.size;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        if (Input.GetButtonDown("Crouch"))
        {
            crouchPressed = true;
        }
        //jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }
    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();


    }

    void PhysicsCheck()
       {
        //    Vector2 pos = transform.position;
        //    Vector2 offset = new Vector2(-footOffset, 0f);
        //    RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, groundLayer);
        //    Debug.DrawRay(pos + offset, Vector2.down, Color.red, groundDistance);
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (leftCheck||rightCheck)
        {
            isOnGround = true;
        }
        else
            isOnGround = false;
        if(headCheck)
        {
            isHeadBlocked = true;
        }
        else
        {
            isHeadBlocked = false;
        }

        float direction = transform.localScale.x;
        Vector2 grapDir = new Vector2(direction, 0f);
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grapDir, grapDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grapDir, grapDistance, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grapDistance, groundLayer);

        if(!isOnGround&&rb.velocity.y<0f&&ledgeCheck&&wallCheck&&!blockedCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance-0.05f)* direction;//射线.distance的意思是从发射射线的点到碰到最近的一个碰撞体之间的距离
            pos.y -= ledgeCheck.distance;
            transform.position = pos;
            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }
    void GroundMovement()
    {
        if(isHanging)
        {
            return;
        }
        if(crouchHeld&&!isCrouch&&isOnGround)
        {
            Crouch();
        }
        else if(!crouchHeld&&isCrouch&&!isHeadBlocked)
        {
            StandUp();
        }
        else if(!isOnGround&&isCrouch)
        {
            StandUp();
        }
        xVelocity = Input.GetAxis("Horizontal");//-1f 1f
        if(isCrouch)
        {
            xVelocity /= crouchSpeedDivisor;
        }
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FilpDirection();
    }

    void MidAirMovement()
    {
        if(isHanging)
        {
            if(jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, jumpHangingForce);
                isHanging = false;
            }
            if(crouchPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }
        if(jumpPressed&&isOnGround&&!isJump && !isHeadBlocked)
        {
            if(isCrouch)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration;//这里的Time.time是记录从按下play游戏的那一刻开始到现在的时间，比如说现在是游戏开始运行的第十五秒，那现在就是15+0.1=15.1秒
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AudioManager.PlayerJumpAudio();
            jumpPressed = false;
            crouchPressed = false;
        }
        else if(isJump)
        {
            if(jumpHeld)
            {
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            }
            if(jumpTime<Time.time)//在游戏运行的过程中时间肯定是在慢慢地增长的，当时间超过15.1的时候，isjump就是自动变成false，这时候就会退出前面的else if，这就相当于实现了一个简便的倒计时器
            {
                isJump = false;
            }
        }
    }

    void FilpDirection()
    {
        if(xVelocity<0)
        {
            transform.localScale = new Vector3(-1, 1,1);

        }
        if(xVelocity>0)
        {
            transform.localScale = new Vector3(1, 1,1);
        }
    }

    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }

    RaycastHit2D Raycast(Vector2 offset,Vector2 rayDirection,float length,LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, rayDirection * length,color);
        return hit;
    }

    
}
