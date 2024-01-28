using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject hitSoundGameObject;
    
    [SerializeField] private GameObject shootSoundGameObject;

    private Animator _anim;
    private static readonly int Scoped = Animator.StringToHash("Scoped");

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _anim = GetComponent<Animator>();
    }

    public void OnZoom()
    {
        _anim.SetBool(Scoped, !_anim.GetBool(Scoped));
    }

    public void OnFire()
    {
        Shoot();
    }

    public void OnLook(InputValue inputValue)
    {
        var mouseMovement = inputValue.Get<Vector2>() / 25;
        
        // Apply x rotation
        transform.Rotate(Vector3.up, mouseMovement.x);
        // Apply y rotation
        Camera.main.transform.Rotate(-Vector3.right, mouseMovement.y, Space.Self);
    }

    private void Shoot()
    {
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, 256, layerMask))
        {
           
            // Create shot noise from gun
            Instantiate(shootSoundGameObject, transform.position, Quaternion.identity);
            
            // Check if person hit, kill if so
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                if (hit.transform.TryGetComponent<BulletHitCallback>(out var callback))
                {
                    // Create hit noise from hit
                    Instantiate(hitSoundGameObject, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                    callback.GetShot(hit.point);
                }
            }
            
            
        }
    }
}
