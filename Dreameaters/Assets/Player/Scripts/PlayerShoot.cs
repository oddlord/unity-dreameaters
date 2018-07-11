using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start() {
        if (cam == null) {
            Debug.Log("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update() {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn) {
            return;
        }

        if (currentWeapon.bullets < currentWeapon.maxBullets) {
            if (Input.GetButtonDown("Reload")) {
                weaponManager.Reload();
                return;
            }
        }

        if (currentWeapon.fireRate <= 0) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        } else {
            if (Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            }
        }
    }

    // Is called on the Server when the player shoots
    [Command]
    void CmdOnShoot() {
        RpcDoShootEffect();
    }

    // Is called on all clients when we need to do
    // a shoot effect
    [ClientRpc]
    void RpcDoShootEffect() {
        StartCoroutine(DoShootEffectCoroutine());
    }

    IEnumerator DoShootEffectCoroutine() {
        weaponManager.GetCurrentSounds().PlayShotSound();

        WeaponGraphics weaponGraphics = weaponManager.GetCurrentGraphics();
        weaponGraphics.muzzleFlash.Play();
        weaponGraphics.laserRendered.enabled = true;
        yield return new WaitForSeconds(weaponGraphics.laserTime);
        weaponGraphics.laserRendered.enabled = false;
    }

    // Is called on the Server when we hit something
    // Takes in the hit point and the normal of the surface
    [Command]
    void CmdOnHit (Vector3 _pos, Vector3 _normal) {
        RpcDoHitEffect(_pos, _normal);
    }

    // Is called on all clients
    // Here we can spawn in cool effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal) {
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client] // A Client is a method that is executed only on the client (needed???)
    void Shoot() {
        
        if (!isLocalPlayer || weaponManager.isReloading) {
            return;
        }

        if (currentWeapon.bullets <= 0) {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        Debug.Log("Remaining bullets: " + currentWeapon.bullets);

        // We are shooting, call the OnShoot method on the Server
        CmdOnShoot();

        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)) {
            if(_hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            // We hit something, call the OnHit method on Server
            CmdOnHit(_hit.point, _hit.normal);
        }

        if (currentWeapon.bullets <= 0) {
            weaponManager.Reload();
        }
    }

    [Command] // A Command is a method called from a client that is executed on the server
    void CmdPlayerShot(string _playerID, int _damage, string _sourceID) {
        Player _player = GameManager.GetPlayer(_playerID);
        Player _source = GameManager.GetPlayer(_sourceID);

        Debug.Log(_source.username + " shot " + _player.username);

        _player.RpcTakeDamage(_damage, _sourceID);
    }
}
