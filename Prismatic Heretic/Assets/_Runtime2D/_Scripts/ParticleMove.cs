using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMove : MonoBehaviour
{
	public ParticleSystem p;
	public ParticleSystem.Particle[] particles;
	public GameObject target;
	public AudioSource source;
	public AudioClip heal;
	//public float affectDistance;
	//float sqrDist;
	Transform thisTransform;
	// Start is called before the first frame update
	void Start()
	{
		source=this.GetComponent<AudioSource>();
		p = GetComponent<ParticleSystem>();
		target = FindObjectOfType<Player>().gameObject;
		Destroy(this.gameObject, 5f);
		//sqrDist = affectDistance * affectDistance;
	}


	void Update()
	{
		
		particles = new ParticleSystem.Particle[p.particleCount];

		p.GetParticles(particles);

		for (int i = 0; i < particles.Length; i++)
		{
			//Old algo version
			//float force = (particles[i].startLifetime - particles[i].remainingLifetime) * (30 * Vector3.Distance(Target.position, particles[i].position));
			//particles[i].velocity = (Target.position - particles[i].position).normalized * force;
			particles[i].velocity = (target.transform.position - particles[i].position).normalized * 30;

			float dist = Vector3.Distance(particles[i].position, target.transform.position);
			if (dist<1f) {
				target.GetComponent<Player>().GainHealth(1);
				source.PlayOneShot(heal);
				particles[i].remainingLifetime = 0;
			}
		}

		p.SetParticles(particles, particles.Length);

	}
}