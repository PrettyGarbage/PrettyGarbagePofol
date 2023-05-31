using System;
using Common;
using Cysharp.Threading.Tasks;

public class DialogueSystem : SceneContext<DialogueSystem>
{
    DialogueView dialogueView;
    
    public async UniTask ShowDialogueAsync(Dialogue dialogue, float waitSeconds = 0)
    {
        await HideDialogueAsync();
        
        dialogueView = Managers.Resource.Instantiate(Constants.PrefabUI("DialogueCanvas"), transform).GetComponent<DialogueView>();
        await UniTask.WhenAll(dialogueView.ShowAsync(dialogue), UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: this.GetCancellationTokenOnDestroy()));
    }

    public async UniTask HideDialogueAsync()
    {
        if (dialogueView != null)
        {
            await dialogueView.HideAsync();
            Managers.Resource.Destroy(dialogueView.gameObject);
        }
        
        dialogueView = null;
    }
}
