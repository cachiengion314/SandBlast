using UnityEngine;

public class YellowBloodExplosion2D : MonoBehaviour
{
    [SerializeField] ParticleSystem particle1;
    [SerializeField] ParticleSystem particle2;
    [SerializeField] ParticleSystem particle3;

    public void SetColor(Color color)
    {
        var main1 = particle1.main;
        main1.startColor = color;

        var main2 = particle2.main;
        main2.startColor = color;

        var main3 = particle3.main;
        main3.startColor = color;
    }
}
