using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Framework.Common.Template.SceneLoader.Example
{
    public class SceneLoaderTest : MonoBehaviour
    {
        void Start()
        {
            SceneLoader.Instance.OnReceiveOrder.Where(info => info.prev == "Prev Scene Name" && info.next == "Next  Scene Name").Subscribe(info =>
            {
                //씬 전환 명령을 받았을 때 처리...
            });
            
            SceneLoader.Instance.OnPreLoad.Where(info => info.prev == "Prev Scene Name" && info.next == "Next  Scene Name").Subscribe(info =>
            {
                //전처리 이후, 씬 전환 전 처리...
            });
            
            SceneLoader.Instance.OnLoading.Where(info => info.prev == "Prev Scene Name" && info.next == "Next  Scene Name").Subscribe(info =>
            {
                //씬 전환 중 매 프레임 처리...
                //progress 정보 포함
            });
            
            SceneLoader.Instance.OnPostLoad.Where(info => info.prev == "Prev Scene Name" && info.next == "Next  Scene Name").Subscribe(info =>
            {
                //씬 전환 후 처리...
            });
            
            //씬 전환
            SceneLoader.Instance.LoadSceneAsync(1, async () =>
            {
                //씬 전환 전 3초 대기
                //실제로 사용 시 데이터 동기화 등의 비동기 처리 진행
                await UniTask.Delay(3000);      
            }).Forget();
        }
    }
}
