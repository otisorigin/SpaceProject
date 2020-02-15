using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthSystem : MonoBehaviour
{
    [Inject] private GameManager _manager;
    private int _maxHealth;
    private int _currentHealth;
    private Text _healthCounter;

    //--------------------Events---------------------------------
    public event Action<float> OnHealthPctChanged = delegate { };

    private void Awake()
    {
        _manager.OnNextTurn += NextTurn;
    }

    public void InitHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
        InitHealthCounter();
        SetHealthBarColor();
    }

    private void Update()
    {
        //------------------------------
        if (Input.GetKeyDown(KeyCode.Minus))
            ModifyHealth(-10);
        //---------------------------
        HealthBarPosition();
    }

    private void InitHealthCounter()
    {
        _healthCounter = transform.parent.GetComponentInChildren<Text>();
        UpdateHealthCounter();
    }

    private void UpdateHealthCounter()
    {
        _healthCounter.text = _currentHealth + "/" + _maxHealth;
    }

    private void ModifyHealth(int amount)
    {
        var updatedHealth = _currentHealth + amount;
        if (updatedHealth >= 0 && updatedHealth <= _maxHealth)
        {
            _currentHealth = updatedHealth;
            float currentHealthPct = _currentHealth / (float) _maxHealth;
            OnHealthPctChanged(currentHealthPct);
            UpdateHealthCounter();
        }
    }

    private void SetHealthBarColor()
    {
        transform.parent.GetComponentsInChildren<Image>()[1].color =
            _manager.IsUnitOfCurrentPlayer(transform.parent.GetComponent<Unit>())
                ? Constants.Colors.LightGreen
                : Constants.Colors.Red;
    }

    private void HealthBarPosition()
    {
        var healthbar = transform.parent.GetComponentInChildren<Healthbar>();
        var unitRotation = transform.parent.GetChild(0).transform.rotation;
        if (unitRotation.z <= 1.0f && unitRotation.z >= 0.7 || unitRotation.z <= -0.7f)
        {
            ChangeHealthBarPosition(healthbar.distance);
        }

        if (unitRotation.z <= 0.7f && unitRotation.z >= 0.0f || unitRotation.z >= -0.7f && unitRotation.z <= 0.0f)
        {
            ChangeHealthBarPosition(-healthbar.distance);
        }
    }

    private void ChangeHealthBarPosition(float delta)
    {
        var healthBarTransform = transform.parent.GetComponentInChildren<Healthbar>().transform;
        var unitPosition = transform.parent.position;
        if (!healthBarTransform.position.y.Equals(unitPosition.y + delta))
        {
            healthBarTransform.position = new Vector3(unitPosition.x, unitPosition.y + delta,
                unitPosition.z - 1);
        }
    }

    private void NextTurn()
    {
        SetHealthBarColor();
    }
}