using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    public class Openable : Interactable
    {
        [SerializeField] Type _type;
        [SerializeField] bool _ignoreColliderWhenOpening = true;
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
                if (_type == Type.Rotate)
                {
                    var rotation = IsOpened ? obj.OpenQuaternion : obj.CloseQuaternion;

                    var completed = Quaternion.Angle(obj.Object.transform.rotation, rotation) < 1f;
                    if (!completed)
                    {
                        obj.Object.transform.rotation = Quaternion.Slerp(obj.Object.transform.rotation, rotation, Time.deltaTime * _speed);
                    }

                    if (_ignoreColliderWhenOpening)
                    {
                        obj.Collider.enabled = !IsOpened && completed;
                    }
                }
                else
                {
                    var position = IsOpened ? obj.OpenPosition : obj.ClosePosition;
                    var completed = Vector3.Distance(obj.Object.transform.position, position) < 0.01f;

                    if (!completed)
                    {
                        obj.Object.transform.position = Vector3.MoveTowards(obj.Object.transform.position, position, Time.deltaTime * _speed);
                    }

                    if (_ignoreColliderWhenOpening)
                    {
                        obj.Collider.enabled = _ignoreColliderWhenOpening && !IsOpened && completed;
                    }
                }
            }
        }     

        public override void Interact()
        {
            IsOpened = !IsOpened;
        }   
    }

    enum Type
    {
        Rotate,
        Position
    }

    [Serializable]
    class PivotObject
    {
        public GameObject Object;
        public Collider Collider;

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

            if (Object.TryGetComponent<Collider>(out var c))
            {
                Collider = c;
            }
            else 
            {
                Collider = Object.GetComponentInChildren<Collider>();
            }
        }

    }

}