using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    /*NetworkVariable<bool> _isDead = new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, false);*/
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    /*int _currentHealth;*/
    NetworkVariable<int> _currentHealth = new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    public int currentHealth {
        get { return _currentHealth.Value; }
        set { _currentHealth.Value = value; }
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public GameObject[] disableGameObjectsOnDeath;

    public GameObject deathEffect;

    public GameObject spawnEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if(IsLocalPlayer)
        {
            // Switch camera
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        if(IsOwner)
            BroadCastNewPlayerSetupServerRpc();
    }

    [ServerRpc]
    private void BroadCastNewPlayerSetupServerRpc()
    {
        Debug.LogWarning("ServerRPC Setup Player");
        SetupPlayerOnAllClientsClientRpc();
    }

    [ClientRpc]
    private void SetupPlayerOnAllClientsClientRpc()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        Debug.LogWarning("Setup player client rpc");
        SetDefaults();
    }

    private void Update()
    {
       /* Debug.LogWarning(currentHealth + " " + IsLocalPlayer);*/

        if (!IsLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            TakeDamageClientRpc(999999);
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int _amount)
    {
        if (isDead)
            return;

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Disable some components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable GameOjbects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // Disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //Spawn a death effect
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        // Switch camera
        if(IsLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        // Call Respawn method
        StartCoroutine("Respawn");
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = GameManager.instance.GetSpawnPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        // Enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // Enable the gameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        // Create Spawn Effect
        GameObject _gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }
}
