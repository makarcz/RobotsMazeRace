using System;
using System.Collections.Generic;
using System.Text;

namespace RobEnvMK
{
    class CustomEventHandler
    {
        public CustomEventHandler()
        {
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(RobotProperties rp)
        {
            m_RobotProperties = rp;
        }

        private RobotProperties m_RobotProperties;

        public RobotProperties RobotProperties
        {
            get { return m_RobotProperties; }
            set { m_RobotProperties = value; }
        }
    }

    public class CustomEventPublisher
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            EventHandler<CustomEventArgs> handler = RaiseCustomEvent;

            if (null != handler)
            {
                handler(this, e);
            }
        }
    }

}
