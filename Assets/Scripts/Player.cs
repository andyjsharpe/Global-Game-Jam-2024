using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField] private LayerMask layerMask;

    //[SerializeField] private GameObject hitSoundGameObject;
    
    [SerializeField] private GameObject shootSoundGameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Debug.Log(hit.transform.name);
            
            // Create shot noise from gun
            Instantiate(shootSoundGameObject, transform.position, Quaternion.identity);
            
            // Check if person hit, kill if so
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                hit.transform.GetComponent<Person>().GetShot();
            }
            
            // Create hit noise from hit
            //Instantiate(hitSoundGameObject, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
        }
    }
}
