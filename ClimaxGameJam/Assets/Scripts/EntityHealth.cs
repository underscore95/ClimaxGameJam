using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private BarUI _healthBarOptional;

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
        }
    }

    public float MaxHealth => _maxHealth;

    private void Awake()
    {
        Debug.Assert(_maxHealth > 0);
    }

    private void OnEnable()
    {
        _health = _maxHealth;
    }
}