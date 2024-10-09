using UnityEngine;
using UnityEngine.Pool;

public class Pool : MonoBehaviour
{
	public static Pool Instance;
	private ObjectPool<GameObject> _ObjectPool;
	private GameObject _Sphere;

	GameObject CreateFunc()
	{
		GameObject instance = Instantiate(_Sphere);
		return instance;
	}

	void ActionOnGet(GameObject instance)
	{
		instance.SetActive(true);
	}

	void ActionOnRelease(GameObject instance)
	{
		instance.SetActive(false);
	}

	void ActionOnDestroy(GameObject instance)
	{
		Destroy(instance);
	}

	void TakeSphereFromPool()
	{
		GameObject instance = _ObjectPool.Get();
		instance.transform.position = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
		instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void ReturnSphereToPool(GameObject instance)
	{
		_ObjectPool.Release(instance);
	}

	void Start()
	{
		Instance = this;
		_Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		_Sphere.AddComponent<Rigidbody>();
		_Sphere.AddComponent<SphereController>();
		_ObjectPool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, false, 20, 20);
		InvokeRepeating("TakeSphereFromPool", 0f, 0.2f);
	}

	void OnDestroy()
	{
		_ObjectPool.Dispose();
		Instance = null;
	}
}

public class SphereController : MonoBehaviour
{
	void Start()
	{
		this.name = "Instance";
	}

	void Update()
	{
		if (transform.position.y < -20.0f)
		{
			Pool.Instance.ReturnSphereToPool(this.gameObject);
		}
	}
}