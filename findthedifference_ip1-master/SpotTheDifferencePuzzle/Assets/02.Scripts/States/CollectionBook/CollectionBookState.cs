using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class CollectionBookState : State
{
    [SerializeField]
    private Transform _collectionContent;
    [SerializeField]
    private CollectionItem _pfCollectionItem;

    [SerializeField]
    private ScrollRect _scrollRect;
    
    private List<CollectionInfo> _collectionInfoList;
    private List<CollectionItem> _collectionItemList;

    private CollectionGradeToggle[] _gradeToggles;

    private CollectionGradeType _selectGradeType = CollectionGradeType.NONE;
    private Coroutine _gradeChangedCoroutine;

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

        if (_collectionInfoList == null)
        {
             AssetBundleManager.Instance.LoadAssetBundles(DataManager.Instance.AssetBundleData.collectionAssetBundle, false, null,() => {});

            _selectGradeType = CollectionGradeType.NONE;
            _collectionItemList = new List<CollectionItem>();
            _collectionInfoList = DataManager.Instance.GetCollectionInfoList();
            CreateItems();
            
            _gradeToggles = GetComponentsInChildren<CollectionGradeToggle>();

            for (int i = 0; i < _gradeToggles.Length; i++)
            {
                CollectionGradeType collectionGradeType = (CollectionGradeType)(i-1);
                _gradeToggles[i].SetToggle(collectionGradeType, OnGradeChanged);
            }

            _gradeToggles[0].Toggle.isOn = true;
        }
        else
        {
            SetCollectionList();
        }
    }

    private void OnDisable()
    {
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateUserCurrency);
    }

    public override IEnumerator OnPostOpen()
    {
        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateUserCurrency);
        yield return null;
    }

    private void UpdateUserCurrency(object[] args)
    {  
        SetCollectionList();
    }

    IEnumerator ShowCollections()
    {
        for (int i = 0; i < _collectionItemList.Count; i++)
        {
            _collectionItemList[i].gameObject.SetActive(_selectGradeType == CollectionGradeType.NONE || _collectionInfoList[i].gradeType == _selectGradeType);
            if (_collectionItemList[i].gameObject.activeInHierarchy)
            {
                _collectionItemList[i].ShowAni();
                SoundManager.Instance.PlayUISound(_collectionItemList[i].gameObject, AudioDataKey.ui_collection_card_flip);
                yield return new WaitForSeconds(0.05f);
            }
            
        }
    }

    IEnumerator HideCollections()
    {

        //SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_theme_card_flip);

        for (int i = 0; i < _collectionItemList.Count; i++)
        {
            if (_collectionItemList[i].gameObject.activeInHierarchy)
            {
                _collectionItemList[i].HideAni();
            }
        }

        yield return new WaitForSeconds(0.35f);

        for (int i = 0; i < _collectionItemList.Count; i++)
        {
            _collectionItemList[i].gameObject.SetActive(false);
        }
    }

    IEnumerator ChangeList()
    {
        yield return StartCoroutine(HideCollections());

        yield return new WaitForSeconds(0.1f);
        _scrollRect.normalizedPosition = Vector2.zero;

        yield return StartCoroutine(ShowCollections());      
    }

    
    private void OnGradeChanged(CollectionGradeType gradeType)
    {
        if (gameObject.activeInHierarchy)
        {
            if (_gradeChangedCoroutine != null)
            {
                StopCoroutine(_gradeChangedCoroutine);
            }
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_common);
            _selectGradeType = gradeType;
            _gradeChangedCoroutine = StartCoroutine(ChangeList());
        }        
    }

    private void OnSelected(BaseEventData obj)
    {
        throw new NotImplementedException();
    }

    private void OnGradeToggleChagned(bool arg0)
    {
        throw new NotImplementedException();
    }

    public void OnClose()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), false);
    }

    public override void OnBack()
    {
        OnClose();
        if (_gradeChangedCoroutine != null)
        {
            StopCoroutine(_gradeChangedCoroutine);
        }
    }

    private void CreateItems()
    {
        for (int i = 0; i < _collectionInfoList.Count; i++)
        {
            GameObject go = ObjectUtil.InstantiateAtTarget(_pfCollectionItem.gameObject, _collectionContent) as GameObject;
            _collectionItemList.Add(go.GetComponent<CollectionItem>());
            _collectionItemList[i].SetCollectionItem(_collectionInfoList[i]);
        }
    }

    private void SetCollectionList()
    {
        for (int i = 0; i < _collectionItemList.Count; i++)
        {
            _collectionItemList[i].ShowCollectionItem();            
        }
    }

}