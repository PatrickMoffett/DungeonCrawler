using Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class BattleStarter : MonoBehaviour
    {
        public float distancePerProc = 100f;
        public float chancePerProc = 0.5f;
        
        private Vector3 _lastLocation;
        private float _currentDistance = 0f;

        public void Start()
        {
            _lastLocation = transform.position;
        }
        public void Update()
        {
            Vector3 currentLocation = transform.position;
            _currentDistance += Vector3.Distance(currentLocation, _lastLocation);
            _lastLocation = currentLocation;
            if (_currentDistance >= distancePerProc)
            {
                _currentDistance = 0f;
                if (chancePerProc > Random.Range(0f, 1f))
                {
                    ServiceLocator.Instance.Get<ApplicationStateManager>().PushState<BattleModeState>();
                }
            }
        }
    }
}