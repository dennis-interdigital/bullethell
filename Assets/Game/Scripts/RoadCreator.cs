using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace BulletHell
{
    public class RoadCreator : MonoBehaviour
    {
        public GameObject[] m_RoadPartPrefabs;
        public GameObject[] m_ObjectPackPrefabs;
        public GameObject[] m_ItemPackPrefabs;

        public RoadPart m_LastPart;
        [HideInInspector]
        public ObstaclePack m_LastObstacle;
        [HideInInspector]
        public List<ObstaclePack> m_Obstacles;

        [HideInInspector]
        public int ObstacleCounter = 0;
        [HideInInspector]
        public int ItemCounter = 0;

        [SerializeField] Transform RoadParent;

        private StageManager stageManager;

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            m_Obstacles = new List<ObstaclePack>();

            RoadPart last = null;
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = Instantiate(m_RoadPartPrefabs[0], RoadParent);
                RoadPart p = obj.GetComponent<RoadPart>();
                p.Init(stageManager.gameController);

                if (i == 0)
                {
                    obj.transform.position = Vector3.zero;
                    //PlayerCar.m_Current.m_CurrentRoadPart = p;
                }
                else
                {
                    obj.transform.position = last.EndPoint.position;
                    //obj.transform.rotation = Quaternion.Euler(0, i * 3, 0);
                }

                if (last != null)
                {
                    last.m_NextPart = p;
                }
                last = p;
                m_LastPart = last;
            }
        }

        public void DoUpdate()
        {
            if (m_LastPart.transform.position.y < 200)
            {
                for (int i = 0; i < 10; i++)
                {
                    int r = Random.Range(0, m_RoadPartPrefabs.Length);
                    GameObject obj;

                    obj = Instantiate(m_RoadPartPrefabs[r], RoadParent);


                    RoadPart p = obj.GetComponent<RoadPart>();
                    p.Init(stageManager.gameController);
                    obj.transform.position = m_LastPart.EndPoint.position;
                    //obj.transform.rotation = m_LastPart.transform.rotation;
                    m_LastPart.m_NextPart = p;
                    m_LastPart = p;
                }
            }
        }
    }
}