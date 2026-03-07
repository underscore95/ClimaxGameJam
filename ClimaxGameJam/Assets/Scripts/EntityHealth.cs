using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private BarUI _healthBarOptional;
    [SerializeField] private bool _destroyOnDeath;

    public Action OnDeath { get; set; }

    private float _health;

    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _maxHealth);
            if (_healthBarOptional)
            {
                _healthBarOptional.SetProgress(_health / _maxHealth);
            }

            if (_health <= 0) OnDeath.Invoke();
            print("Damaged: " + _health + " / " + _maxHealth);
        }
    }

    private void Awake()
    {
        Debug.Assert(_maxHealth > 0);
        _health = _maxHealth;
        if (_destroyOnDeath) OnDeath += () => Destroy(gameObject);
    }
}