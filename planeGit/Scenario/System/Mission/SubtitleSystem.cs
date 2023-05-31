using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UnityEngine;

public class SubtitleSystem : SceneContext<SubtitleSystem>
{
    #region Public Methods

    public async UniTask ShowSubtitleAsync(Dialogue dialogue, float waitSeconds, CancellationToken token = default)
    {
        if (token.IsCancellationRequested) return;

        try
        {
            await UniTask.WhenAny(
                DialogueSystem.Instance.ShowDialogueAsync(dialogue, waitSeconds),
                UniTask.WaitUntilCanceled(token)
            );
            
            await DialogueSystem.Instance.HideDialogueAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public async UniTask ShowSubtitleAsync(Dialogue dialogue, CancellationToken token = default)
    {
        await ShowSubtitleAsync(dialogue, 0, token);
    }

    #endregion
}