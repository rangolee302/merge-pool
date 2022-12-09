using UnityEngine;
using UnityEngine.InputSystem;
using Lofelt.NiceVibrations;

namespace MergeBalls
{
    public class BallControl : MonoBehaviour
    {
        [SerializeField] BallCreator _ballCreator;
        [SerializeField] ShotLine _line;
        [SerializeField] float _minDrag;
        [SerializeField] float _maxDrag;
        Vector2 _startPosition;
        Vector2 _position;
        bool isAiming = false;
        float _maxMagnitude = 0.3f;
        float _minMagnitude = 0.1f;
        float _magnitudeLevelThreshold = 0.04f;
        float _prevMagnitudeLevel = 0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Fire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _startPosition = _position;
                isAiming = true;
            }
        }

        public void Release(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 value = NormalizeDirectionVector();
                value = Clamp(value);
                HapticController.fallbackPreset = HapticPatterns.PresetType.RigidImpact;
                HapticPatterns.PlayEmphasis(0.85f, 0.05f);
                _ballCreator.FireBall(
                    new Vector3(
                        -value.x,
                        0,
                        -value.y
                    )
                );
                isAiming = false;
                _line.Clear();
            }
        }

        int GetMagnitudeLevel(float m)
        {
            return Mathf.RoundToInt((m - _minMagnitude) / _magnitudeLevelThreshold);
        }

        Vector2 NormalizeDirectionVector()
        {
            Vector2 value = _position - _startPosition;

            // scale down the vector by screen width
            value /= new Vector2(Screen.width, Screen.width);
            return value;
        }

        Vector2 Clamp(Vector2 vector)
        {
            Vector2 max = vector.normalized * _maxMagnitude;
            Vector2 min = vector.normalized * _minMagnitude;
            if (vector.magnitude >= max.magnitude) return max;
            if (vector.magnitude <= min.magnitude) return min;
            return vector;
        }

        public void DragAndFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _position = context.ReadValue<Vector2>();
                if (isAiming)
                {
                    Vector2 value = NormalizeDirectionVector();
                    value = Clamp(value);
                    int level = GetMagnitudeLevel(value.magnitude);
                    if(level != _prevMagnitudeLevel)
                    {
                        HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
                        HapticPatterns.PlayEmphasis(0.2f, 0.05f);
                    }
                    _prevMagnitudeLevel = level;
                    _line.Set(new Vector3(
                        -value.x,
                        0,
                        -value.y
                    ));
                }
            }
        }
    }
}
