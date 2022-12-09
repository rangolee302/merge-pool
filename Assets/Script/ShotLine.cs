using UnityEngine;
using System;

namespace MergeBalls
{
    [RequireComponent(typeof(LineRenderer))]
    public class ShotLine : MonoBehaviour
    {
        LineRenderer _line;
        Vector3 _start;

        void Start()
        {
            _start = transform.position;
            _line = GetComponent<LineRenderer>();
            _line.positionCount = 2;
        }

        public void Set(Vector3 direction)
        {
            if (Physics.Raycast(_start, direction, out RaycastHit hit, float.MaxValue))
            {
                int segmentCount = Mathf.RoundToInt((hit.distance) / (direction.magnitude * 10f));
                segmentCount = Math.Max(1, segmentCount);
                _line.positionCount = segmentCount;

                _line.SetPositions(CreateSegment(_start, hit.point, segmentCount));
            }
        }

        Vector3[] CreateSegment(Vector3 start, Vector3 end, int segmentCount)
        {
            Vector3[] list = new Vector3[segmentCount];
            for (int i = 1; i <= segmentCount; i++)
            {
                list[i - 1] = Vector3.Lerp(start, end, ((float)i / (float)segmentCount));
            }
            return list;
        }

        public void Clear()
        {
            _line.positionCount = 0;
            _line.SetPositions(new Vector3[] { });
        }
    }
}
