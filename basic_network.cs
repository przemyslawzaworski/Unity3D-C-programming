//reference: www.gamasutra.com/blogs/author/ChristianArellano/1000200/
//Make "player" prefab, add basic_network.cs and built-in NetworkIdentity script.
//Then create new GameObject and add built-in Network Manager script.
//In Network Manager, set player prefab to "Player Prefab" field.
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class basic_network : NetworkBehaviour
{
	struct Position
	{
		public int n;
		public float x;
		public float y;
		public float z;
	}

	Queue<KeyCode> queue; 
	Position predicted_state;
	[SyncVar(hook="OnServerStateChanged")] Position server_state;
	[SyncVar] Color color;

	void Start()
	{
		Begin();
		if (isLocalPlayer) 
		{
			queue = new Queue<KeyCode>();
			predicted_state = new Position{n = 0,x = 0,y = 0,z = 0};
		}
		GetComponent<Renderer>().material.color = (isLocalPlayer ? Color.blue : Color.red) ;
	}
	
	[Server]
	void Begin()
	{
		server_state = new Position {n = 0,x = 0,y = 0,z = 0};
	}
	
	[Command]
	void CmdmovementOnServer(KeyCode arrow_key)
	{
		server_state = movement(server_state, arrow_key);
	}

	void OnServerStateChanged(Position new_state)
	{
		server_state = new_state;
		if (queue != null) 
		{
			while (queue.Count > (predicted_state.n - server_state.n)) queue.Dequeue();		
			UpdatePredictedState();
		}
	}

	void UpdatePredictedState()
	{
		predicted_state = server_state;
		foreach (KeyCode arrow_key in queue) 
		{
			predicted_state = movement(predicted_state, arrow_key);
		}
	}

	void Update()
	{
		if (isLocalPlayer) 
		{
			KeyCode[] arrow_keys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow };
			foreach (KeyCode arrow_key in arrow_keys) 
			{
				if (!Input.GetKey(arrow_key)) continue;
				queue.Enqueue(arrow_key);
				UpdatePredictedState(); 					  
				CmdmovementOnServer(arrow_key);
			}
		}
		Position state = isLocalPlayer ? predicted_state : server_state;
		transform.position = new Vector3(state.x, state.y, state.z);
	}
	
	Position movement(Position p, KeyCode arrow_key)
	{
		float dx = 0;
		float dy = 0;
		float dz = 0;
		switch (arrow_key)
		{
			case KeyCode.UpArrow:
				dz = Time.deltaTime;
				break;
			case KeyCode.DownArrow:
				dz = -Time.deltaTime;
				break;
			case KeyCode.RightArrow:
				dx = Time.deltaTime;
				break;
			case KeyCode.LeftArrow:
				dx = -Time.deltaTime;
				break;
		}
		return new Position {n = 1 + p.n, x = dx + p.x, y = dy + p.y, z = dz + p.z};
	}
}