using System.Collections;
using Interfaces;

namespace Observer
{
    public abstract class Subject : ISubject
    {
        private bool _skipNotify = false;

        protected ArrayList _observers = new ArrayList();

        public void AddObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void AddObserver(ArrayList observer)
        {
            foreach (IObserver aObserver in observer)
            {
                AddObserver(aObserver);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            if (_skipNotify)
                return;

            foreach (IObserver observer in _observers)
            {
                observer.Update(this);
            }
        }

        public void DisableNotify()
        {
            _skipNotify = true;
        }

        public void EnableNotify()
        {
            _skipNotify = false;
        }

        public ArrayList getObservers()
        {
            return _observers;
        }
    }
}