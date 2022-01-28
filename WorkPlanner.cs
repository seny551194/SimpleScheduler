using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

namespace SimpleScheduler
{
    public static class WorkPlanner
    {
        private class ServiceTimeComparer : IComparer<IService>
        {
            public int Compare(IService? x, IService? y)
            {
                if (x.ExecuteTime > y.ExecuteTime)
                    return 1;
                else if (x.ExecuteTime == y.ExecuteTime)
                    return 0;
                else
                    return -1;
            }
        }

        private static readonly ServiceTimeComparer _serviceTimeComparer = new ServiceTimeComparer();
        private static readonly Timer _timer = new Timer();
        private static readonly List<IService> _services = new List<IService>();

        public static void Initialization() =>
            _timer.Elapsed += OnTimerElapsed;

        public static void AddServices(IEnumerable<IService> services)
        {
            if (services == null || services.Count() == 0)
                return;

            IService currentService = _services.FirstOrDefault();

            _services.AddRange(services);

            if (_services.Count > 1)
                _services.Sort(_serviceTimeComparer);

            if (_services[0] != currentService)
                SetCurrent(_services[0]);
        }

        public static void AddService(IService service)
        {
            if (service == null)
                return;

            if (FindServiceById(service.Id) != null)
                throw new Exception($"Service with Id \"{service.Id}\" already exist");

            _services.Add(service);

            if (_services.Count > 1)
                _services.Sort(_serviceTimeComparer);

            if (_services[0] == service)
                SetCurrent(service);
        }

        public static void RemoveService(IService service)
        {
            if (service == null)
                return;

            bool isCurrent = _services.First().Equals(service);

            _services.Remove(service);

            if (isCurrent)
                Next();
        }

        public static IService FindServiceById(string id) =>
            _services.FirstOrDefault(e => e.Id == id);

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e) =>
            StartService(_services.FirstOrDefault());

        public static void StartService(IService service)
        {
            if (service == null)
                return;

            service.OnStart();

            RemoveService(service);
        }

        private static void SetCurrent(IService service)
        {
            _timer.Stop();

            if (service == null)
                return;

            DateTime executeTime = service.ExecuteTime;
            DateTime currentTime = DateTime.UtcNow;

            double waitMilliseconds = (executeTime - currentTime).TotalMilliseconds;

            if (waitMilliseconds <= 0)
            {
                StartService(service);
                return;
            }

            _timer.Interval = waitMilliseconds;
            _timer.Start();
        }

        private static void Next() => SetCurrent(_services.FirstOrDefault());
    }
}