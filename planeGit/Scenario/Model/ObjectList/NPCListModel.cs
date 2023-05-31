using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCListModel : SceneContext<NPCListModel>
{
    #region Properties

    public List<NPCModel> NPCList { get; private set;  }

    #endregion

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        NPCList = FindObjectsOfType<NPCModel>(true).ToList();
        
        //Ex :
        //NPCList.Where(model => model.id != 5).ForEach(model => model.Animator.SetTrigger("Idle" ));
    }

    #endregion

    #region Public Methods

    public NPCModel Get(int id) => NPCList.FirstOrDefault(npc => npc.id == id);
    public NPCModel[] Gets(params int[] ids) => NPCList.Where(npc => ids.Contains(npc.id)).ToArray();
    public NPCModel[] GetsExcept(params int[] ids) => NPCList.Where(npc => !ids.Contains(npc.id)).ToArray();
    
    
    #endregion
}
