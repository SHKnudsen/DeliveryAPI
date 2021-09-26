using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DeliveryAPI.UserService.Utils
{
    /// <summary>
    /// Utility class to keep track of orders.
    /// Orders have an Access window, if the order has
    /// not been completed before that window is up
    /// the order should expire.
    /// This class keeps track of all created orders
    /// and automatically notifies any listeners when
    /// an order is expired.
    /// </summary>
    public class OrderStateWatcher : IStateWatcher
    {
        private HashSet<OrderTimer> watchers = new HashSet<OrderTimer>();
        
        /// <summary>
        /// Event to fire when an order has expired
        /// </summary>
        public event IStateWatcher.OrderExpiredHandler OrderExpired;

        /// <summary>
        /// Creates a new <see cref="OrderTimer"/> and adds
        /// it to the active watchers.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void AddWatcher(string id, DateTime startTime, DateTime endTime)
        {
            var timeSpan = endTime - startTime;
            var expireTimer = new Timer(timeSpan.TotalMilliseconds);
            var orderTimer = new OrderTimer(id, expireTimer);
            orderTimer.Expired += OnOrderTimerExpired;
            watchers.Add(orderTimer);
        }

        /// <summary>
        /// Removes an active watcher and unsubscribe it
        /// </summary>
        /// <param name="id"></param>
        public void RemoveWatcher(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var orderTimer = watchers.FirstOrDefault(x => x.OrderId == id);
            if (orderTimer is null)
            {
                return;
            }

            orderTimer.Expired -= OnOrderTimerExpired;
            watchers.Remove(orderTimer);
        }

        private void OnOrderTimerExpired(object sender, OrderTimer e)
        {
            OrderExpired?.Invoke(e.OrderId);
            e.Expired -= OnOrderTimerExpired;
            watchers.Remove(e);
        }
    }

    /// <summary>
    /// Helper class to associate a timer with an order id
    /// </summary>
    public class OrderTimer
    {
        public string OrderId { get; private set; }
        public Timer ExpireTimer { get; private set; }

        internal event EventHandler<OrderTimer> Expired;

        public OrderTimer(string orderId, Timer expireTimer)
        {
            OrderId = orderId;
            ExpireTimer = expireTimer;
            StartTimer();
        }

        private void StartTimer()
        {
            ExpireTimer.Elapsed += ExpireTimer_Elapsed;
            ExpireTimer.Enabled = true;
        }

        private void ExpireTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ExpireTimer.Stop();
            ExpireTimer.Enabled = false;
            ExpireTimer.Elapsed -= ExpireTimer_Elapsed;
            Expired?.Invoke(null, this);
        }

    }
}
