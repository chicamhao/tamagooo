using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;

namespace Entity
{
    public class Door : Interactable
    {
        [SerializeField] GameObject _pivotObject;
        [SerializeField] float _openAngel = 90f;
        [SerializeField] float _speed = 2f;

        public bool IsOpened;
        private Quaternion _openRotation;
        private Quaternion _closeRotation;

        void Start()
        {
            _closeRotation = _pivotObject.transform.rotation;
            _openRotation = Quaternion.Euler(_pivotObject.transform.eulerAngles + new Vector3(0f, _openAngel, 0f));
        }

        void Update()
        {
            var rotation = IsOpened ? _openRotation : _closeRotation;
            if (!Mathf.Approximately(_pivotObject.transform.rotation.y, rotation.y))
            {
                _pivotObject.transform.rotation = Quaternion.Slerp(_pivotObject.transform.rotation, rotation, Time.deltaTime * _speed);
            }
        }

        public override void Interact()
        {
            IsOpened = !IsOpened;
        }
    }
}