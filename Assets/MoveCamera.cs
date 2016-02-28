
// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;
//using System.Net.WebSockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using System.Threading;
using System.Net;
using System.IO;
using LitJson;
using System;
//using Newtonsoft.Json;
using UnityEngine.Networking;

public class MoveCamera : MonoBehaviour 
{
	

	public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
	public float panSpeed = 4.0f;		// Speed of the camera when being panned
	public float zoomSpeed = 4.0f;		// Speed of the camera going back and forth

	private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts

	private bool isZooming;		// Is the camera zooming?
	//private double velx,vely,velz;	
	//private double nposx, nposy, nposz; 

	WebSocket ws=null;
	//ClientWebSocket webSocket = null;
	float velx=0, vely=0, velz=0, nposx=0, nposy=0, nposz=0;
	string action = "false";
	float sign = 1, velMag = 1;
	~ MoveCamera()
	{
		ws.Close ();
	}
	void Start() {
		ws = new WebSocket ("ws://127.0.0.1:9999");
		ws.Connect ();
		//Connect("ws://127.0.0.1:8888").Wait();
	}

	/*public static async Task Connect(string uri)
	{
		try
		{
			webSocket = new ClientWebSocket();
			await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
			await Task.WhenAll(Recieve(webSocket));
		}
		catch (Exception ex)
		{
			Console.WriteLine("Exception: {0}", ex);
		}
		finally
		{
			if (webSocket != null)
				webSocket.Dispose();
		}



	}

	public static async Task Recieve(ClientWebSocket webSocket)
	{
		byte[] buffer = new byte[1024];
		while (webSocket.State == WebSocketState.Open)
		{
			var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			string strdata = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
			string[] words = strdata.Split('}');
			string json = words[0] + '}';
			Console.WriteLine("Receive: " + json);
			if (result.MessageType == WebSocketMessageType.Close)
			{
				await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
			}

		}
	}
*/
	//bool PAUSE=false,MOVE=true;
/*	void readValues()
	{
		velz = 0.12;
		velx = 0.7;
		vely = 0.2;
		nposx = 0.8;
		nposy = 0.9;
		nposz = 1.5;
	}
	*/
	string estimateAction(float velx, float vely, float velz, float nposx, float nposy, float nposz)
	{
		string action;
		Vector3 velVec = new Vector3 (velx, vely, velz);
		Vector3 nposVec = new Vector3 (nposx,nposy,nposz);
		float angle = Vector3.Angle (velVec, nposVec);
		//float sign = Mathf.Sign (Vector3.Dot (velVec, nposVec));
		//angle = sign * angle;
		//print(angle);
		//float yang = Vector3.Angle (vely, velz);
		if (nposx == 0.0f && nposy == 0.0f && nposz == 0.0f) {
			action = "false";
			return action;
		}
		if (angle < 45.0f || angle > 135.0f) {
			action = "translate";
			return action;
		}
		else {
			action = "rotate";
			return action;
		}

		
	}

	void Update ()
	{
		//Vector3 velVec = new Vector3(0,0,0);
		//Vector3 nposVec = new Vector3(0,0,0);

			ws.OnMessage += (sender, e) => {
			
			//print (e.Data);
				//dynamic stuff = JsonConvert.DeserializeObject(e.Data);
				//			JSONObject json = JSONObject.StringObject(e.Data);
				//			print(json.GetField("normal_x"));

				JsonData data = JsonMapper.ToObject (e.Data);
			//print(data.ToString());
				//print ("writng data");
				 velx = float.Parse(data["velocity_x"].ToString());
				 vely = float.Parse(data["velocity_y"].ToString());
				 velz = float.Parse(data["velocity_z"].ToString());
				 nposx = float.Parse(data["normal_x"].ToString());
				 nposy = float.Parse(data["normal_y"].ToString());
				 nposz = float.Parse(data["normal_z"].ToString());

			};

		//float velMag = velVec.magnitude;
		//float sign = Mathf.Sign (Vector3.Dot (velVec, nposVec));*/

		/*velz = 1.0f;
		velx = 0.0f;
		 vely = 0.0f;
		 nposx = 0.0f;
		 nposy = 0.0f;
		 nposz = 1.0f;*/
		print (velx);
		Vector3 velVec = new Vector3 (velx, vely, velz);
		Vector3 nposVec = new Vector3 (nposx, nposy, nposz);
		action = estimateAction (velx, vely, velz, nposx, nposy, nposz);
		//action = "translate";

		//velVec.Normalize();
		//float velMagnitude = 0.1f;
		//velVec = velMagnitude * velVec;
		//nposVec.Normalize ();

		//action = estimateAction (velx1, vely1, velz1, nposx1, nposy1, nposz1);
		//print(action);
		//action = "translate";

		//velVec.Normalize();
		//float velMagnitude = 1.0f;
		//velVec = velMagnitude * velVec;
		//nposVec.Normalize ();

		velMag = velVec.magnitude;
		sign = Mathf.Sign (Vector3.Dot (velVec, nposVec));

		// Move the camera on it's XY plane
		if (action == "translate") {
			// motion in x 
			//readValues();
			//Vector3 location =  new Vector3((int)(nposx*velx), (int)(nposy*vely), (int)(nposz*velz));
			Vector3 trans = sign * velMag * nposVec * 0.001f;

			transform.Translate (trans * Time.deltaTime,Space.Self);
		}

		else if (action == "rotate") {
			Vector2 velXY = new Vector2 (velVec.x, velVec.y);
			velXY.Normalize ();
			Vector2 perpvelXY = new Vector2 (-velXY.y, velXY.x);
			Vector3 rot = velMag * perpvelXY * 0.001f;
			//Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
			Vector3 offset = new Vector3 (0, 0, 1);
			transform.RotateAround ((transform.position + offset), rot, Time.deltaTime);

		} 
	}
}