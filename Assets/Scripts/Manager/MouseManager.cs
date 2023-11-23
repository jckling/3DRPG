using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : Singleton<MouseManager>
{
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    public Texture2D point, doorway, attack, target, arrow;
    private RaycastHit hitInfo;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (InteractWithUI())
        {
            Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
        }
        else
        {
            SetCursorTexture();
            MouseControl();
        }
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                case "Attackable":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }

    private bool InteractWithUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}