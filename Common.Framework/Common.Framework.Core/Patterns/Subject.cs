// <copyright file="Subject.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Collections.Generic;

namespace Common.Framework.Core.Patterns
{
    public abstract class Subject
    {
        protected Subject()
        {
            Observers = new List<Observer>();
        }

        protected List<Observer> Observers { get; set; }

        public virtual void Attach(Observer observer)
        {
            if (!Observers.Contains(observer))
            {
                Observers.Add(observer);
            }
        }

        public virtual void Detach(Observer observer)
        {
            if (Observers.Contains(observer))
            {
                Observers.Remove(observer);
            }
        }

        public virtual void Notify()
        {
            foreach (var observer in Observers)
            {
                observer.Update();
            }
        }
    }
}