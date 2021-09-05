using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{

    const string PLAYER_TAG = "Player";

    public Camera cam;

    public LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot : No camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.IsOn)
            return;

        if(currentWeapon.fireRate <=0)
        { 
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    private void Shoot()
    {
        if (IsLocalPlayer)
        {
            //We are shooting , call the OnShoot method on the server
            OnShootServerRpc();

            RaycastHit _hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
            {
                // We hit something
                if (_hit.collider.tag == PLAYER_TAG)
                {
                    Debug.LogWarning("Calling ServerRPC Player Shot" + IsServer);
                    PlayerShotServerRpc(_hit.collider.name, currentWeapon.damage);
                }

                // We hit something, call the hit method on the server
                OnHitServerRpc(_hit.point, _hit.normal);
            }
        }
    }

    // is called on the server when a player shoots
    [ServerRpc]
    void OnShootServerRpc()
    {
        DoShootEffectClientRpc();
    }

    //is called on all clients when we need to do an shoot effect;
    [ClientRpc]
    void DoShootEffectClientRpc()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //is called on the server when we hit something
    //Takes in position and normal of the surface
    [ServerRpc]
    void OnHitServerRpc(Vector3 _pos, Vector3 _normal)
    {
        DoHitEffectClientRpc(_pos, _normal);
    }

    //is called on all clients
    //here we can do cool effects
    [ClientRpc]
    void DoHitEffectClientRpc(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [ServerRpc]
    void PlayerShotServerRpc(string _playerID, int _damage)
    {
        Debug.Log(_playerID + "has been shot." + IsServer);

        Player _player = GameManager.GetPlayer(_playerID);
        _player.TakeDamageClientRpc(_damage);
    }
}