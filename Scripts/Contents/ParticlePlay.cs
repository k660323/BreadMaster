using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    ParticleSystem particleSystem;

    [SerializeField]
    float additionalLifeTime;

    [SerializeField]
    bool isPlayAwake = true;

    private void Awake()
    {
        TryGetComponent(out particleSystem);
        if(particleSystem && isPlayAwake)
        {
            PlayParticle();
        }
    
    }

    public void PlayParticle()
    {
        particleSystem.Play();

        Invoke(nameof(DestroyParticle), particleSystem.main.duration + additionalLifeTime);

    }

    void DestroyParticle()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
