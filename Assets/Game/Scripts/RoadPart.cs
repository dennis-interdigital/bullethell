using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bullethell
{
    public class RoadPart : MonoBehaviour
    {
        public Transform EndPoint;

        [HideInInspector]
        public RoadPart m_NextPart;

        public float m_MoveSpeed = 0;

        bool isInit = false;
        public GameController m_Control;


        public void Init(GameController gameController)
        {
            m_Control = gameController;
            isInit = true;
        }

        void Update()
        {
            if (isInit)
            {
                transform.position += m_Control.m_GameSpeed * Time.deltaTime * new Vector3(0, -1, 0);

                if (transform.position.y <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }


        void OnDrawGizmos()
        {
            //if (PlayerCar.m_Current != null)
            //{
            //    if (PlayerCar.m_Current.m_CurrentRoadPart == this)
            //    {
            //        Gizmos.color = Color.red;
            //        Gizmos.DrawLine(transform.position, transform.position+new Vector3(0,40,0));
            //    }
            //}
        }
    }
}
