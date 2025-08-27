using UnityEngine;
using System.Collections.Generic;
using Doulingo.UI;
using Coffee.UIExtensions;

public class PianoKeyEffectController : MonoBehaviour
{
    [SerializeField]
    private UIParticle particlePrefab;
    [SerializeField]
    private List<UIParticle> particlePools = new List<UIParticle>();


    public void ShowEffect(PianoKeyButton button, Color color)
    {
        var particle = particlePools.Find(x => !x.gameObject.activeSelf);
        if (particle == null)
        {
            particle = Instantiate(particlePrefab, transform.parent);
            particlePools.Add(particle);
        }
        particle.gameObject.SetActive(true);
        // Position effect at button location
        particle.transform.position = button.transform.position;
        var main = particle.GetComponentInChildren<ParticleSystem>().main;
        main.startColor = color;
    }
}