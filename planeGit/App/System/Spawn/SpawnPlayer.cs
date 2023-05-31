using System;
using System.Linq;
using UnityEngine;

public class SpawnPlayer : SceneContext<SpawnPlayer>
{
    #region Field

    ///<summary>
    ///이런 방식은 안좋아하지만...
    ///0번 Observer 나머지 인덱스는 플레이어의 번호
    ///</summary>
    [SerializeField] Transform[] spawnPoint;
    
    Transform spawnPointObserver;
    Transform spawnPointClient1;
    Transform spawnPointClient2;
    Transform spawnPointClient3;
    Transform spawnPointClient4;

    #endregion

    public Vector3 this[int index]
    {
        get
        {
            return index switch
            {
                0 => spawnPointObserver?.position ?? Vector3.zero,
                1 => spawnPointClient1?.position ?? Vector3.zero,
                2 => spawnPointClient2?.position ?? Vector3.zero,
                3 => spawnPointClient3?.position ?? Vector3.zero,
                4 => spawnPointClient4?.position ?? Vector3.zero,
            };
        }
    }

    #region Public Methods

    public void SpawnPlayerOnPoint(Player player)
    {
        if (spawnPoint is { Length: 5 }) player.transform.position = spawnPoint[(int)player.Role].position;
    }

    #endregion
}