using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Search,
    }

    [Header("Health")]
    [SerializeField] int _Health = 1000;
    [SerializeField] int _maxHealth = 2000;

    [Header("Navigation")]
    [SerializeField] float _ReNavigateInterval = 1f;

    [Header("Patrol Settings")]
    [SerializeField] private float _PatrolSpeed = 2f;
    [SerializeField] private Transform[] _PatrolPoints;     // Possible patrol/search locations
    [SerializeField] private float _IdleAtPatrolPointTime = 2f;

    [Header("Chase Settings")]
    [SerializeField] private float _ChaseSpeed = 5f;
    [SerializeField] private float _PreferredDistance = 8f;
    [SerializeField] private float _DistanceTolerance = 2f;
    [SerializeField] private float _ChaseBuffer = 1f; // The buffer time to wait before the chase state exits to the search state

    [Header("Search Settings")]
    [SerializeField] private float _TimeToRememberPlayer = 3f;

    [Header("Attack Settings")]
    [SerializeField] private bool _AttackOnSight = true;
    [SerializeField] private float _InitialAttackInterval = 1f; // The time to wait before the first attack after entering the chase state

    // Internal references
    PlayerController _player;
    AINavigation _navigation;
    AIPerception _perception;
    ShootMechanic _shootMechanic;

    //My Variables
    public float aiDeath = 0;
    public bool oneDeadAI = false;
    public bool regenActive = false;
    public HealthBar healthBar;

    // Internal state
    AIState _currentState = AIState.Idle;
    private float _stateTimer = 0f;
    private float _chaseBuffer = 0f;

    //Vita & Tau's chat boxes
    [SerializeField] public GameObject tauOne;
    [SerializeField] public GameObject tauTwo;
    [SerializeField] public GameObject tauThree;
    [SerializeField] public GameObject vitaOne;
    [SerializeField] public GameObject vitaTwo;
    [SerializeField] public GameObject vitaThree;
    public bool bothAlive = true;

    private Transform _currentPatrolPoint;
    private Vector3 _chaseStartPosition;
    private Vector3 _lastKnownPlayerPosition;



    void Start()
    {
        healthBar.UpdateHealthBar(_Health, _maxHealth);
    }

    #region Unity Lifecycle

    private void Awake()
    {
        // Find the player, navigation, shoot mechanic, and perception components
        _player = FindFirstObjectByType<PlayerController>();
        _navigation = this.GetComponent<AINavigation>();
        _shootMechanic = this.GetComponent<ShootMechanic>();
        _perception = this.GetComponent<AIPerception>();



        // Start the agent in the idle state
        SwitchState(AIState.Idle);
    }


    private void Update()
    {
        // Update the state timer
        _stateTimer += Time.deltaTime;

        // Set the name of the agent to the current state for debugging purposes
        this.name = "AI Agent: " + _currentState;

        // Update the state of the agent
        switch (_currentState)
        {
            case AIState.Idle:
                UpdateIdle();
                break;

            case AIState.Patrol:
                UpdatePatrol();
                break;

            case AIState.Chase:
                UpdateChase();
                break;

            case AIState.Search:
                UpdateSearch();
                break;
        }

        //Regen ability activates when one dies.
        if (regenActive)
        {
            _Health += 1;
            healthBar.UpdateHealthBar(_Health, _maxHealth);
        }


    }

    #endregion

    #region State Updates

    private void SwitchState(AIState newState)
    {

        // First call the exit state method to clean up the current state
        OnExitState(_currentState);

        // Then set the new state
        _currentState = newState;

        // Finally call the enter state method to initialize the new state
        OnEnterState(newState);
    }

    // Called once when the state is entered
    private void OnEnterState(AIState state)
    {
        _stateTimer = 0f;

        switch (state)
        {
            case AIState.Idle:
                break;

            case AIState.Patrol:
                _navigation.SetSpeed(_PatrolSpeed);
                PickNewPatrolPoint();
                break;

            case AIState.Chase:
                _chaseStartPosition = this.transform.position;
                _navigation.SetSpeed(_ChaseSpeed);
                break;

            case AIState.Search:
                _navigation.SetSpeed(_PatrolSpeed);
                _navigation.SetDestination(_lastKnownPlayerPosition, true);
                break;
        }
    }

    // Called once when the state is exited
    private void OnExitState(AIState state)
    {
        switch (state)
        {
            case AIState.Idle:
                break;

            case AIState.Patrol:
                break;

            case AIState.Chase:
                break;

            case AIState.Search:
                break;
        }
    }

    #endregion

    #region State Updates
    private void UpdateIdle()
    {
        // If we can see the player, start chasing.
        if (_perception.CanSeePlayer)
        {
            SwitchState(AIState.Chase);
        }

        // If player is not even in detection range, start searching.
        else
        {
            SwitchState(AIState.Patrol);
        }
    }

    private void UpdatePatrol()
    {
        // If we see the player at any time, start chasing.
        if (_perception.CanSeePlayer)
        {
            SwitchState(AIState.Chase);
            return;
        }

        // If no search points, just go back to idle.
        if (_PatrolPoints == null || _PatrolPoints.Length == 0)
        {
            SwitchState(AIState.Idle);
            return;
        }

        // Make sure we have somewhere to go.
        if (_currentPatrolPoint == null)
        {
            PickNewPatrolPoint();
        }

        // If we've reached the destination, set the state timer to the idle time.
        if (_navigation.IsAtDestination())
        {
            _currentPatrolPoint = null;
            SwitchState(AIState.Idle);
        }
    }

    private void UpdateChase()
    {
        float distanceToPlayer = Vector3.Distance(this.transform.position, _player.transform.position);

        if (_perception.CanSeePlayer)
        {
            // Update last known position whenever we see the player.
            //_lastKnownPlayerPosition = _player.transform.position;

            // Too far: move closer.
            if (distanceToPlayer > _PreferredDistance + _DistanceTolerance)
            {
                //Debug.Log("TOO FAR, moving closer");
                _navigation.SetDestination(Vector3.Lerp(_player.transform.position, _chaseStartPosition, 0.5f));
            }
            // Too close: back away to our preferred distance.
            else if (distanceToPlayer < _PreferredDistance - _DistanceTolerance)
            {
                //Debug.Log("TOO CLOSE, moving away");
                Vector3 directionAway = (this.transform.position - _player.transform.position).normalized;
                Vector3 targetPosition = _player.transform.position + directionAway * _PreferredDistance;

                _navigation.SetDestination(targetPosition);
            }

            // Within target distance, stay still.
            else
            {
                //Debug.Log("WITHIN TARGET DISTANCE, staying still");
                _navigation.SetDestination(this.transform.position);
            }


            // Perform the shoot action
            if(_AttackOnSight && _stateTimer >= _InitialAttackInterval) 
            {
                _shootMechanic.PerformShoot(_perception.GetPlayerCenterPosition());
            }
        }
        else
        {
            _chaseBuffer += Time.deltaTime;

            // If we have been chasing for too long, start searching.
            if(_chaseBuffer >= _ChaseBuffer)
            {
                _lastKnownPlayerPosition = _player.transform.position;
                _chaseBuffer = 0f;

                SwitchState(AIState.Search);
            }   
        }
    }

    private void UpdateSearch()
    {
        // If we have been searching for too long, go back to patrol.
        if (_stateTimer >= _TimeToRememberPlayer)
        {
            // Move to last known location.
            _navigation.SetDestination(_lastKnownPlayerPosition);

            // Once we arrive and still can't see them, go back to search.
            if (_navigation.IsAtDestination())
            {
                SwitchState(AIState.Patrol);
            }
        }

        // If we can see the player again, start chasing.
        if (_perception.CanSeePlayer)
        {
            SwitchState(AIState.Chase);
        }
    }

    #endregion

    private void PickNewPatrolPoint()
    {
        if (_PatrolPoints == null || _PatrolPoints.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, _PatrolPoints.Length);
        _currentPatrolPoint = _PatrolPoints[index];

        _navigation.SetDestination(_currentPatrolPoint.position);
    }

    //AI takes damage
    public void TakeDamage(int damage)
    {

        _Health -= damage;

        if (bothAlive)
        {
            ChatLogChooser();
        }
        //Update healthbar
        healthBar.UpdateHealthBar(_Health, _maxHealth);
        if (_Health <= 0)
        {
            //Destroy the enemy
            Destroy(this.gameObject);
            bothAlive = false;
            //Set everything off
            vitaOne.SetActive(false);
            vitaTwo.SetActive(false);
            vitaThree.SetActive(false);
            tauOne.SetActive(false);
            tauTwo.SetActive(false);
            tauThree.SetActive(false);

            //If this is the first AI to die, then we run this
            if (aiDeath == 0)
            {
                aiDeath++;
                //Parameter to bring the Vita and Tau merge online
                oneDeadAI = true;
                
            }
            

        }
    }

    //Updates the stats of the remaining twin
    public void UpdateStats(int health, int speed)
    {
        Debug.Log("Merging Vita and Tau");
        //Restore health to max
        _Health += _maxHealth;
        //Add Vita or Tau's health to the health of the surviving one
        _Health += health;
        healthBar.UpdateHealthBar(_Health, _maxHealth);

        //Increase speed slightly
        _ChaseSpeed += speed;

        //Enable his regeneration capabilities
        regenActive = false;
    }

    public void ChatLogChooser()
    {
        //Set everything off
        vitaOne.SetActive(false);
        vitaTwo.SetActive(false);
        vitaThree.SetActive(false);
        tauOne.SetActive(false);
        tauTwo.SetActive(false);
        tauThree.SetActive(false);

        int number = UnityEngine.Random.Range(0, 6);

        if (number == 0)
        {
            vitaOne.SetActive(true);
        }
        else if (number == 1)
        {
            tauOne.SetActive(true);
        }
        else if (number == 2)
        {
            vitaTwo.SetActive(true);
        }
        else if (number == 3)
        {
            tauTwo.SetActive(true);
        }
        else if (number == 4)
        {
            vitaThree.SetActive(true);
        }
        else if (number == 5)
        {
            tauThree.SetActive(true);
        }
    }
}