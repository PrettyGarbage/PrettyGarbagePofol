using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Library.Manager;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Image loadingBar;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text bodyText;
    
    public async UniTask ShowAsync(Dialogue dialogue)
    {
        titleText.text = dialogue.Title;
        bodyText.text = string.Empty;
        loadingBar.fillAmount = 0f;

        animator.SetBool("IsOpen", true);
        await animator.WaitAnimationCompleteAsync(this.GetCancellationTokenOnDisable());
        
        foreach (var body in dialogue.Body)
        {
            float duration = body.Message.Length * 0.1f;
            
            if (body.Clip != null)
            {
                SoundManager.Instance.PlaySoundEffect(body.Clip, body.Volume);
                duration = body.Clip.length;
            }

            bodyText.text = body.Message;

            var timer = 0f;
            while (timer < duration)
            {
                await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDisable());
                timer += Time.deltaTime * (ConfigModel.Instance.Setting.debugMode ? 1 : DataModel.Instance.OPSModel.MissionSpeed.Value);
                loadingBar.fillAmount = timer / duration;
            }

            loadingBar.fillAmount = 1;
        }
    }

    public async UniTask HideAsync()
    {
        animator.SetBool("IsOpen", false);
        await animator.WaitAnimationCompleteAsync(this.GetCancellationTokenOnDisable());
        
        Managers.Resource.Destroy(gameObject);
        titleText.text = string.Empty;
        bodyText.text = string.Empty;
        loadingBar.fillAmount = 0f;
    }
}
