using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;

public class Ball : MonoBehaviour
{
    public int id { get; private set; }
    public int level { get; private set; }
    public Color color { get; private set; }
    public float power = 1.0f;
    public float radius = 1.5f;
    public bool interactive { get; set; }
    [SerializeField] Material[] _numberMaterialList;
    MMF_Player _effectPlayer;
    [SerializeField] MMF_Player _scorePlayer;
    ParticleSystem _explodeEffect;

    public class OnMergeArgs
    {
        public Ball ball;
        public int incomingBallLevel;
        public Vector3 position;
    }
    public event EventHandler<OnMergeArgs> OnMerge;

    public class OnExplodeArgs
    {
        public Ball ball;
        public Vector3 postiion;
    }
    public event EventHandler<OnExplodeArgs> OnExplode;

    public void SetExplosionEffect(ParticleSystem particleSystem)
    {
        _effectPlayer = GetComponent<MMF_Player>();
        if (_effectPlayer == null) return;
        var particles = _effectPlayer.GetFeedbackOfType<MMF_ParticlesInstantiation>();
        if (particles != null)
        {
            particles.ParticlesPrefab = particleSystem;
            _explodeEffect = particleSystem;
        }
    }

    public void SetMergeEffect(string value)
    {
        if (_scorePlayer == null) return;
        MMF_FloatingText text = _scorePlayer.GetFeedbackOfType<MMF_FloatingText>();
        if (text != null)
        {
            text.Value = value;
        }
    }

    public void PlayMergeEffect()
    {
        if (_scorePlayer == null) return;
        _scorePlayer.PlayFeedbacks();
    }

    public void ChangeColor(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = material;
        this.color = material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!interactive) return;
        Ball ball = collision.transform.GetComponent<Ball>();

        if (CanMerge(ball))
        {
            SetLevel(level + ball.level);
            if (level >= 4)
            {
                StartCoroutine(Explode());
            }
            OnMerge?.Invoke(this, new OnMergeArgs
            {
                ball = this,
                incomingBallLevel = ball.level,
            });
            Destroy(ball.gameObject);
        }
    }

    IEnumerator Explode()
    {
        Debug.Log(_effectPlayer.TotalDuration);
        _effectPlayer.PlayFeedbacks();

        yield return new WaitForSeconds(_effectPlayer.TotalDuration);

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPos, radius, 1.0F);
            }
            // TODO : check is Mosnter?
        }
        HapticController.fallbackPreset = HapticPatterns.PresetType.LightImpact;
        HapticPatterns.PlayEmphasis(1f, 0.2f);
        OnExplode?.Invoke(this, new OnExplodeArgs() { ball = this, postiion = transform.position });
        GetComponent<Renderer>().enabled = false;
        // Debug.Log(_explodeEffect.main.startLifetimeMultiplier);
        yield return new WaitForSeconds(Mathf.RoundToInt(1f));
        Destroy(this.gameObject);
    }

    bool CanMerge(Ball ball)
    {
        return ball != null
        && color != Color.gray
        && ball.interactive
        && ball.id > id
        && ball.color == color;
    }

    public void SetLevel(int value)
    {
        level = value;
        float scale = value * 0.5f + 0.5f;
        float mass = scale * 1;
        SetMass(mass);
        Renderer renderer = GetComponent<Renderer>();
        SetNumberMaterial(renderer, GetMaterialByLevel(level));
        transform.localScale = new Vector3(scale, scale, scale);
    }

    Material GetMaterialByLevel(int level)
    {
        if (level > _numberMaterialList.Length)
        {
            return null;
        }
        return _numberMaterialList[level - 1];
    }

    void SetNumberMaterial(Renderer renderer, Material newMaterial)
    {
        if (newMaterial == null) return;
        List<Material> list = new List<Material>(renderer.materials);
        if (list.Count > 1)
        {
            renderer.materials = new Material[]{
                renderer.materials[0],
                newMaterial
            };
            return;
        }
        list.Add(newMaterial);
        renderer.materials = list.ToArray();
    }

    void SetMass(float mass)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = mass;
        }
    }

    public void SetId(int value)
    {
        id = value;
    }

}
