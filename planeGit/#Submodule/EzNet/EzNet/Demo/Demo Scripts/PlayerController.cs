using EzNetLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public bool isOwn = false;
    public bool useMovementLerp = true;

    #region Movement Lerp
    private Vector3 destPosition = Vector3.zero; // 보간할 위치 정보를 저장하는 변수
    private Vector3 destRotation = Vector3.zero; // 보간할 회전 정보를 저장하는 변수
    #endregion

    #region Movement
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float playerSpeed = 2.0f; // 플레이어 이동 속도를 나타내는 변수
    private float jumpHeight = 1.0f; // 플레이어 점프 높이를 나타내는 변수
    private float gravityValue = -9.81f;
    float sendTime = 0.0f;
    bool moved = false;
    #endregion

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime; // 플레이어의 y축 속도에 중력 가속도를 더해줌
            controller.Move(playerVelocity * Time.deltaTime); // 플레이어 이동
        }

        if (isOwn) // 현재 객체가 로컬 플레이어인 경우
        {
            PlayerInput();
        }
        else // 현재 객체가 로컬 플레이어가 아닌 경우
        {
            UpdateLerpMovement();
        }
    }

    public void PlayerInput()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
            moved = true;
        }
        if (Input.GetButtonDown("Jump")/* && controller.isGrounded*/)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

        sendTime -= Time.deltaTime;
        if (moved)
        {
            if (sendTime <= 0.0f)
            {
                EzNet.Add("pos", transform.position);
                EzNet.Add("rot", transform.eulerAngles);
                EzNet.SendBroadcast(EzNet.WhereSpace.MySpace, GetComponent<EzNetObject>(), nameof(MovementReceiver));
                //EzNet.SendBroadcast(EzNet.WhereSpace.MySpace, GetComponent<EzNetObject>()
                //    , nameof(MovementReceiver), nameof(PlayerSpawner.SpawnNewPlayer));
                Debug.Log("Send my data");
                sendTime = 0.1f;
                moved = false;
            }
        }
    }
    public void UpdateLerpMovement()
    {
        if (destPosition != Vector3.zero)
            transform.position = Vector3.Lerp(transform.position, destPosition, Time.deltaTime * 10.0f);
        if (destRotation != Vector3.zero)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, destRotation, Time.deltaTime * 10.0f);
    }
    public float GetVelocityDifferencePercent(Vector3 oldVelocity, Vector3 newVelocity)
    {
        Vector3 velocityDiff = newVelocity - oldVelocity;
        float velocityMag = oldVelocity.magnitude;

        if (velocityMag > 0)
        {
            float percent = (velocityDiff.magnitude / velocityMag) * 100f;
            return percent;
        }

        return 0f;
    }

    #region Network Methods

    public void MovementReceiver()
    {
        if (useMovementLerp)
        {
            ReadLerpMovement();
        }
        else
        {
            ReadRawMovement();
        }
    }
    public void ReadLerpMovement()
    {
        destPosition = EzNet.Read<Vector3>("pos");
        destRotation = EzNet.Read<Vector3>("rot");
    }
    public void ReadRawMovement()
    {
        transform.position = EzNet.Read<Vector3>("pos");
        transform.eulerAngles = EzNet.Read<Vector3>("rot");
    }

    #endregion
}
