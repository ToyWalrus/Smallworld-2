using UnityEngine;


namespace UnityModels
{
    public abstract class IUnityModel<T> : MonoBehaviour
    {
        public abstract T GetModel();
    }
}