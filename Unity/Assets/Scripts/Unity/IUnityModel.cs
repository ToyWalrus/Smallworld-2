using UnityEngine;


namespace UnityModels
{
    public interface IUnityModel<T>
    {
        public T GetModel();
    }
}