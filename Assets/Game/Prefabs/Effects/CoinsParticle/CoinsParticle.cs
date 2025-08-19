using System;
using System.Collections;
using Coffee.UIExtensions;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CoinsParticle : MonoBehaviour
{
  [Header("Element")]
  [SerializeField] ParticleSystem coinPS;
  [SerializeField] UIParticle uiParticle;

  [Header("Setting")]
  [SerializeField] private float moveSpeed;
  private int coinAmount;

  public bool IsPlaying()
  {
    if (coinPS.isPlaying) return true;
    return false;
  }

  public void EmissingTo(float3 fromPos, float3 targetPos, int amount, Action onCompleted = null)
  {
    if (coinPS.isPlaying) return;
    transform.position = fromPos;
    uiParticle.Play();

    ParticleSystem.Burst burst = coinPS.emission.GetBurst(0);
    coinAmount = amount;
    burst.count = amount;
    coinPS.emission.SetBurst(0, burst);
    coinPS.Clear(true);
    coinPS.Play(true);

    ParticleSystem.MainModule main = coinPS.main;
    StartCoroutine(PlayCoinParticlesCoroutine(targetPos, onCompleted));
  }

  IEnumerator PlayCoinParticlesCoroutine(float3 targetPos, Action onCompleted = null)
  {
    yield return new WaitForSeconds(.58f);
    ParticleSystem.MainModule main = coinPS.main;
    main.gravityModifier = 0;

    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[coinAmount];

    while (coinPS.isPlaying)
    {
      coinPS.GetParticles(particles);

      for (int i = 0; i < particles.Length; ++i)
      {
        if (((Vector3)targetPos - particles[i].position).sqrMagnitude < .09f)
        {
          // Instantiate(
          //     hitMiscEfx, particles[i].position + -Vector3.forward, Quaternion.identity
          // ).Play();
          particles[i].remainingLifetime = 0f;
          continue;
        }
        var _speedFactor = (i + 1) % 12;
        if (_speedFactor < 3) _speedFactor = 3;
        var _moveSpeed = _speedFactor * moveSpeed;
        MoveEachFrameTo(ref particles[i], targetPos, _moveSpeed);
      }

      coinPS.SetParticles(particles);
      yield return null; // one frame have passed
    }
    onCompleted?.Invoke();
  }

  void MoveEachFrameTo(ref ParticleSystem.Particle particle, in Vector3 targetPos, float speed)
  {
    particle.position = Vector3.MoveTowards(
        particle.position, targetPos, speed * Time.deltaTime
    );
  }
}
