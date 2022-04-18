// NetworkServer.cs - generates red texture and send to NetworkClient.cs
// NetworkClient.cs - generates blue texture and send to NetworkServer.cs
using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;

public class NetworkClient : MonoBehaviour
{
	TcpClient _Client = null;
	Thread _Thread = null; 
	string _IPAddress = "127.0.0.1";
	string _Port = "8052";
	string _Message = "";
	Texture2D _Texture = null;
	byte[] _Bytes = new byte[1024 * 1024 * 4];
	bool _Reload = false;

	void ConnectToServer() 
	{
		if (_Thread != null) return;
		_Thread = new Thread (new ThreadStart(NetworkReader));
		_Thread.IsBackground = true;
		_Thread.Start();
	}

	void NetworkReader() 
	{
		try
		{
			_Client = new TcpClient(_IPAddress, Int32.Parse(_Port));
			_Message = "Client is connected";
			byte[] bytes = new byte[_Bytes.Length];
			while (true) 
			{
				NetworkStream networkStream = _Client.GetStream();
				if (networkStream.CanRead)
				{
					int length;
					while ((length = networkStream.Read(bytes, 0, bytes.Length)) != 0)
					{
						_Bytes = new byte[length];
						Array.Copy(bytes, 0, _Bytes, 0, length); 
						_Reload = true;
					}
				}
				networkStream.Close();
			}
		}
		catch (SocketException socketException) 
		{
			_Message = "Socket exception: " + socketException;
		}
	}

	void NetworkWriter() 
	{
		if (_Client == null) return;
		try
		{
			NetworkStream networkStream = _Client.GetStream();
			if (networkStream.CanWrite) 
			{
				byte[] bytes = GenerateTexture();
				networkStream.Write(bytes, 0, bytes.Length);
			}
		}
		catch (SocketException socketException) 
		{
			_Message = "Socket exception: " + socketException;
		}
	}

	void LoadTexture(byte[] bytes)
	{
		if (_Texture != null) Destroy(_Texture);
		_Texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
		_Texture.LoadImage(bytes);
	}

	byte[] GenerateTexture()
	{
		if (_Texture != null) Destroy(_Texture);
		_Texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
		for (int y = 0; y < _Texture.height; y++)
		{
			for (int x = 0; x < _Texture.width; x++)
			{
				_Texture.SetPixel(x, y, Color.blue);
			}
		}
		_Texture.Apply();
		return _Texture.EncodeToPNG();
	}

	void OnGUI()
	{
		_IPAddress = GUI.TextField(new Rect(10, 10, 100, 20), _IPAddress, 25);
		_Port = GUI.TextField(new Rect(150, 10, 100, 20), _Port, 25);
		if (GUI.Button(new Rect(300, 10, 100, 30), "Connect")) ConnectToServer();
		if (GUI.Button(new Rect(450, 10, 100, 30), "Send Data")) NetworkWriter();
		GUI.Label(new Rect(10, 50, 700, 20), _Message);
		if (_Texture != null)
		{
			GUI.DrawTexture(new Rect(256, 256, 256, 256), _Texture, ScaleMode.ScaleToFit, true, 1.0F);
		}
		if (_Reload)
		{
			LoadTexture(_Bytes);
			_Reload = false;
		}
	}
}