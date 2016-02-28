using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using System.Threading;
using System.Net;
using System.IO;
using LitJson;
//using Boomlagoon.JSON;

//using Newtonsoft.Json;
using UnityEngine;
using System;
//using Newtonsoft.Json;
using UnityEngine.Networking;

public class webClient : MonoBehaviour {

	// Use this for initialization
	//Stream data;

	WebSocket ws=null;
	void Start () {
		//print ("Hello VTHAcks");
		ws = new WebSocket ("ws://127.0.0.1:8888");
			ws.Connect ();
			//Connect("ws://127.0.0.1:8888").Wait();
			//print ("Hello VTHAcks1");
	
	}
	void Update () {
		
		ws.OnMessage += (sender, e) => {
			print("wiritng data");
			//dynamic stuff = JsonConvert.DeserializeObject(e.Data);
//			JSONObject json = JSONObject.StringObject(e.Data);
//			print(json.GetField("normal_x"));
			JsonData data = JsonMapper.ToObject(e.Data);
			print(data["normal_x"]);
			//print(e.Data);
			};
		}

}

