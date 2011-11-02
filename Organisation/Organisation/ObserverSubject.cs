using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace ObserverSubject
{

    public interface Observer
    {
        void Update(object subject);
    }


    public abstract class Subject
    {
        private ArrayList observers = new ArrayList();

        public void AddObserver(Observer observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer observer in observers)
            {
                observer.Update(this);
            }
        }
    }

}

