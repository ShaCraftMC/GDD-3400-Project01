using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float health, float maxHealth)
    {
        slider.value = health / maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
