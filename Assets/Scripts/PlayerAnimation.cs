using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator anim;
    PlayerMovement movement;
    Rigidbody2D rb;
    int groundID;
    int hangingID;
    int crouchID;
    int speedID;
    int fallID;
    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody2D>();
        groundID = Animator.StringToHash("isOnGround");
        hangingID = Animator.StringToHash("isHanging");
        crouchID = Animator.StringToHash("isCrouching");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }

   
    void Update()
    {
        anim.SetFloat(speedID, Mathf.Abs(movement.xVelocity));
        anim.SetBool(groundID, movement.isOnGround);
        anim.SetBool(hangingID, movement.isHanging);
        anim.SetBool(crouchID, movement.isCrouch);
        anim.SetFloat(fallID, rb.velocity.y);//这个竖直方向上的速度就是原来写在Robbie上的那个速度，所以只需要获取父级的刚体就行了
        
    }

    public void StepAudio()
    {
        AudioManager.PlayerFootstepAudio();
    }

    public void CrouchStepAudio()
    {
        AudioManager.PlayerCrouchFootstepAudio();
    }
}
