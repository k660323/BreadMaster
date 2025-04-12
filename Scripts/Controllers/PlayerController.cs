using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Define;

// 1. ¿ßƒ° ∫§≈Õ
// 2. πÊ«‚ ∫§≈Õ
public class PlayerController : BaseController
{
    float velocity;
    public override Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;
            switch (_state)
            {
                case Define.State.Idle:
                    Owner.anim.SetBool("bMove", false);
                    break;
                case Define.State.Moving:
                    Owner.anim.SetBool("bMove", true);
                    break;
            }
        }
    }

    public override void Init()
    {
        TryGetComponent(out owner);
    }

    void Update()
    {
        switch(State)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Moving:
                UpdateMove();
                break;
        }
    }

    void UpdateIdle()
    {
        if(Managers.Input.inputDir != Vector2.zero)
        {
            State = State.Moving;
            return;
        }    
    }

    void UpdateMove()
    {
        if(Managers.Input.inputDir == Vector2.zero)
        {
            State = State.Idle;
            return;
        }

    }

    private void FixedUpdate()
    {
        velocity += Physics.gravity.y * Time.fixedDeltaTime;

        Player player = Owner as Player;
        if (State == State.Moving)
        {
            transform.forward = new Vector3(Managers.Input.inputDir.x, 0, Managers.Input.inputDir.y);
            player.CharacterController.Move(new Vector3(Managers.Input.inputDir.x, velocity, Managers.Input.inputDir.y) * Owner.stat.MoveSpeed * Time.deltaTime);
        }

        player.CharacterController.Move(new Vector3(0.0f, velocity, 0.0f));
    }
}
