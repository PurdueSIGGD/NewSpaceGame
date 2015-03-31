﻿using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {
	private float health = 100; //suit integrity
	private float med = 100; //actual health
	private float startingoxy = 30;
	private float oxy;
	private GameObject text;
	private int wallet;
	private int startingwallet;
	private float startinghealth;
	private bool oxywarning, oxyerror, suitwarning, suiterror, medwarning, cranewarning;
	private static string okmessage = "All systems operational";
	private static string oxymessage = "WARNING: LOW OXYGEN\n";
	private static string oxymessagegone = "WARNING: NO OXYGEN\n";
	private static string suitmessage = "WARNING: LOW SUIT INTEGRITY\n";
	private static string suitmessagegone = "WARNING: SUIT LOST\n";
	private static string medmessage = "WARNING: VITAL SIGNS ARE LOW\n";
	private static string cranemessage = "WARNING: CRANE IS DESTROYED\n";
	private static string empmessage = "WARNING: EMP DETECTED\n";
	private static string empmessage2 = "TIME UNTIL SYSTEM REBOOT: ";
	private bool ejected;
	private bool emergency, on;
	private float timesince, timeerror;
	private float timesincelastdamage;
	private float rechargetime;
	private bool pause;
	public bool acceptingOxy, emp = false; //is oxy less than startingoxy?
	public float emptime = 0;
	// Use this for initialization
	void Start () {
		timesincelastdamage = -1;
		if (PlayerPrefs.HasKey("startingoxy")) {
			startingoxy = PlayerPrefs.GetFloat("startingoxy");
		} else {
			PlayerPrefs.SetFloat("startingoxy",startingoxy);
		}
		if (PlayerPrefs.HasKey ("wallet")) {
			wallet = PlayerPrefs.GetInt("wallet");
			startingwallet =  PlayerPrefs.GetInt("wallet");
		} else {
			PlayerPrefs.SetInt("wallet", wallet);
		}	
		if (PlayerPrefs.HasKey ("health")) {
			health = PlayerPrefs.GetFloat("health");
			//health = 100; //REMOVE LATER
		} else {
			PlayerPrefs.SetFloat("health", health);
		}	
		if (PlayerPrefs.HasKey ("emprechargetime")) {
			//print (PlayerPrefs.GetFloat("emprechargetime"));
			rechargetime = PlayerPrefs.GetFloat("emprechargetime");
		} else {
			PlayerPrefs.SetFloat("emprechargetime", 10);
		}
		timesince = 0;
		startinghealth = health;
		oxy = startingoxy;
		text = GameObject.Find("GuiText");
	}
	
	// Update is called once per frame
	void Update () {

		acceptingOxy = (oxy < startingoxy);
		string words = "";
		if (timesincelastdamage >= 0) {
			timesincelastdamage += Time.deltaTime;
			if (timesincelastdamage > 5 && med < 100) { //regen health

				if (med > 1) med +=  (8 * Time.deltaTime)/med;
				if (med <= 1) med +=  Time.deltaTime;
			}
		}
		if (!(medwarning || oxywarning || oxyerror || suitwarning || suiterror || cranewarning || emp)) {
			emergency = false;
			words += okmessage;

		} else {
 			emergency = true;
			if (emp) words += (empmessage2 +  (rechargetime - emptime).ToString("F2") + "\n" + empmessage);
			if (suitwarning) words += suitmessage;
			if (suiterror) words += suitmessagegone;
			if (oxywarning) words += oxymessage;
			if (oxyerror) words += oxymessagegone;
			if (medwarning) words += medmessage;
			if (cranewarning) words += cranemessage;

		}
		if (emergency) { //flashing lights

			timesince+=Time.deltaTime;
			if (timesince > .25f) {
				if (timesince > .5f) timesince = 0;
				if (emp) {
					words = empmessage2 + (rechargetime - emptime).ToString("F2") + "\n";

				} else {
					words = "";
				}
			}
		}
		string final = 
			"Suit Integrity: " + health.ToString("F2") + "/" + startinghealth.ToString("F2") + "\n" +
				"Oxygen Levels: " + oxy.ToString("F2") + "/" + startingoxy.ToString("F2") + "\n" +
				"Health: " + med.ToString("F2") + "/100.00\n" +
				"Cash: " + wallet + "\n" + 
				words;
		((GUIText)text.GetComponent("GUIText")).text = final;

		if (med != 100 && !pause) {
			this.SendMessage("FaderTime",med/100);
		}
		medwarning = (med < 35);
		suitwarning = (health < 20 && health > 0);
		suiterror = (health <= 0);
		oxyerror  = (oxy <= 0);
		if (emp && med <= 0) emp = false;
		cranewarning = (this.GetComponentInChildren<CraneController>().broken);
		ejected = ((RopeScript2D)GameObject.Find("Ship").GetComponent("RopeScript2D")).ejected || ((RopeScript2D)GameObject.Find("Ship").GetComponent("RopeScript2D")).brokenrope;
		if (ejected) { //change oxygen from being ejected
			if ((health < 50 && health > 1 )|| health == 0) {
				changeOxy(-1 * Time.deltaTime * (50 - health)/10 );
			} else {
				changeOxy(-1 * Time.deltaTime);
			}
		} else {
			changeOxy(3 * 	Time.deltaTime);
		}

		if (oxy < 15) {
			changeMed(-1 * Time.deltaTime * (15-oxy)/3);
			oxywarning = (!oxyerror);
		} else {
			oxywarning = false;
		}

	}

	void changeHealth(float f) { //not actual health, this is just the suit itegrity
		if (!pause) {
			if (health > 1 && health + f > 0) {
				if (med >= 3*f/health) { //your health can be damaged by attacks, but your suit works like armor
					med += 3*f/health;
				} else {
					med = 0;
				}
			} else {
				health = 0;
				changeMed(f);
			}
			if (health != 0) {
				health+=f;
			}
		}
	}
	void changeOxy(float f) {
		if (!pause) {
			if (f + oxy > startingoxy) {
				oxy = startingoxy;
			} else if (f + oxy <= 0) {
				oxy = 0;
			} else {
				oxy += f;
			}
		}
	}
	void changeMed(float f) { //local use ONLY
		if (!pause) {
			timesincelastdamage = 0;
			if (med + f >= 100) {
				med = 100;
			} else if (med+f <=0) {
				med = 0;
			} else {
				med += f;
			}
		}
	}
	void changeWallet(int i) {
		wallet += i;
	}
	void StopDoingThat() {
		pause = true;
	}
	void Im_Leaving() {
		PlayerPrefs.SetInt ("wallet", wallet);
		PlayerPrefs.SetFloat ("health", health);
		PlayerPrefs.SetInt ("startingwallet", startingwallet);
	}
}
