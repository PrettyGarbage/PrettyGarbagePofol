using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinBoard : MonoBehaviour {

    private float _targetAngle = 0;

    private Action _onSpinComplete;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Image _boardImage;
    [SerializeField]
    private string[] _spinEventItemIds;

    private bool _isJackpot;
    private List<ProductItemInfo> _productItemInfos;

    private BaseAniBehaviour _aniBehaviour;

    public void Spin(SpinBoardResult spinBoardResult, Action onSpinComplete)
    {
        _isJackpot = false;
        
        if (_aniBehaviour == null)
        {
            _aniBehaviour = _animator.GetBehaviour<BaseAniBehaviour>();
            _aniBehaviour.onStateExit += OnSpinEnd;
        }
        
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_enevt_spin_click);

        _onSpinComplete = onSpinComplete;
        for (int i = 0; i < _spinEventItemIds.Length; i++)
        {
            if (_spinEventItemIds[i].Equals(spinBoardResult.spinBoardId))
            {
                SpinBoardAction(i, spinBoardResult.productItemList);
                _isJackpot = _spinEventItemIds[i].Equals(GameConstants.SPIN_EVENT_JACKPOT);
                break;
            }
        }        
    }


    private void SpinBoardAction(int index, List<ProductItemInfo> productItemInfos)
    {
        _productItemInfos = productItemInfos;
        _targetAngle =  ((360 / _spinEventItemIds.Length) * index);
        Debug.Log("_targetAngle : " + _targetAngle);
        _animator.Play("spin", -1, 0);
    }

    private void OnSpinEnd(AnimatorStateInfo obj)
    {
        Debug.Log("SpinBoardAction End");
        if (_isJackpot)
        {
            PopupSpinEventRewardState.Open(_productItemInfos);
        }
        else
        {
            PopupRewardState.Open(_productItemInfos);
        }

        if (_onSpinComplete != null)
            _onSpinComplete();
    }

    public void OnAniEventSetCollectAngle()
    {
        _boardImage.transform.localEulerAngles = Vector3.forward * _targetAngle;
        SoundManager.Instance.PlayUISound(_boardImage.gameObject, AudioDataKey.ui_event_spinroll_loop);
    }

    public void OnAniEventRollSound()
    {
        SoundManager.Instance.PlayUISound(_boardImage.gameObject, AudioDataKey.ui_event_spinroll_loop);
    }

}
