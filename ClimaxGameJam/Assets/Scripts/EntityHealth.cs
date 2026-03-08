using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private BarUI _healthBarOptional;
    [SerializeField] private float _secondsPerRegen = 30;
    [SerializeField] private int _amountPerRegen = 0;

    public Action OnDeath { get; set; }

    private float _health;
    private Coroutine _regenCoroutine;

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
        Debug.Assert(_amountPerRegen >= 0);
    }

    private void OnEnable()
    {
        _health = _maxHealth;
        _regenCoroutine = StartCoroutine(StartRegen());
    }

    private void OnDisable()
    {
        StopCoroutine(_regenCoroutine);
    }

    private IEnumerator StartRegen()
    {
        yield return new WaitForSeconds(_secondsPerRegen);
        if (_health > 0) Health += _amountPerRegen;
        _regenCoroutine = StartCoroutine(StartRegen());
    }
}