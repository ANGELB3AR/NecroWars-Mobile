using UnityEditor;
using UnityEngine;

namespace SmitesoftUnpack
{
    [ExecuteInEditMode]
    public class PrefabUnpack : MonoBehaviour
    {
        private bool isPrefabUnpacked;

        public void OnValidate() //Added on 17-9-2022
        {
            if (!isPrefabUnpacked)
            {
                //Added on 26-9-2022 //Added this for those already unpacked such as those the demos
                if (!PrefabUtility.IsPartOfAnyPrefab(gameObject))
                {
                    isPrefabUnpacked = true;
                   
                }

                //Added on 17-9-2022
                if (PrefabUtility.IsPartOfAnyPrefab(gameObject) &&
                    PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
                {
                    PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    isPrefabUnpacked = true;
                    
                }
            }

        }

        public void OnEnable()
        {
            OnValidate();
            if (isPrefabUnpacked)
            {
                DestroyImmediate(this);
            }
        }
    }
}

