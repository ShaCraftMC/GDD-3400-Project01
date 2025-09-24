using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

namespace GDD3400.Project01
{
    public class Dog : MonoBehaviour
    {
        /// <summary>
        /// Notes: Arena is 50units x 50units Middle is 0,0. edges is -25,25 or 25,25 or 25,-25 or -25,-25
        /// 
        /// </summary>
        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        // Required Variables (Do not edit!)
        private float _maxSpeed = 5f;
        private float _sightRadius = 7.5f;
        //Temp spd var
        private float speed = 3f;

        //My Own
        Vector3 savedPosition;
        bool onPatrol = false;
        bool onHerding = false;
        bool onStraight = false;
        //Vector3 ranDirection;
        Rigidbody rb3d;
        private float maxRotation = 90f;
        private float orientation = 0f;
        bool firstTime = true;
        Vector3 safeZone;
        float wanderCount;
        float count = 8f;



        // Layers - Set In Project Settings
        public LayerMask _targetsLayer;
        public LayerMask _obstaclesLayer;

        // Tags - Set In Project Settings
        private string friendTag = "Friend";
        private string threatTag = "Threat";
        private string safeZoneTag = "SafeZone";



        public void Awake()
        {
            // Find the layers in the project settings
            _targetsLayer = LayerMask.GetMask("Targets");
            _obstaclesLayer = LayerMask.GetMask("Obstacles");
            //Get the component of the rigidbody that is attached to our dog
            rb3d = GetComponent<Rigidbody>();
            //Freeze rotation for time being so physics doesn't mess with our rotation
            rb3d.freezeRotation = true;
            //Get our spawn location, which is the safe zone and save for future
            //GameObject safeZone = GameObject.FindGameObjectWithTag("SafeZone"); 
            StartCoroutine("Wandering");
            onPatrol = true;
            onHerding = false;
            onStraight = false;
            wanderCount = 0;

            //Set tag of dog to initially of "threat"
            //transform.Find("Collision").tag = "Threat";

            
        }

        public void Start()
        {
            //Save safeZone position
            safeZone = transform.position;
            transform.Find("Collision").tag = "Threat";
            _sightRadius = 1;

        }

        private void Update()
        {
            if (!_isActive) return;

            Perception();
            //DetectObjectsInRadius();
            //DecisionMaking();
            //Look for objects in range with specific tag;




        }

        private void Perception()
        {

        }

        private void DecisionMaking()
        {

            //print(gameObject.tag);
            //Start patroling
            if (onPatrol)
            {
                Patroling();
            }
            
            else if (onHerding)
            {
                Herding();
            }
            
            else if (onStraight)
            {
                //Straight();

            }
            //if dog is doing nothing, stop all coroutines
            else
            {
                StopCoroutine("Wandering");
            }
            count--;
            


        }

        //Wandering Coroutine (Used to constantly fire the behavior for right now)
        IEnumerator Wandering()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);

                DecisionMaking();
            }
        }

        /// <summary>
        /// Make sure to use FixedUpdate for movement with physics based Rigidbody
        /// You can optionally use FixedDeltaTime for movement calculations, but it is not required since fixedupdate is called at a fixed rate
        /// </summary>
        private void FixedUpdate()
        {
            if (!_isActive) return;


        }

        //Bulk code for the patroling function of the dog
        public void Patroling()
        {
            speed = 3;
            //Transform child = transform.Find("Friend");
            transform.Find("Collision").tag = "Friend";
            //print(child.tag);

            //Code for Wandering
            float randomBinomial = UnityEngine.Random.value - UnityEngine.Random.value;
            //Set our random value of -1 to 1 times our maxRotation set above
            orientation += randomBinomial * maxRotation;

            //Safe guard to make sure our rotation is strictly between 0 - 360
            orientation = Mathf.Repeat(orientation, 360);
            //Convert our orentation value to direction value so our dog can use it
            //Using a method setup to convert

            Vector3 direction = OrientationToVector(orientation);

            //Set velocity with direction
            rb3d.linearVelocity = direction * speed;

            if (direction != Vector3.zero)
            {
                rb3d.MoveRotation(Quaternion.LookRotation(direction));
            }

            //If the Patroling() has fired atleast 6 times, set velocity to zero,and start the herding method
            
            if (wanderCount >= 6)
            {
                rb3d.linearVelocity = Vector3.zero;
                onPatrol = false;
                onHerding = true;
            }
            wanderCount++;
            
            


        }

        //Herding function behavior
        public void Herding()
        {
            //reduce our speed, and set our tag to friend, so sheep will flock
            speed = 2;
            transform.Find("Collision").tag = "Friend";
            //Force dog to look at the safeZone;
            transform.LookAt(safeZone);
            //Go to the safezone
            Vector3 direction = (safeZone - transform.position).normalized;
            rb3d.linearVelocity = direction * speed;


            
            //If wanderCount <= 0, end herding, and begin patroling again
            if (wanderCount <= 0)
            {
                onPatrol = true;
                onHerding = false;
            }
            //wanderCount is our timer for our herding, so after a certain point, dog goes back to wander
            wanderCount = wanderCount - 2f;
            
            
        }



        //Currently working on, goal is to go straight out, then stop, and start the onPatroling()
        /*
        public void Straight()
        {
            speed = 5;
            transform.LookAt(-transform.position);
            Vector3 direction = (-transform.position - transform.position).normalized;
            rb3d.linearVelocity = direction * speed;


            onStraight = false;

            if (count <= 0)
            {
                rb3d.linearVelocity = Vector3.zero;
                onPatrol = true;

            }

        }
        */

        //Translate our turn from degrees to radians, so Unity can use it
        private Vector3 OrientationToVector(float angleDegress)
        {
            float radians = angleDegress * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        }

        
        //Not working as intended
        
        void DetectObjectsInRadius()
        {
            //_sightRadius = _sightRadius - 5;
            // Get all colliders within the radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, _sightRadius);

            foreach (Collider collider in colliders)
            {
                // Check if the object has the specified tag
                if (collider.CompareTag("Friend"))
                {
                    onPatrol = false;
                    onHerding = true;
                }
        
       
                /*
                else if (collider.CompareTag("SafeZone"))
                {
                    onHerding = false;
                    onPatrol = true;

                }  
                */
            }
            /*
            if (colliders.Length <= 0)`
            {
                onPatrol = true;
                onHerding = false;
            }
            */
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Friend"))
            {
                onPatrol = false;
                onHerding = true;
            }

        }
    }

}























        /*
        //Patroling Variables
        public Transform[] patrolPoints;
        public float waitTime;
        int currentPointIndex;
        public float speed = 5;
        bool once = false;
        */

        /*
        void Update()
        {
            if (transform.position != patrolPoints[currentPointIndex].transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, speed * Time.deltaTime);
            }
            else
            {
                if (once == false)
                {
                    once = true;
                    StartCoroutine(Wait());
                }

            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitTime);
            if (currentPointIndex + 1 < patrolPoints.Length)
            {
                currentPointIndex++;
            }
            else
            {
                currentPointIndex = 0;
            }
            once = false;
        */
            //Follow Code
            /*
            private void Update()
            {
                if (Vector2.Distance(transform.position, target.position) > minDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, _maxSpeed * Time.deltaTime);
                }

            }
            */




           
