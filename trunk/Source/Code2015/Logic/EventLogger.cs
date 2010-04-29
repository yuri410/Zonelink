using System;
using System.Collections.Generic;
using System.Text;
using Code2015.BalanceSystem;

namespace Code2015.Logic
{
    enum EventType
    {
        Strike,
        Food,
        Oil,
        Wood,
        Count
    }

    struct EventEntry
    {
        public SimulationObject Object;
        public EventType Type;
    }
    delegate void EventLogHandler(EventEntry e);
    class EventLogger
    {
        static EventLogger singleton;

        public static EventLogger Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new EventLogger();
                return singleton;
            }
        }

        private EventLogger() { }

        public void Log(EventType type, SimulationObject obj)
        {
            if (NewLog != null)
            {
                EventEntry e;
                e.Type = type;
                e.Object = obj;
                NewLog(e);
            }
        }

        public event EventLogHandler NewLog;
    }
}
