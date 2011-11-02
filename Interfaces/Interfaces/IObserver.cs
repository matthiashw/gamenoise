using System.Collections;

namespace Interfaces
{
    public interface ISubject
    {
        void AddObserver(ArrayList observer);
        void AddObserver(IObserver observer);
        void DisableNotify();
        void EnableNotify();
        void Notify();
        void RemoveObserver(IObserver observer);
        ArrayList getObservers();
    }

    public interface IObserver
    {
        void Update(object subject);
    }
}