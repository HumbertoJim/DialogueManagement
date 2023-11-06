using System.Collections.Generic;
using System;
using UnityEngine;

namespace ScheduleManagement
{
    namespace Managers
    {
        public abstract class ScheduleManager : MonoBehaviour
        {
            protected Schedulers.Scheduler Scheduler { get; private set; }

            protected virtual void Awake()
            {
                Scheduler = new Schedulers.Scheduler();
            }

            protected virtual void Update()
            {
                Scheduler.Process();
            }
        }
    }

    namespace Schedulers
    {
        public class Scheduler
        {
            readonly Dictionary<string, Schedulables.Schedulable> schedulables;

            public Scheduler()
            {
                schedulables = new Dictionary<string, Schedulables.Schedulable>();
            }

            public void Process()
            {
                foreach (Schedulables.Schedulable schedulable in schedulables.Values) schedulable.Process();
            }

            public void Add(string name, Schedulables.Schedulable schedulable)
            {
                schedulables.Add(name, schedulable);
            }

            public void Start(string name)
            {
                schedulables[name].Start();
            }

            public void Start(string name, float time)
            {
                schedulables[name].Start(time);
            }

            public void Cancel(string name)
            {
                schedulables[name].Cancel();
            }

            public void Finish(string name)
            {
                schedulables[name].Finish();
            }

            public bool IsActive(string name)
            {
                return schedulables[name].Active;
            }
        }
    }

    namespace Schedulables
    {
        public class Schedulable
        {
            readonly Action function;

            protected float time;

            protected float counter;

            public bool Active { get; protected set; }

            public Schedulable(Action function)
            {
                time = 0;
                this.function = function;
            }

            public Schedulable(float time, Action function)
            {
                this.time = time;
                this.function = function;
            }

            public virtual void Start()
            {
                counter = 0;
                Active = true;
            }

            public virtual void Start(float time)
            {
                this.time = time;
                Start();
            }

            public virtual void Finish()
            {
                Active = false;
                counter = 0;
                function();
            }

            public virtual void Cancel()
            {
                Active = false;
                counter = 0;
            }

            public void Process()
            {
                if (Active)
                {
                    counter += Time.deltaTime;
                    if (counter > time)
                    {
                        Finish();
                    }
                }
            }
        }

        public class NonStopSchedulable : Schedulable
        {
            public NonStopSchedulable(float time, Action function) : base(time, function) { }

            public override void Finish()
            {
                base.Finish();
                Active = true;
            }
        }

        public class RandomSchedulable : Schedulable
        {
            readonly float min_time;
            readonly float max_time;

            public RandomSchedulable(float min_time, float max_time, Action function) : base(function)
            {
                this.min_time = min_time;
                this.max_time = max_time;
            }

            public override void Start()
            {
                time = time = UnityEngine.Random.Range(min_time, max_time);
                base.Start();
            }
        }

        public class AdditiveSchedulable : Schedulable
        {
            public AdditiveSchedulable(Action function) : base(0, function) { }

            public override void Start(float time)
            {
                this.time += time;
                Active = true;
            }

            public override void Finish()
            {
                time = 0;
                base.Finish();
            }
        }
    }
}