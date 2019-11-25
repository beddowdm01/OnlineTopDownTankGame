using Photon.Pun;
using UnityEngine;


public class TDMGameMode : GameMode
{
    [SerializeField]
    private int killsToWin = 10;

    public override void UpdateGameModeScore()
    {
        foreach (PlayerCharacter player in players)
        {
            if(player.GetKills() >= killsToWin)
            {
                photonView.RPC("EndGame", RpcTarget.All, player.GetName());
            }
        }
    }

}
