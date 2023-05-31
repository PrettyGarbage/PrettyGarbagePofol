using Common;
using MJ.Utils;
using UnityEngine;

public class PlayerSpawnSystem : AppContext<PlayerSpawnSystem>
{
    public Player Spawn(Define.Role role)
    {        
        var playerGo = Managers.Resource.Instantiate(Constants.PrefabChar(role.ToString()));
        if (!playerGo)
        {
            Logger.LogError("PlayerSpawnSystem.Spawn - playerGo is null");
            return null;
        }

        if (DataModel.Instance.Mine.Role.Value == role) playerGo.GetComponentsInChildren<Renderer>().ForEach(x => x.enabled = false);
        
        Player player = playerGo.GetOrAddComponent<Player>();
        player.Role = role;
        if(DataModel.Instance.USModels.Mine().Role.Value == role) player.SetPlugin();
        
        SpawnPlayer.Instance.SpawnPlayerOnPoint(player);
        return player;
    }
    
    public void Despawn(Player player)
    {
        if (player != null)
        {
            Managers.Resource.Destroy(player.gameObject);
        }
    }
}
