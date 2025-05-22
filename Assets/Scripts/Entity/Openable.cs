using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class Openable : Interactable
    {
        [SerializeField] List<PivotObject> _pivotObjects;
        [SerializeField] float _speed = 2f;

        public bool IsOpened;

        private void Start()
        {
            foreach (var obj in _pivotObjects)
            {
                obj.Start();
            }
        }
        void Update()
        {
            foreach (var obj in _pivotObjects)
            {
                var rotation = IsOpened ? obj.OpenQuaternion : obj.CloseQuaternion;
                obj.Object.transform.rotation = Quaternion.Slerp(obj.Object.transform.rotation, rotation, Time.deltaTime * _speed);

                var position = IsOpened ? obj.OpenPosition : obj.ClosePosition;
                obj.Object.transform.position = Vector3.MoveTowards(obj.Object.transform.position, position, Time.deltaTime * _speed);
            }
        }     

        public override void Interact()
        {
            IsOpened = !IsOpened;
        }   
    }

    [Serializable]
    class PivotObject
    {
        public GameObject Object;
        public Vector3 ToRotate;
        public Vector3 ToPosition;

        public Vector3 ClosePosition => _closePosition;
        private Vector3 _closePosition;
        public Vector3 OpenPosition => _openPosition;
        private Vector3 _openPosition;

        public Quaternion CloseQuaternion => _closeQuaternion;
        private Quaternion _closeQuaternion;
        public Quaternion OpenQuaternion => _openQuaternion;
        private Quaternion _openQuaternion;



        public void Start()
        {
            _openPosition = Object.transform.position + ToPosition;
            _closePosition = Object.transform.position;

            _openQuaternion = Quaternion.Euler(Object.transform.eulerAngles + ToRotate);
            _closeQuaternion = Object.transform.rotation;
        }

    }

}