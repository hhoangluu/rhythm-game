using UnityEngine;
using Doulingo.Core;
using Doulingo.Gameplay;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating different types of staff drawers
    /// </summary>
    public class StaffDrawerFactory : MonoBehaviour
    {
        [Header("Staff Drawer Prefabs")]
        [SerializeField] private BeatLineStaffDrawer beatLineStaffDrawerPrefab;
        [SerializeField] private MusicalStaffDrawer musicalStaffDrawerPrefab;
        
        public IStaffDrawer GetStaffDrawer(StaffDrawerType type)
        {
            switch (type)
            {
                case StaffDrawerType.BeatLineStaffDrawer:
                    return Instantiate(beatLineStaffDrawerPrefab);
                    
                case StaffDrawerType.MusicalStaffDrawer:
                    return Instantiate(musicalStaffDrawerPrefab);
                    
                default:
                    Debug.LogError($"[StaffDrawerFactory] Unknown staff drawer type: {type}");
                    return null;
            }
        }
    }
}
