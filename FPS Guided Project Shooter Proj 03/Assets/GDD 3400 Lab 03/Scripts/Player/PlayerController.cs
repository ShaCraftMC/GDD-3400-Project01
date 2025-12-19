using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEditor.Rendering.Analytics;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Cinemachine3rdPersonAim _ThirdPersonAim;
    [SerializeField] ShootMechanic _ShootMechanic;

    //Variables for player
    public int health = 500;
    public int maxHealth = 500;
    public int deathCount = 0;

    public Vector3 targetPosition;

    //Life panel
    //[SerializeField] public GameObject lifePanel;
    [SerializeField] public GameObject lifeOne;
    [SerializeField] public GameObject lifeTwo;
    [SerializeField] public GameObject lifeThree;

    //Healthbar support
    [SerializeField] HealthBar healthBar;

    public void Awake()
    {
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    public void Update()
    {
        if (_ThirdPersonAim == null || _ShootMechanic == null) return;
        
        // Pass in the aim target point to the shoot mechanic
        _ShootMechanic.AimTargetPoint = _ThirdPersonAim.AimTarget;

        // Start and stop the shoot action based on the shoot action input
        if (Mouse.current.leftButton.wasPressedThisFrame) PerformShoot();
    }

    private void PerformShoot()
    {
        Debug.Log("I Shot");
        // Perform the shoot action
        _ShootMechanic.PerformShoot(_ShootMechanic.AimTargetPoint);

        // Look at the aim target, this helps make the character look more natural when shooting
        this.transform.LookAt(_ThirdPersonAim.AimTarget);
        this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        health = health - damage;
        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0 && deathCount == 0)
        {
            targetPosition = new Vector3(19, 0, -0.57f);
            gameObject.transform.position = targetPosition;
            health = 500;
            deathCount++;
            healthBar.UpdateHealthBar(health, maxHealth);
            lifeThree.SetActive(false);
        }
        else if (health <= 0 && deathCount == 1)
        {
            targetPosition = new Vector3(19, 0, -0.57f);
            gameObject.transform.position = targetPosition;
            health = 500;
            deathCount++;
            healthBar.UpdateHealthBar(health, maxHealth);
            lifeTwo.SetActive(false);
        }
        else if (health <= 0 && deathCount == 2)
        {
            targetPosition = new Vector3(19, 0, -0.57f);
            gameObject.transform.position = targetPosition;
            health = 500;
            deathCount++;
            healthBar.UpdateHealthBar(health, maxHealth);
            lifeOne.SetActive(false);
        }
        else if (health <= 0 && deathCount >= 3)
        {
            SceneManager.LoadScene("GameOver");
        }

    }
}
