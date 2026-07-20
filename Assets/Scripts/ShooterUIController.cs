using UnityEngine;
using UnityEngine.UI;

public class ShooterUIController : MonoBehaviour
{
    [SerializeField] private Button _shootButton;
    [SerializeField] private ProjectileShooter _shooter;

    private void Start()
    {
        if (_shootButton != null && _shooter != null)
        {
            _shootButton.onClick.AddListener(_shooter.Shoot);
        }
    }
}