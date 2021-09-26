using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DeliveryAPI.UserService.Utils
{
    public interface IStateWatcher
    {
        void AddWatcher(string id, DateTime startTime, DateTime endTime);
        void RemoveWatcher(string id);

        delegate void OrderExpiredHandler(string orderId);
        event OrderExpiredHandler OrderExpired;
    }
}
