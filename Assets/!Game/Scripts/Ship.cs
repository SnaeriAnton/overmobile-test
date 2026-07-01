using UnityEngine;

namespace Game
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private float _verticalAmplitude = 0.25f;
        [SerializeField] private float _rotationAmplitude = 4f;
        [SerializeField] private float _animationSpeed = 1f;

        private Vector3 _startLocalPosition;
        private Quaternion _startLocalRotation;

        private void Awake()
        {
            _startLocalPosition = transform.localPosition;
            _startLocalRotation = transform.localRotation;
        }

        private void Update()
        {
            float wave = Time.time * _animationSpeed;
            float verticalOffset = Mathf.Sin(wave) * _verticalAmplitude;
            float zRotation = Mathf.Sin(wave) * _rotationAmplitude;

            transform.localPosition = _startLocalPosition + Vector3.up * verticalOffset;
            transform.localRotation = _startLocalRotation * Quaternion.Euler(0f, 0f, zRotation);
        }
    }
}
