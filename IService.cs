using System;

namespace SimpleScheduler
{
    public interface IService
    {
        public string Id { get; }

        public DateTime ExecuteTime { get; }

        public void OnStart();
    }
}
