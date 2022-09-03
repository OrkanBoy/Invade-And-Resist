using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
    public GameObject m_Prefab;

    private ParticleSystem m_ParticleSystem;
    private List<GameObject> m_Instances = new List<GameObject>();
    private ParticleSystem.Particle[] m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
    }

    // Update is called once per frame

    [Range(0.1f, 10.0f)]
    public float UPDATE_INTERVAL = 2.0f;
    float sinceLastUpdate = 0.0f;
    void LateUpdate()
    {
        new ParticleSystem.Particle();
        int count = m_ParticleSystem.GetParticles(m_Particles);

        while (m_Instances.Count < count)
            m_Instances.Add(CreateInstance());

        if (sinceLastUpdate >= UPDATE_INTERVAL && m_Prefab.GetComponent<Light2D>() != m_Instances[0].GetComponent<Light2D>())
        {
            for (int i = 0; i < m_Instances.Count; i++)
            {
                Destroy(m_Instances[i]);
                m_Instances[i] = CreateInstance();
            }
            sinceLastUpdate = 0.0f;
        }

        bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        for (int i = 0; i < m_Instances.Count; i++)
            if (i < count)
            {
                if (worldSpace)
                    m_Instances[i].transform.position = m_Particles[i].position;
                else
                    m_Instances[i].transform.localPosition = m_Particles[i].position;
                m_Instances[i].gameObject.SetActive(true);
            }
            else
                m_Instances[i].gameObject.SetActive(false);

        sinceLastUpdate += Time.deltaTime;
    }

    GameObject CreateInstance() 
    {
        GameObject o = Instantiate(m_Prefab, m_ParticleSystem.transform);
        o.transform.eulerAngles = m_ParticleSystem.transform.eulerAngles;
        return o;
    }
}
