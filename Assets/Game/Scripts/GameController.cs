using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform m_SpeedParticle;

    public float m_GameSpeed = 100;

    public void HandleGameOver()
    {
        m_GameSpeed = 0;
        m_SpeedParticle.gameObject.SetActive(false);
    }

    public void HandleWin()
    {

    }


}