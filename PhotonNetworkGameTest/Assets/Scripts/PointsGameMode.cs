using Photon.Pun;
using UnityEngine;

public class PointsGameMode : GameMode
{
    [SerializeField]
    private int damageToWin = 500;

    public override void UpdateGameModeScore()
    {
        foreach (PlayerCharacter player in players)
        {
            if (player.GetDamage() >= damageToWin)
            {
                photonView.RPC("EndGame", RpcTarget.All, player.GetName());
            }
        }
    }
}
