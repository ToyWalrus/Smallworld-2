using UnityEngine;


namespace UnityModels
{
    public abstract class UnityModel<T> : MonoBehaviour
    {
        public abstract T GetModel();
    }
}