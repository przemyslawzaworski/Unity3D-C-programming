using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrippingParticles : MonoBehaviour
{
	public int Count = 128;
	public float Speed = 0.2f;
	public Vector2 Lifetime = new Vector2(0.5f, 3.0f);
	[SerializeField] private Mesh _Mesh;

	class Particle
	{
		public Vector3 Position;
		public float Lifetime;
		public MeshCollider Collider;
		public ClosestPoint Point;
	};

	List<Particle> _Particles;
	Material _Material;
	Vector3 _Scale;

	void Start()
	{
		_Particles = new List<Particle>();
		_Material = new Material(Shader.Find("Sprites/Default"));
		_Material.SetColor("_Color", Color.blue);
		_Scale = new Vector3(0.05f, 0.05f, 0.05f);
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				if (_Particles.Count >= Count)
				{
					_Particles[0].Point.Release();
					_Particles.RemoveAt(0);
				}
				Particle particle = new Particle();
				particle.Position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				particle.Lifetime = UnityEngine.Random.Range(Lifetime.x, Lifetime.y);
				particle.Collider = hit.collider as MeshCollider;
				particle.Point = new ClosestPoint(particle.Collider);
				_Particles.Add(particle);
			}
		}
		for (int i = (_Particles.Count - 1); i > -1; i--)
		{
			Particle particle = _Particles[i];
			if (particle.Lifetime > 0.0f)
			{
				particle.Lifetime = particle.Lifetime - Time.deltaTime;
				Vector3 position = new Vector3(particle.Position.x, particle.Position.y, particle.Position.z);
				position.y -= Speed * Time.deltaTime;
				particle.Position = particle.Point.Update(particle.Collider, position);
			}
			else
			{
				particle.Point.Release();
				_Particles.RemoveAt(i);
			}
		}
	}

	void OnRenderObject()
	{
		if (_Particles == null) return;
		_Material.SetPass(0);
		for (int i = 0; i < _Particles.Count; i++)
		{
			Matrix4x4 trs = Matrix4x4.TRS(_Particles[i].Position, Quaternion.identity, _Scale);
			Graphics.DrawMeshNow(_Mesh, trs);
		}
	}

	void OnDestroy()
	{
		for (int i = 0; i < _Particles.Count; i++)
		{
			_Particles[i].Point.Release();
		}
		Destroy(_Material);
	}
}