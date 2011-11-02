using System.Collections;

namespace Observer
{
    public interface ISubject
    {
        void AddObserver(ArrayList observer);
        void AddObserver(IObserver observer);
        void DisableNotify();
        void EnableNotify();
        void Notify();
        void RemoveObserver(IObserver observer);
        ArrayList GetObservers();
    }

    public interface IObserver
    {
        void Update(object subject);
    }
}