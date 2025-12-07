using UnityEngine;

namespace Prototyping
{
    public class LocationLoader : MonoBehaviour
    {
        // Make this unique per object you want to save
        [SerializeField] private string saveKey = "_Position";

        private string KeyX => saveKey + "_x";
        private string KeyY => saveKey + "_y";
        private string KeyZ => saveKey + "_z";

        // Called when the component or GameObject becomes enabled/active
        private void OnEnable()
        {
            LoadPositionIfExists();
        }

        // Called when the component or GameObject becomes disabled/inactive
        private void OnDisable()
        {
            SavePosition();
        }

        private void SavePosition()
        {
            Vector3 pos = transform.position;

            PlayerPrefs.SetFloat(KeyX, pos.x);
            PlayerPrefs.SetFloat(KeyY, pos.y);
            PlayerPrefs.SetFloat(KeyZ, pos.z);
            PlayerPrefs.Save();   // Force write to disk
        }

        private void LoadPositionIfExists()
        {
            if (!PlayerPrefs.HasKey(KeyX)) 
            {
                // Nothing saved yet
                return;
            }

            float x = PlayerPrefs.GetFloat(KeyX);
            float y = PlayerPrefs.GetFloat(KeyY);
            float z = PlayerPrefs.GetFloat(KeyZ);

            Vector3 loadedPos = new Vector3(x, y, z);
            transform.position = loadedPos;
        }
    }
}