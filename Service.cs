using System;

namespace SimpleScheduler
{
    public abstract class Service : IService
    {
        protected Service(string id, DateTime executeTime)
        {
            Id = id;
            ExecuteTime = executeTime;
        }

        public string Id { get; }

        public DateTime ExecuteTime { get; }

        public abstract void OnStart();
    }
}
