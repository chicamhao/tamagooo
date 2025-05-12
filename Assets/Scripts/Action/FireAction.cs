using System;
using Input;
using UnityEngine;

namespace Action
{
    public sealed class FireAction : MonoBehaviour
    {
        [SerializeField] private GameObject _lightObject;
        
        private InputHandle _inputHandle;

        private void Start()
        {
            _inputHandle = GetComponent<InputHandle>();
            _lightObject.SetActive(false);
        }

        public void Fire()
        {
            if (_inputHandle.GetFireInputDown())
            {
                _lightObject.SetActive(true);
            }

            if (_inputHandle.GetFireInputReleased())
            {
                _lightObject.SetActive(false);
            }
        }
    }
}