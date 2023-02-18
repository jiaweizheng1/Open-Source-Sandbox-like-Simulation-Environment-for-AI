using UnityEngine;

    public class RotationOffset : MonoBehaviour
    {
        private Quaternion _initialRotation;
        public float Amplitude = 15;
        public Vector3 sinDirection = Vector3.up;
        public float Offset;
        public float Speed = 1;
        public Space space = Space.Self;

        private void Start()
        {
            _initialRotation = space == Space.Self ? transform.localRotation : transform.rotation;
        }

        private void Update()
        {
            float angle = Amplitude * Mathf.Sin(Time.time * Speed + Offset);
            Quaternion rotationOffset = Quaternion.AngleAxis(angle, sinDirection);

            if (space == Space.Self)
            {
                transform.localRotation = _initialRotation * rotationOffset;
            }
            else
            {
                transform.rotation = _initialRotation * rotationOffset;
            }
        }
    }
