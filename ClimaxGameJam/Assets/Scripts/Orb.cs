using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Orb : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactInput;
    [SerializeField] private float _fadeTime = 1.0f;
    [SerializeField] private float _displayTime = 5f;
    [SerializeField] private float _interactDistance = 15f;
    [SerializeField] private CanvasGroup _winCanvas;
    [SerializeField] private Canvas _interactCanvas;
    [SerializeField] private TMP_Text _timerText;
    private PlayerController _player;
    private Camera _camera;

    private void Awake()
    {
        Debug.Assert(_winCanvas);
        Debug.Assert(_fadeTime > 0);
        _player = FindFirstObjectByType<PlayerController>();
        _interactInput.action.Enable();
        _camera = Camera.main;
        _interactCanvas.enabled = false;
    }

    private void Update()
    {
        bool pressed = _interactInput.action.WasPressedThisFrame();
        if (_player.Dead || _player.Won)
        {
            return;
        }

        _interactCanvas.enabled = Physics.Raycast(
            _camera.transform.position,
            _camera.transform.forward,
            out _,
            _interactDistance,
            1 << gameObject.layer
        );

        if (!pressed || !_interactCanvas.enabled) return;

        StartCoroutine(Win());
    }

    private IEnumerator Win()
    {
        _player.Won = true;
        _timerText.text = _timerText.text.Replace("00:00", FindFirstObjectByType<TimerUI>().GetTimeText());

        yield return CanvasUtils.FadeIn(_winCanvas, _fadeTime);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        yield return GameObjectUtils.DestroyAllExcept(gameObject, _camera.gameObject, _winCanvas.gameObject);

        yield return new WaitForSeconds(_displayTime);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}