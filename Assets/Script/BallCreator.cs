using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MergeBalls
{

    public class BallCreator : MonoBehaviour
    {
        [SerializeField] Ball ballPrefab;
        [SerializeField] GameObject _pad;
        [SerializeField] float damp = 0.75f;
        [SerializeField] float forceMulitplyer = 800;
        [SerializeField] List<Material> _materialList;
        [SerializeField] float power = 1.5f;
        [SerializeField] float radius = 2f;
        [SerializeField] int _initialNum = 18;
        [SerializeField] Transform _plane;
        [SerializeField] ParticleSystem _explodeParticle;

        public int ballCount { get => _queue.Count; }
        Queue<Ball> _queue;
        GameObject _back;
        Ball _currentBall = null;
        Timer _loadingTimer;
        float _loadTime = 1f;
        int _idPool = 0;

        public class OnFireBallArgs : EventArgs
        {
            public Ball ball;
            public int ballCount;
        }
        public event EventHandler<OnFireBallArgs> OnFireBall;

        public class OnAddBallArgs : EventArgs
        {
            public Queue<Ball> queue;
        }
        public event EventHandler<OnAddBallArgs> OnAddBall;
        public event EventHandler OnDequeueBall;

        public void Initiallize()
        {

            _loadingTimer = new Timer();
            _loadingTimer.SetLimit(_loadTime);
            _loadingTimer.OnTimeEnd += OnTimeEnd;
            _loadingTimer.Start();
            _back = new GameObject("BallQueue");
            _queue = new Queue<Ball>();
            _explodeParticle.collision.AddPlane(_plane);
            AddBall(_initialNum);
        }

        void OnDisable()
        {
            _loadingTimer.OnTimeEnd -= OnTimeEnd;
        }

        void Update()
        {
            _loadingTimer.Tick(Time.deltaTime);
        }

        public void AddBall(int num)
        {
            Queue<Ball> updateQueue = new Queue<Ball>();
            
            for (int i = 0; i < num; i++)
            {
                Ball ball = Instantiate(ballPrefab, _back.transform);
                ball.gameObject.SetActive(false);
                ball.ChangeColor(_materialList[Random.Range(0, _materialList.Count)]);
                ball.SetLevel(
                    ball.color == Color.gray ?
                    2: Random.Range(1, 2)
                );
                ball.power = power;
                ball.radius = radius;
                ball.SetExplosionEffect(_explodeParticle);
                _queue.Enqueue(ball);
                updateQueue.Enqueue(ball);
            }
            OnAddBall?.Invoke(this, new OnAddBallArgs { queue = updateQueue });
        }
        
        void OnTimeEnd(object sender, EventArgs args)
        {
            PrepareToFire();
            _loadingTimer.ReStart();
        }

        void PrepareToFire()
        {
            if (_queue.Count <= 0 || _currentBall != null)
            {
                return;
            }

            Ball ball = _queue.Dequeue();
            ball.interactive = false;
            ball.gameObject.SetActive(true);
            ball.gameObject.name = $"Ball-{_queue.Count}";
            ball.SetId(_idPool++);
            ball.transform.SetParent(_pad.transform);
            ball.transform.localPosition = Vector3.zero;
            _currentBall = ball;
            OnDequeueBall?.Invoke(this, EventArgs.Empty);
        }

        public void FireBall(Vector3 direction)
        {
            if (_currentBall == null)
            {
                return;
            }
            Rigidbody rb = _currentBall.gameObject.AddComponent<Rigidbody>();
            rb.angularDrag = 1f;
            Vector3 force = direction * direction.magnitude;
            _currentBall.interactive = true;
            rb.AddForce(force * forceMulitplyer);
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.drag = damp;
            OnFireBall?.Invoke(this, new OnFireBallArgs { 
                ball = _currentBall,
                ballCount = _queue.Count
             });
            _currentBall = null;
        }

        public List<Ball> GetRemainingBalls()
        {
            return new List<Ball>(_queue);
        }
    }
}