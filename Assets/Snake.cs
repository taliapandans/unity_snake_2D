using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    
    public Transform bodyPrefab;

    private InputAction _playerMovement;
    private Vector2 _keyInput;
    private Vector2 _direction = Vector2.zero;
    private List<Transform> _segments = new List<Transform>();
    private float _speed = 10;
    private float _moveCounter = 0;
    private float _speedMultiplier = 1f;

    private void Start()
    {
        ResetState();

        // Include head (GameObject transform) into list of snake segments 
        _segments.Add(transform);

        // Bind player movements with input systems
        _playerMovement = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        // Update the input/direction of snake head each frame
        _keyInput = _playerMovement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // If snake head position in (0,0), it needs keypress to start movement direction
        // In case of two keys pressed together, the head has to pick the direction with nearest integer
        // So it can only move in x-(horizontal) OR y-(vertical) axis
        if (_direction.x == 0 && _direction.y == 0)
        {
            _direction = _keyInput;
        } 
        // In case snake moves in y-Axis, only allow to press horizontal (right OR left) keys then move in x-Axis
        else if (Math.Abs(_keyInput.x) > Math.Abs(_keyInput.y) && _direction.y != 0)
        {
            _direction = new Vector2(_keyInput.x, 0.0f);
        }
        // In case snake moves in x-Axis, only allow to press horizontal (up OR down) keys then move in y-Axis
        else if(Math.Abs(_keyInput.x) < Math.Abs(_keyInput.y) && _direction.x != 0)
        {
            _direction = new Vector2(0.0f, _keyInput.y);
        }

        // moveCounter acts as speed translation of the segments since snake move in discrete speed
        _moveCounter += _speed * Time.fixedDeltaTime;
        while(_moveCounter >= 1)
        {
            // Change the position of the next segment to the previous segment
            // Starting from the last segment (tail) until the segment before the head
            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
            }

            // Move the snake in the direction it is facing
            // Round the values to ensure it aligns to the grid
            this.transform.position = new Vector2(
                Mathf.Round(this.transform.position.x) + _direction.x,
                Mathf.Round(this.transform.position.y) + _direction.y
            );
            _moveCounter--;
        }
    }

    private void Grow()
    // Add cloned segments to list
    {
        // Clone segment (bodyPrefab) assets
        Transform segment = Instantiate(this.bodyPrefab);

        // Put each added segment to the last part of the body/list
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
        
        _speed += _speedMultiplier;
    }

    public bool SnakeArea(float posX, float posY)
    //Tell snake segments position so that it doesnt overlap with food
    {
        foreach (Transform segment in _segments)
        {
            if (posX != segment.position.x && posY != segment.position.y)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetState()
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If head collide with food, add segment to body
        if(other.tag == "Food")
        {
            Grow();
        }
        else if(other.tag == "Wall")
        {
            ResetState();
        }
        else if(other.tag == "OwnSegment")
        {
            // in case the first two segment touch head due to discrete speed 
            if (other.gameObject.GetInstanceID() != _segments[1].gameObject.GetInstanceID())
            {
                ResetState();
            }
            else if (_segments.Count > 2 && other.gameObject.GetInstanceID() != _segments[2].gameObject.GetInstanceID())
            {
                ResetState();
            }
        }
    }
}
