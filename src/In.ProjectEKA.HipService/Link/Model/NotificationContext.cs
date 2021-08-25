using System;
using System.Collections.Generic;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class NotificationContext
    {
        public NotificationPatientContext Patient { get; }

        public NotificationCareContext NotificationCareContext { get; }

        public List<string> HiTypes { get; }

        public DateTime Date { get; }

        public NotificationContextHip Hip { get; }


        public NotificationContext(NotificationPatientContext patient, NotificationCareContext notificationCareContext,
            List<string> hiTypes, DateTime dateTime, NotificationContextHip hip)
        {
            Patient = patient;
            NotificationCareContext = notificationCareContext;
            HiTypes = hiTypes;
            Date = dateTime;
            Hip = hip;
        }
    }
}