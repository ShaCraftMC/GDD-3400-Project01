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
        
        //My Own
        Vector3 savedPosition;
        bool onPatrol = true;
        //Vector3 ranDirection;
        Rigidbody rb3d;
        private float maxRotation = 90f;
        private float orientation = 0f;
        bool firstTime = true;
        
        

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
            GameObject safeZone = GameObject.FindGameObjectWithTag("SafeZone");
            onPatrol = true;

            StartCoroutine("Wandering");
        }

        private void Update()
        {
            if (!_isActive) return;

            Perception();
            //DecisionMaking();


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
            else
            {
                StopCoroutine("Wandering");
            }
            
        }

        IEnumerator Wandering()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                DecisionMaking();
            }

            //Patroling();
        }

        /// <summary>
        /// Make sure to use FixedUpdate for movement with physics based Rigidbody
        /// You can optionally use FixedDeltaTime for movement calculations, but it is not required since fixedupdate is called at a fixed rate
        /// </summary>
        private void FixedUpdate()
        {
            if (!_isActive) return;
            

        }

        public void Patroling()
        {
            //For testing purposes
            //gameObject.tag = friendTag;

            print(gameObject.tag);

            //Code for Wandering
            float randomBinomial = UnityEngine.Random.value - UnityEngine.Random.value;
            //Set our random value of -1 to 1 times our maxRotation set above
            orientation += randomBinomial * maxRotation;

            //Safe guard to make sure our rotation is strictly between 0 - 360
            orientation = Mathf.Repeat(orientation, 360);

            if (firstTime)
            {
                firstTime = false;
                orientation = 0;
            }
            //Convert our orentation value to direction value so our dog can use it
            //Using a method setup to convert

            Vector3 direction = OrientationToVector(orientation);

            rb3d.linearVelocity = direction * _maxSpeed;


            if (direction != Vector3.zero)
            {
                rb3d.MoveRotation(Quaternion.LookRotation(direction));
            }


        }

        private Vector3 OrientationToVector(float angleDegress)
        {
            float radians = angleDegress * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radians),0, Mathf.Cos(radians));
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




           
