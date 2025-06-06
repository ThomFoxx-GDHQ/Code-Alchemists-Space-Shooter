using UnityEngine;

public class ShieldVisualization : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _fullHealthColor;
    [SerializeField] private Color _halfHealthColor;
    [SerializeField] private Color _lowHealthColor;

    public void ApplyHealth(int health)
    {        
        switch (health)
        {
            case 3:
                _renderer.material.color = _fullHealthColor;
                UIManager.Instance.UpdateShieldUI(3);
                break;
            case 2:
                _renderer.material.color = _halfHealthColor;
                UIManager.Instance.UpdateShieldUI(2);
                break;
            case 1:
                _renderer.material.color = _lowHealthColor;
                UIManager.Instance.UpdateShieldUI(1);
                break;
            case 0:
                _renderer.material.color = _fullHealthColor;
                UIManager.Instance.UpdateShieldUI(0);
                break;
            default:
                break;
        }
    }
}
