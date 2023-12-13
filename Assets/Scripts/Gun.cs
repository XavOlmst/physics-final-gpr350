using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Gun : MonoBehaviour
{
    public List<GameObject> Weapons;
    private int _index = 0;

    /// <summary>
    /// The direction of the initial velocity of the fired projectile. That is,
    /// this is the direction the gun is aiming in.
    /// </summary>
    private Vector3 FireDirection => transform.up;

    /// <summary>
    /// The position in world space where a projectile will be spawned when
    /// Fire() is called.
    /// </summary>
    private Vector3 SpawnPosition => transform.position;

    /// <summary>
    /// The currently selected weapon object, an instance of which will be
    /// created when Fire() is called.
    /// </summary>
    private GameObject CurrentWeapon
    {
        get
        {
            if (_index < Weapons.Count && _index >= 0)
            {
                return Weapons[_index];
            }
            return null;
        }
    }

    /// <summary>
    /// Spawns the currently active projectile, firing it in the direction of
    /// FireDirection.
    /// </summary>
    /// <returns>The newly created GameObject.</returns>
    private void Fire()
    {
        GameObject obj = Instantiate(CurrentWeapon, SpawnPosition, Quaternion.identity);
        if (obj == null)
        {
            return;
        }

        PhysicsRigidbody2D physicsRigidbody = obj.GetComponent<PhysicsRigidbody2D>();
        if (physicsRigidbody == null)
        {
            return;
        }

        physicsRigidbody.Velocity = FireDirection * 15.0f;
    }

    /// <summary>
    /// Moves to the next weapon. If the last weapon is selected, calling this
    /// again will roll over to the first weapon again. For example, if there
    /// are 4 weapons, calling this 4 times will end up with the same weapon
    /// selected as if it was called 0 times.
    /// </summary>
    private void CycleNextWeapon()
    {
        _index++;
        if (_index >= Weapons.Count)
        {
            _index = 0;
        }

    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.digit1Key.IsPressed())
            {
                transform.rotation *= Quaternion.Euler(0.0f, 0.0f, 1.0f);
            }

            if (keyboard.digit2Key.IsPressed())
            {
                transform.rotation *= Quaternion.Euler(0.0f, 0.0f, -1.0f);
            }
            
            if (keyboard.wKey.wasPressedThisFrame)
            {
                CycleNextWeapon();
            }
            
            if (keyboard.enterKey.wasPressedThisFrame)
            {
                Fire();
            }
        }
    }
}
