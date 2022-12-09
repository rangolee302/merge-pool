using System;
using UnityEngine;

namespace MergeBalls
{

    public class Timer
    {
        float _limit;
        bool _isStarted = false;
        public float _time;
        public event EventHandler OnTimeEnd;
        public bool debug = false;
        public float time { 
            get => _time; 
            private set => _time = value; 
        }
        public float limit
        {
            get => _limit;
            private set => _limit = value;
        }

        public void SetLimit(float limit)
        {
            _limit = limit;
        }

        public void Tick(float time)
        {
            if (!_isStarted) return;
            _time += time;
            if (_time >= _limit)
            {
                _isStarted = false;
                OnTimeEnd?.Invoke(this, EventArgs.Empty);
            }
        }

        void p(string m)
        {
            if(debug)
                Debug.Log(m);
        }

        public void Start()
        {
            _isStarted = true;
        }

        public void Pause()
        {
            _isStarted = false;
        }

        public void ReStart()
        {
            _isStarted = true;
            _time = 0;
        }

    }

}