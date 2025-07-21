using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Joe Morris
//Use this whenever needing a reference to a player component

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] bool onlyAllowOnePlayer = true;

    //Player Components/References
    Player_MovementMachine _movementMachine;
    Player_Walk _walk;
    Player_Dash _dash;
    Player_Rotate _rotate;
    Player_Animation _animation;
    Player_Jump _jump;
    Player_Gravity _gravity;
    Player_ForceHandler _forceHandler;
    Player_CombatMachine _combatMachine;
    Player_ScriptSteal _scriptSteal;
    Player_HealthComponent _health;
    Player_ChildMover _childMover;
    Player_Respawn _respawn;
    Player_Mana _mana;
    Collider _coll;
    CharacterController _controller;
    Player_RootMotion _rootMotion;
    PlayerInput _playerInput;

    [Header("Sound Bank")]
    public SoundBankSO SoundBank;

    //Public Getters
    [Header("Public Getters")]
    public Player_MovementMachine MovementMachine { get { return _movementMachine; } }
    public Player_Walk Walk { get { return _walk; } }
    public Player_Dash Dash { get { return _dash; } }
    public Player_Rotate Rotate { get { return _rotate; } }
    public Player_Animation Animation { get { return _animation; } }
    public Player_Jump Jump { get { return _jump; } }
    public Player_Gravity Gravity { get { return _gravity; } }
    public Player_ForceHandler ForceHandler { get { return _forceHandler; } }
    public Player_CombatMachine CombatMachine { get { return _combatMachine; } }
    public Player_ScriptSteal ScriptSteal { get { return _scriptSteal; } }
    public Player_HealthComponent Health { get { return _health; } }
    public Player_ChildMover ChildMover { get { return _childMover; } }
    public Player_Respawn Respawn { get { return _respawn; } }
    public Player_Mana Mana { get { return _mana; } }
    public Player_RootMotion RootMotion { get { return _rootMotion; } }
    public Collider Collider { get { return _coll; } }
    public CharacterController CharacterController { get { return _controller;}}
    public PlayerInput PlayerInput { get { return _playerInput;}}
    void Awake()
    {
        HandleInstance();
        AssignReferences();
    }

    void HandleInstance()
    {
        if (instance != null && onlyAllowOnePlayer)
        {
            Destroy(transform.root.gameObject);
        }
        else instance = this;
    }

    void AssignReferences()
    {
        _movementMachine = GetComponent<Player_MovementMachine>();
        _walk = GetComponent<Player_Walk>();
        _dash = GetComponent<Player_Dash>();
        _rotate = GetComponent<Player_Rotate>();
        _animation = GetComponent<Player_Animation>();
        _jump = GetComponent<Player_Jump>();
        _gravity = GetComponent<Player_Gravity>();
        _forceHandler = GetComponent<Player_ForceHandler>();
        _combatMachine = GetComponent<Player_CombatMachine>();
        _scriptSteal = GetComponent<Player_ScriptSteal>();
        _health = GetComponent<Player_HealthComponent>();
        _childMover = GetComponent<Player_ChildMover>();
        _respawn = GetComponent<Player_Respawn>();
        _mana = GetComponent<Player_Mana>();
        _coll = GetComponent<Collider>();
        _controller = GetComponent<CharacterController>();
        _rootMotion = GetComponent<Player_RootMotion>();
        _playerInput = GetComponent<PlayerInput>();
    }
}
