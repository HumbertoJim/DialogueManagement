using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ScheduleManagement.Managers;
using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;

using AgentManagement.Actions;
using VRoidExtensions.Models;

namespace AgentManagement
{
    namespace Agents
    {
        public class GenericAgent : ScheduleManager
        {
            [Header("Dependencies")]
            [SerializeField] GenericModel model;
            [SerializeField] NavMeshAgent navNesh;

            [Header("Actions")]
            [SerializeField] float minTimeForRandom = 5;
            [SerializeField] float maxTimeForRandom = 15;
            [SerializeField] AgentAction idleAction;
            [SerializeField] AgentAction defaultAction;
            [SerializeField] List<AgentAction> randomActions;
            [SerializeField] bool enableRandomAction = false;

            [Header("Movement")]
            [SerializeField] float movementSpeed = 2;
            [SerializeField] float targetMargin = 1f;
            [SerializeField] float maxAllowedFarDistance = 15f;
            [SerializeField] bool enableMovement = true;
            List<Vector3> path;
            AgentAction goBack;

            [Header("Random Walk")]
            [SerializeField] float randomWalkRange = 10f;
            [SerializeField] bool enableRandomWalk = true;
            AgentAction randomWalk;

            [Header("Rotation")]
            [SerializeField] float rotationTime = 3f;
            [SerializeField] float startRotationDelay = 0.5f;
            [SerializeField] float endRotationDelay = 0.5f;
            [SerializeField] float angleTheresholdForRotationTime = 90; // El tiempo de rotacion es el mismo siempre, pero la velocidad no. Si el angulo de rotacion es menor que esta variable, se mantiene constante, caso contrario, incrementa a medida que el angulo lo hace.
            [SerializeField] float angleTheresholdForRotateAnimation = 45; // Umbral para decidir que animacion de rotacion usar: se tiene tres, una donde no rota pero si mueve los pies, una donde rota a la derecha y otra donde rota a la izquierda
            bool turning;
            float rotateTo;
            float rotationSpeed;

            [Header("Audio")]
            [SerializeField] AudioClip defaultClip;
            [Header("Test")]
            [SerializeField] Transform targetMarker;


            // properties
            public GenericModel Model { get; private set; }
            public Vector3 Target { get; private set; }
            public Vector3 StartPosition { get; private set; }
            public float MovementSpeed { get { return navNesh.speed; } set { navNesh.speed = value; } }
            public AgentAction CurrentAction { get; private set; }
            public bool EnableRandomAction
            {
                get { return enableRandomAction; }
                set
                {
                    if (enableRandomAction != value)
                    {
                        if (!value)
                        {
                            Scheduler.Cancel("ExecuteRandomAction");
                        }
                        enableRandomAction = value;
                    }
                }
            }

            public bool EnableMovement
            {
                get { return enableMovement; }
                set
                {
                    if (value && !enableMovement) { if (path.Count > 0) Goto(path[0]); }
                    else { navNesh.ResetPath(); Model.Animator.SetFloat("Speed", 0); }
                    enableMovement = value;
                }
            }

            protected override void Awake()
            {
                base.Awake();

                path = new List<Vector3>();
                StartPosition = transform.position;
                MovementSpeed = movementSpeed;
                Model = model;
                Model.SetInformation();

                SetDefault();

                // include idle to random actions
                randomActions.Add(idleAction);

                // go back action
                goBack = ScriptableObject.CreateInstance<AgentAction>();
                goBack.name = "GoBack";
                goBack.path = new List<Vector3>() { StartPosition };

                // include walk to random actions
                randomWalk = ScriptableObject.CreateInstance<AgentAction>();
                randomWalk.name = "RandomWalk";
                if (enableRandomWalk)
                {
                    randomWalk.path = new List<Vector3>() { new Vector3(StartPosition.x + Random.Range(-randomWalkRange, randomWalkRange),
                StartPosition.y, StartPosition.z + Random.Range(-randomWalkRange, randomWalkRange))};
                    randomActions.Add(randomWalk);
                }

                // temporal manager

                // temporal manager - random action
                Scheduler.Add("ExecuteRandomAction", new Schedulable(() => ExecuteAction(randomActions[Random.Range(0, randomActions.Count)])));

                // temporal manager - rotate
                Scheduler.Add("EndRotation",
                    new Schedulable(
                        rotationTime,
                        () =>
                        {
                            turning = false;
                            transform.eulerAngles = new Vector3(0, rotateTo, 0);
                            Model.Animator.SetBool("Rotate", false);
                            Model.Animator.SetFloat("Rotation", 0);
                        }
                    )
                );

                Scheduler.Add("Rotate",
                    new Schedulable(
                        startRotationDelay,
                        () =>
                        {
                            rotateTo = Mathf.Asin((Target.z - transform.position.z) / (Target - transform.position).magnitude) * Mathf.Rad2Deg;
                            rotateTo = transform.position.x > Target.x ? 270 + rotateTo : 90 - rotateTo;

                            float rotation_delta = (360f + (rotateTo - transform.eulerAngles.y)) % 360f; // angle between desiredDirection and current direction
                        float rotation_delta_complement = 360f - rotation_delta;
                            float min_rotation_delta;

                        // Rotation movement
                        if (rotation_delta < rotation_delta_complement) // to right
                        {
                                min_rotation_delta = rotation_delta;
                                Model.Animator.SetFloat("Rotation", (rotation_delta < angleTheresholdForRotateAnimation) ? 0 : 45);
                            }
                            else // to left
                        {
                                min_rotation_delta = rotation_delta_complement;
                                Model.Animator.SetFloat("Rotation", (rotation_delta_complement < angleTheresholdForRotateAnimation) ? 0 : -45);
                            }

                        // Rotation speed
                        rotationSpeed = rotationTime;
                            if (min_rotation_delta > angleTheresholdForRotationTime)
                            {
                                rotationSpeed *= min_rotation_delta / angleTheresholdForRotationTime;
                            }

                            Model.Animator.SetBool("Rotate", true);

                        // end next rotation temporal
                        Scheduler.Start("EndRotation");
                        }
                    )
                );

                // temporal manager - goto
                Scheduler.Add("Goto",
                    new Schedulable(
                        startRotationDelay + rotationTime + endRotationDelay,
                        () =>
                        {
                            if (EnableMovement)
                            {
                                Model.Animator.SetFloat("Speed", 1);
                                navNesh.SetDestination(Target);
                            }
                        }
                    )
                );

                // temporal manager - execute action
                Scheduler.Add("ExecuteAction", new Schedulable(0.5f, () => ExecuteAction()));

                // include default to random actions
                if (defaultAction)
                {
                    randomActions.Add(defaultAction);
                    ExecuteAction(defaultAction);
                }
                else
                {
                    ExecuteAction(idleAction);
                }
            }

            protected override void Update()
            {
                base.Update();

                if (turning) // Turning
                {
                    transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, rotateTo, rotationSpeed * Time.deltaTime), 0);
                }
                else if (path.Count > 0)
                {
                    if (EnableMovement)
                    {
                        if ((transform.position - StartPosition).magnitude > maxAllowedFarDistance && CurrentAction != goBack)
                        {
                            ExecuteAction(goBack);
                        }
                        else if (navNesh.pathStatus != NavMeshPathStatus.PathComplete || IsTargetAprox(transform.position))
                        {
                            path.RemoveAt(0);
                            if (path.Count > 0) Goto(path[0]); else ExecuteAction(idleAction);
                        }
                    }
                }
                else if (EnableRandomAction && !Scheduler.IsActive("ExecuteRandomAction") && !Scheduler.IsActive("ExecuteAction"))
                {
                    Scheduler.Start("ExecuteRandomAction", Random.Range(minTimeForRandom, maxTimeForRandom));
                }
            }

            void SetDefault()
            {
                turning = false;

                path.Clear();
                navNesh.ResetPath();
                Model.Audio.Stop();

                Model.Animator.SetBool("Rotate", false);
                Model.Animator.SetFloat("Speed", 0);
            }

            /* Audio */
            public virtual void PlayAudio(AudioClip clip)
            {
                if (clip)
                {
                    Model.Audio.clip = clip;
                }
                else
                {
                    Model.Audio.clip = defaultClip;
                }
                Model.Audio.Play();
            }

            /* Goto*/
            public void Navigate(List<Vector3> path)
            {
                this.path = new List<Vector3>(path);
                if (path.Count > 0)
                {
                    Goto(path[0]);
                }
            }

            public void Goto(Vector3 position, bool enqueue_path)
            {
                if (enqueue_path)
                {
                    path.Add(position);
                    if (path.Count == 1)
                    {
                        Goto(path[0]);
                    }
                }
                else
                {
                    path = new List<Vector3>() { position };
                    Goto(path[0]);
                }
            }

            void Goto(Vector3 position)
            {
                Scheduler.Cancel("Goto");

                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 10f, NavMesh.AllAreas))
                {
                    position = hit.position;
                }
                else
                {
                    position = transform.position + Random.insideUnitSphere;
                }

                if (targetMarker) targetMarker.position = position;

                Rotate(position, false);

                Scheduler.Start("Goto");
            }

            public bool IsTargetAprox(Vector3 position)
            {
                return (Target - position).magnitude < targetMargin;
            }

            public void Stop()
            {
                path.Clear();
                navNesh.ResetPath();
                Model.Animator.SetFloat("Speed", 0);
            }

            /* Rotate */
            public void Rotate(Transform target)
            {
                Rotate(target.position, true);
            }

            public void Rotate(Vector3 target)
            {
                Rotate(target, true);
            }

            void Rotate(Vector3 target, bool resetPath)
            {
                Scheduler.Cancel("Rotate");
                Scheduler.Cancel("EndRotation");

                Target = target;
                turning = true;

                if (resetPath)
                {
                    path.Clear();
                    navNesh.ResetPath();
                }

                Model.Animator.SetFloat("Speed", 0);
                Model.Animator.SetBool("Rotate", false);

                Scheduler.Start("Rotate");
            }

            /* ExecuteActions */
            public virtual void ExecuteAction(AgentAction action)
            {
                Scheduler.Cancel("Goto");
                Scheduler.Cancel("Rotate");
                Scheduler.Cancel("ExecuteAction");

                SetDefault();
                CurrentAction = action;

                Scheduler.Start("ExecuteAction");
            }

            protected virtual void ExecuteAction()
            {
                if (CurrentAction.animation != "")
                {
                    Model.Anim(CurrentAction.animation);
                }
                if (CurrentAction.path.Count > 0)
                {
                    Navigate(CurrentAction.path);
                }
                if (CurrentAction.audio != null)
                {
                    PlayAudio(CurrentAction.audio);
                }
                if (CurrentAction == randomWalk)
                {
                    randomWalk.path = new List<Vector3>() { new Vector3(StartPosition.x + Random.Range(-randomWalkRange, randomWalkRange),
                StartPosition.y, StartPosition.z + Random.Range(-randomWalkRange, randomWalkRange))};
                }
            }

            public void SetPrefabAttributes(GameObject model, float minTimeForRandom, float maxTimeForRandom, AgentAction idleAction, AgentAction defaultAction, bool enableRandomAction, float movementSpeed, float maxAllowedFarDistance, bool enableMovement, float randomWalkRange, bool enableRandomWalk, float rotationTime, AudioClip defaultClip)
            {
                if(Model == null) { Model = this.model; }
                Model.ReplaceModel(model);
                this.minTimeForRandom = minTimeForRandom;
                this.maxTimeForRandom = maxTimeForRandom;
                this.idleAction = idleAction;
                this.defaultAction = defaultAction;
                this.enableRandomAction = enableRandomAction;
                this.movementSpeed = movementSpeed;
                this.maxAllowedFarDistance = maxAllowedFarDistance;
                this.enableMovement = enableMovement;
                this.randomWalkRange = randomWalkRange;
                this.enableRandomWalk = enableRandomWalk;
                this.rotationTime = rotationTime;
                this.defaultClip = defaultClip;
            }
        }
    }
}