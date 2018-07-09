using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private Transform weaponHolder;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;
    private WeaponSounds currentSounds;

    public bool isReloading = false;

	void Start () {
        EquipWeapon(primaryWeapon);
	}

    public PlayerWeapon GetCurrentWeapon() {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics() {
        return currentGraphics;
    }

    public WeaponSounds GetCurrentSounds() {
        return currentSounds;
    }

    void EquipWeapon(PlayerWeapon _weapon) {
        currentWeapon = _weapon;

        GameObject _weaponIns = Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder, false);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null) {
            Debug.LogError("No WeaponGraphics component on the weapon object: " + _weaponIns.name);
        }

        currentSounds = _weaponIns.GetComponent<WeaponSounds>();
        if (currentSounds == null) {
            Debug.LogError("No WeaponSounds component on the weapon object: " + _weaponIns.name);
        }

        if (isLocalPlayer) {
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public void Reload() {
        if (isReloading) {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine() {
        Debug.Log("Reloading...");

        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload() {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload() {
        currentSounds.PlayCockSound();
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null) {
            anim.SetTrigger("Reload");
        }
    }
}
