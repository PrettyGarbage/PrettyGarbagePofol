using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Valve.VR;
public class CanvasTransform : MonoBehaviour
{
    ///<summary>
    ///최종 목표 지점
    ///</summary>
    private Vector3 _goalPosition = Vector3.zero;
    ///<summary>
    ///시작지점
    ///</summary>
    private Vector3 _startPosition = Vector3.zero;

    private float _startTime = 0f;
    private float _distance = 0f;

    public float _distanceFromCamera = 50f;
    public float _distTime = 15f;
    async void OnEnable()
    {
        await UniTask.WaitUntil(() => AppFlowModel.Instance.AppFlow.CurrentState == Define.AppFlowState.Main);
        
        if(DataModel.Instance.MyId == 0)
        {
            var childCanvases = GetComponentsInChildren<Canvas>();
            foreach(var canvas in childCanvases)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            this.enabled = false;
            return;
        }
        
        InitCanvas();
    }

    private void OnDisable()
    {
        //플레이어 포지션 이동좌표 리스너 삭제
        SteamVR_Events.NewPoses.RemoveListener(MoveSceneChangeCanvas);
    }

    private void InitCanvas()
    {
        //처음 켜질때 카메라 앞에 생성
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * _distanceFromCamera;
        _goalPosition = transform.position;

        //플레이어 포지션 이동좌표 리스너 등록
        SteamVR_Events.NewPoses.AddListener(MoveSceneChangeCanvas);
    }

    ///<summary>
    ///캔버스 위치 이동
    ///</summary>
    ///<param name="pose"></param>
    void MoveSceneChangeCanvas(TrackedDevicePose_t[] pose)
    {
        //카메라가 보고있는 위치에서 50f만큼 앞에 배치한다
        var cameraSeePosition = Camera.main.transform.position + Camera.main.transform.forward * _distanceFromCamera;
        if (float.IsNaN(cameraSeePosition.x) || float.IsNaN(cameraSeePosition.y) || float.IsNaN(cameraSeePosition.z))
            return;

        //카메라가 보고있는 위치가 현재 오브젝트의 위치보다 30f이상 차이날 경우
        if (Vector3.Distance(_goalPosition, cameraSeePosition) > _distanceFromCamera / 2f)
        {
            _goalPosition = cameraSeePosition;
            _startPosition = transform.position;
            _startTime = Time.time;
            _distance = Vector3.Distance(_startPosition, _goalPosition);
        }
    }

    private void FixedUpdate()
    {
        var distTime = (Time.time - _startTime) / _distance * _distTime;
        var position = Vector3.Lerp(_startPosition, _goalPosition, distTime);
        if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
            return;
        transform.position = position;
        transform.LookAt(Camera.main.transform);
    }
}
