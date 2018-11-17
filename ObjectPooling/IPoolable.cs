namespace Exanite.ObjectPooling
{
    public interface IPoolable
	{
		void OnSpawn();

		void OnDespawn(); // Use this for reseting the GameObject's variables
	}
}