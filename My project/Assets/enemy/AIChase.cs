using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AIChase : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float chaseDistanceThreshold = 3, attackDistanceThreshold = 0.8f;

    [SerializeField]
    private float attackDelay = 1;
    private float passedTime = 1;

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        if(distance < chaseDistanceThreshold)
        {
            OnPointerInput?.Invoke(player.position);
            if (distance <= attackDistanceThreshold)
            {
                //attackbehaviour
                OnMovementInput?.Invoke(Vector2.zero);
                if (passedTime >= attackDelay)
                { 
                    passedTime = 0;
                    OnAttack?.Invoke();
                }
            }
            else
            { 
                //chasing the player
                Vector2 direction = player.position - transform.position;
                OnMovementInput?.Invoke(direction.normalized);
            }
        }
        //idle
        if (passedTime < attackDelay)
        {
            passedTime += Time.deltaTime;
        }
    }


}
