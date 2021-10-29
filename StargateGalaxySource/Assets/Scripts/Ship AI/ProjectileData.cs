using UnityEngine;
using System.Collections;

public class ProjectileData : MonoBehaviour {

    public DataManager.WeaponData weaponData;
    public bool firedByPlayer;
    public GameObject target;
    public Vector3 velocity;
	public AudioClip sound;
    private float life;
    GameObject parent;
    bool configured = false;

	// Use this for initialization
	void Start () {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject obj in objs)
        {
            if (obj == gameObject) continue;
            Physics.IgnoreCollision(obj.collider, collider);
        }
	}
	
	// Update is called once per frame
	void Update () {
        life -= Time.deltaTime;
        if (life <= 0) Destroy(gameObject);

        if (weaponData.effectType == DataManager.WeaponEffectType.Rocket)
        {
            setupVelocity();
        }
	}

    void OnTriggerStay(Collider other)
    {
        if (!configured) return;
        
        GameObject obj = other.gameObject;
        if (obj == parent) return;
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Ally" && other.gameObject.tag != "Enemy")
        {
            DestroyObject(gameObject);
            return;
        }

        ShipAI shipData = obj.GetComponent<ShipAI>();
        shipData.damageShip(weaponData, firedByPlayer);

        DestroyObject(gameObject);
    }

    public void configureProjectile(GameObject parent, DataManager.WeaponData weaponData, GameObject target, bool firedByPlayer)
    {
        if (target == null)
        {
            DestroyObject(gameObject);
            return;
        }
        
        this.weaponData = weaponData;
        this.target = target;
        this.firedByPlayer = firedByPlayer;
        life = 10.0f;
        this.parent = parent;

        transform.LookAt(target.transform.position);
        setupVelocity();
		audio.PlayOneShot(sound);
        configured = true;
    }

    private void setupVelocity()
    {
        if (target == null) return;
        transform.LookAt(target.transform.position);
        velocity = target.transform.position - transform.position;
        velocity.Normalize();
        velocity *= 40; 
        rigidbody.velocity = velocity;
    }

}
