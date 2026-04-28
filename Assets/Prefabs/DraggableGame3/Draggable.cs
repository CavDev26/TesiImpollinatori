using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;

public class Draggable : MonoBehaviour
{
    public bool IsDragging;
    public Vector3 LastPosition;
    public AudioClip Clip;

    private Collider2D _collider;
    private DragController _controller;
    private float _movementTime = 15f;
    private System.Nullable<Vector3> _movementDestination;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _controller = FindObjectOfType<DragController>();
    }

    void FixedUpdate()
    {
        if (_movementDestination.HasValue)
        {
            if(IsDragging)
            {
                _movementDestination = null;
                return;
            }

            if(transform.position != _movementDestination)
            {
                gameObject.layer = DragLayer.Default;
                _movementDestination = null;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _movementDestination.Value, _movementTime * Time.fixedDeltaTime);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Draggable collidedDraggable = collision.GetComponent<Draggable>();

        if(collidedDraggable != null  && _controller.LastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = collision.Distance(_collider);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            transform.position -= diff;
            if(transform.position.x > 1.8f || transform.position.x < -1.8f || transform.position.y > 4.6f || transform.position.y < -4.4f)
            {
                transform.position = LastPosition;
            }
        }

        if (collision.CompareTag("Libero")) //se il box è vuoto
        {
            transform.position = new Vector3(collision.transform.position.x - .07f, collision.transform.position.y + .1f);
            collision.tag = "Occupato";
            GameAudioController.PlayClip(Clip);
            Game3Controller.ImpInBox(GetIndexImp(gameObject.name), GetIndexBox(collision.gameObject.name));
            //Debug.Log(GetIndexBox(collision.gameObject.name) + " contiene " + GetIndexImp(gameObject.name));
        }
        else if(collision.CompareTag("Occupato")) //se il box è occupato
        {
            _movementDestination = LastPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Occupato")) //se il box è occupato
        {
            collision.tag = "Libero";
            Game3Controller.Accoppiamenti[GetIndexBox(collision.gameObject.name)] = -1;
        }
    }

    private int GetIndexBox(string nome)
    {
        switch(nome)
        {
            case "BoxDragged1":
                return 0;
            case "BoxDragged2":
                return 1;
            case "BoxDragged3":
                return 2;
            case "BoxDragged4":
                return 3;
            default: return -1;
        }
    }

    private int GetIndexImp(string nome)
    {
        switch (nome)
        {
            case "Draggable1":
                return 0;
            case "Draggable2":
                return 1;
            case "Draggable3":
                return 2;
            case "Draggable4":
                return 3;
            default: return -1;
        }
    }
}
