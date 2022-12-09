using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MoreMountains.Feedbacks;

namespace MergeBalls
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] BallCreator _ballCreator;
        [SerializeField] ScoreUI _scoreUI;
        [SerializeField] BallQueueUI _queueUI;
        [SerializeField] TimerUI _timerUI;
        [SerializeField] TMP_Text _multiplierText;
        [SerializeField] TMP_Text _gameOver;
        [SerializeField] MMF_Player _addBallEffects;
        [SerializeField] Canvas canvas;
        [SerializeField] Camera _uiCam;
        [SerializeField] int _queueIncrement = 2;
        [SerializeField] float _combiTime = 2f;
        [SerializeField] float _noBallTime = 3f;
        List<Ball> _balls;
        Timer _timer;
        Timer _comboTimer;
        Timer _noBallTimer;
        int _score;
        int _multiplier;

        void Start()
        {
            Application.targetFrameRate = 60;
            Physics.gravity *= 2;
            _score = 0;
            _multiplier = 1;
            _balls = new List<Ball>();
            _timer = new Timer();
            _timer.SetLimit(60f);
            _comboTimer = new Timer();
            _comboTimer.SetLimit(_combiTime);
            _noBallTimer = new Timer();
            _noBallTimer.debug = true;
            _noBallTimer.SetLimit(_noBallTime);
            _timer.Start();
            _timerUI.SetTimer(_timer);

            _ballCreator.OnFireBall += OnFireBall;
            _ballCreator.OnAddBall += OnAddBall;
            _ballCreator.OnDequeueBall += OnDequeueBall;
            _timer.OnTimeEnd += OnTimeEnd;
            _comboTimer.OnTimeEnd += OnComboTimeEnd;
            _noBallTimer.OnTimeEnd += OnNoBall;

            _ballCreator.Initiallize();
        }

        void OnDestory()
        {
            _ballCreator.OnFireBall -= OnFireBall;
            _ballCreator.OnAddBall -= OnAddBall;
            _ballCreator.OnDequeueBall -= OnDequeueBall;
            _timer.OnTimeEnd -= OnTimeEnd;
            _comboTimer.OnTimeEnd -= OnComboTimeEnd;
            _noBallTimer.OnTimeEnd -= OnNoBall;
        }

        void Update()
        {
            _timer.Tick(Time.deltaTime);
            _comboTimer.Tick(Time.deltaTime);
            _noBallTimer.Tick(Time.deltaTime);
            _scoreUI.SetScore(_score);
            _multiplierText.text = _multiplier <= 1 ? "" : $"x{_multiplier.ToString()}";
        }

        void OnMerge(object sender, Ball.OnMergeArgs args)
        {
            _comboTimer.ReStart();
            int increment = _multiplier * (args.ball.level); 
            AddScore(increment);
            args.ball.SetMergeEffect(increment.ToString());
            args.ball.PlayMergeEffect();
            if (_ballCreator.ballCount <= 0)
            {
                _noBallTimer.ReStart();
            }
        }

        void AddScore(int increment)
        {
            _score += increment;
            _multiplier++;
        }

        void OnTimeEnd(object sender, EventArgs args)
        {
            GameOver();
        }

        void OnComboTimeEnd(object sender, EventArgs args)
        {
            _multiplier = 1;
        }

        void OnNoBall(object sender, EventArgs args)
        {
            GameOver();
        }

        void GameOver()
        {
            _gameOver.gameObject.SetActive(true);
        }

        public void ReStartGame()
        {
            SceneManager.LoadScene(0);
        }

        void OnAddBall(object sender, BallCreator.OnAddBallArgs args)
        {
            _queueUI.UpdateQueue(args.queue);
            // _noBallTimer.Pause();
        }

        void OnDequeueBall(object sender, EventArgs args)
        {
            _queueUI.RemoveBall();
        }

        void OnFireBall(object sender, BallCreator.OnFireBallArgs args)
        {
            if (args.ball != null)
            {
                _balls.Add(args.ball);
                args.ball.OnMerge += OnMerge;
                args.ball.OnExplode += OnExplode;
            }

            if (args.ballCount <= 0)
            {
                _noBallTimer.ReStart();
            }
        }

        void OnExplode(object sender, Ball.OnExplodeArgs args)
        {
            Vector2 sp = RectTransformUtility.WorldToScreenPoint(Camera.main, args.postiion);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), sp, _uiCam, out Vector2 point
            );
            RectTransform rect = _addBallEffects.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = point;
                _addBallEffects.PlayFeedbacks();
            }

            // TODO : check monster, adding score, respawn mosnter

            _ballCreator.AddBall(_queueIncrement);
            if (args.ball != null)
            {
                args.ball.OnExplode -= OnExplode;
                args.ball.OnMerge -= OnMerge;
            }
        }
    }
}
