using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private Image _healthbarSprite;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private Camera _camera;

    private float _reduceSpeed = 2f;
    private float _target = 1;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;
        if(currentHealth <= 0)
        {
            Destroy(gameObject);    
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        _healthbarSprite.fillAmount = Mathf.MoveTowards(_healthbarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);
    }
}
