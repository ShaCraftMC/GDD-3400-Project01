using System;
using System.Collections;
using UnityEngine;

namespace GDD3400.Project01
{
    public class Dog : MonoBehaviour
    {

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
        public Vector3 safeZone;
        public Transform target;
        public float minDistance;
        public Vector3 ranDirection;
        bool onPatrol = true;

        // Layers - Set In Project Settings
        public LayerMask _targetsLayer;
        public LayerMask _obstaclesLayer;

        // Tags - Set In Project Settings
        private string friendTag = "Friend";
        private string threatTag = "Threat";
        private string safeZoneTag = "SafeZone";



        //Commented out while I expirement
        
        public void Awake()
        {
            // Find the layers in the project settings
            _targetsLayer = LayerMask.GetMask("Targets");
            _obstaclesLayer = LayerMask.GetMask("Obstacles");
            //Get our spawn location, which is the safe zone and save for future
            safeZone = transform.position;



        }

        private void Update()
        {
            if (!_isActive) return;

            Perception();
            DecisionMaking();
        }

        private void Perception()
        {

        }

        private void DecisionMaking()
        {
            //Start patroling
            if (onPatrol)
            {
                Patroling();
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

        public void Patroling()
        {
            //Code for Patroling
        }

        public void RandomDirection()
        {
            ranDirection = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 0, UnityEngine.Random.Range(-10.0f, 10.0f));
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




           
