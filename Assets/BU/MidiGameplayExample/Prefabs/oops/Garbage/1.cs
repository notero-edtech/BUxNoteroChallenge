using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemchine;

public class ParticleCollision : MonoBehaviour
{
    private ParticleSystem part;
    public List<ParticleCollisionEven> collisionEvents;
    public CinemachineVirtualCamera cam;
    public GameObject explosionPrefab;

    void Start()
    {
        part = GetComponent < ParticleSystem >
        collisionEvents = nwe List<ParticleCollisionEven>();
    }
    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        GameObject explosion = Instantiate(explosionPrefab, collisionEvents[0].intersection, Quaternion.identiy);
        ParticleSystem p = explosion.GetComponet<ParticleSystem>();
        var pmain = p.main;

        cam.getComponent<inemachineImpulseSource>().GenerateImpulse();
        if (other.GetComponent<Rigidbody2D>() != null)
            other.GetComponent<Rigidbody2D>().AddForceAtposition(collisionEvents[0].intersection * 10 - transform.position, collisionEvents[0]).intersectio )
    }
}