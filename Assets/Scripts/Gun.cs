using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public List<GameObject> weapons;
    private int index = 0;

    private bool m_wIsPressed = false;
    private bool m_enterIsPressed = false;

    /// <summary>
    /// The direction of the initial velocity of the fired projectile. That is,
    /// this is the direction the gun is aiming in.
    /// </summary>
    public Vector3 FireDirection
    {
        get
        {
            return transform.up;
        }
    }

    /// <summary>
    /// The position in world space where a projectile will be spawned when
    /// Fire() is called.
    /// </summary>
    public Vector3 SpawnPosition
    {
        get
        {
            return transform.position;
        }
    }

    /// <summary>
    /// The currently selected weapon object, an instance of which will be
    /// created when Fire() is called.
    /// </summary>
    public GameObject CurrentWeapon
    {
        get
        {
            if (index < weapons.Count && index >= 0)
            {
                return weapons[index];
            }
            return null;
        }
    }

    /// <summary>
    /// Spawns the currently active projectile, firing it in the direction of
    /// FireDirection.
    /// </summary>
    /// <returns>The newly created GameObject.</returns>
    public GameObject Fire()
    {
        GameObject obj = Instantiate(CurrentWeapon, SpawnPosition, Quaternion.identity);
        if (obj == null)
        {
            return null;
        }

        Particle2D particle = obj.GetComponent<Particle2D>();
        if (particle == null)
        {
            return null;
        }

        particle.velocity = FireDirection * 10.0f;

        return obj;
    }

    /// <summary>
    /// Moves to the next weapon. If the last weapon is selected, calling this
    /// again will roll over to the first weapon again. For example, if there
    /// are 4 weapons, calling this 4 times will end up with the same weapon
    /// selected as if it was called 0 times.
    /// </summary>
    public void CycleNextWeapon()
    {
        index++;
        if (index >= weapons.Count)
        {
            index = 0;
        }

    }

    void Update()
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

            bool wIsPressed = keyboard.wKey.IsPressed();
            if (wIsPressed && !m_wIsPressed)
            {
                CycleNextWeapon();
            }

            m_wIsPressed = wIsPressed;

            bool enterIsPressed = keyboard.enterKey.IsPressed();
            if (enterIsPressed && !m_enterIsPressed)
            {
                Fire();
            }

            m_enterIsPressed = enterIsPressed;
        }
    }
}
