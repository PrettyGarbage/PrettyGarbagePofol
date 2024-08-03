using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectionData.asset", menuName = "gbros/CollectionData", order = 1)]
public class CollectionSpriteData : ScriptableObject
{
    [SerializeField]
    private string _collectionId;
    [SerializeField]
    private Sprite _collectionImage;
    [SerializeField]
    private Sprite _collectionBigImage;

    public string CollectionId { get { return _collectionId; } }
    public Sprite CollectionImage { get { return _collectionImage; } }
    public Sprite CollectionBigImage { get { return _collectionBigImage; } }
}
