using System;
using Managers;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10.0f;
    public bool isPlayerBullet = true;
    public int Damage { get; set; } = 1;

    private MeshRenderer _meshRenderer;

    private Action<Projectile> _destroyAction;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        transform.Translate(isPlayerBullet
            ? Vector3.up * (speed * Time.deltaTime)
            : Vector3.down * (speed * Time.deltaTime));
    }

    /// <summary>
    /// Method used to set projectile material
    /// </summary>
    /// <param name="material">material to set</param>
    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }

    public void SetDestroyAction(Action<Projectile> action)
    {
        _destroyAction = action;
    }

    public void Destroy()
    {
        _destroyAction.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ProjectileCatch")) Destroy();
    }
}