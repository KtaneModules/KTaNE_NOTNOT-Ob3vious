using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Newtonsoft.Json;

class Settings
{
	public int basetime = 1250;
	public int plustime = 250;
}

public class notnotScript : MonoBehaviour
{

	public KMAudio Audio;
	public AudioClip[] sounds;
	public KMBombInfo Bomb;
	public KMSelectable[] Button;
	public GameObject[] Bar;
	public GameObject Text;
	public GameObject Base;
	public KMBombModule Module;
	public Material Mat;

	private string state = "dormant";
	private int ans = 0;
	private int target = 0;
	private Settings settings;
	private bool TPmodify = false;

	static int _moduleIdCounter = 1;
	int _moduleID = 0;

	void GetSettings()
	{
		var settingsConfig = new ModConfig<Settings>("NotNot");
		settings = settingsConfig.Settings; // This reads the settings from the file, or creates a new file if it does not exist
		settingsConfig.Settings = settings; // This writes any updates or fixes if there's an issue with the file
	}

	private KMSelectable.OnInteractHandler ButtonPressed(int pos)
	{
		return delegate
		{
			Button[pos].AddInteractionPunch();
			ans = pos + 1;
			if(state == "dormant")
			{
				state = "active";
				StartCoroutine(Run());
			}
			return false;
		};
	}

	void Awake()
	{
		_moduleID = _moduleIdCounter++;
		for (int i = 0; i < Button.Length; i++)
		{
			Button[i].OnInteract += ButtonPressed(i);
			Button[i].GetComponent<Renderer>().enabled = false;
		}
        GetSettings();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private IEnumerator Run()
	{
		Debug.LogFormat("[NOT NOT #{0}] You activated the module. Good luck!", _moduleID);
		int stage = 10;
		bool then = false;
		bool then2 = false;
		int[] current = { 0, 0, 0, 0, 0 };
		string[] deny = { "", "not", "not not\n" };
		string[] dir = { "up", "left", "right", "down", "nothing", "red", "yellow", "green", "blue" };
		string[] op = { "or", "and", "then", "ignore" };
		string[] colassign = { "ffffff", "ff0000", "ffdf00", "00ff00", "007fff" };
		string[] hex = { "ff", "df", "bf", "9f", "7f", "5f", "3f", "1f", "00" };
		ans = 0;
		float t = 0f;
		string gridchars = "0182HP3BDJ4GMT95KVFZX6RCQLAY7EWONUSI";
		string[] dirchars = { "null", "down", "right", "right and down", "left", "left and down", "left and right", "left, right and down", "up", "up and down", "up and right", "up, right and down", "left and up", "left, up and down", "left, up and right", "left, up, right and down", "nothing", "down and nothing", "right and nothing", "right, down and nothing", "left and nothing", "left, down and nothing", "left, right and nothing", "left, right, down and nothing", "up and nothing", "up, down and nothing", "up, right and nothing", "up, right, down and nothing", "left, up and nothing", "left, up, down and nothing", "left, up, right and nothing", "left, up, right, down and nothing", };
		int xpos = 0;
		int ypos = 0;
		int wordcount = 0;
		string display = "";
		for (int i = 0; i < 36; i++)
		{
			if ((Bomb.GetSerialNumber()[0] == gridchars[i]))
			{
				xpos = i % 6;
				ypos = i / 6;
			}
		}
		while (t < 20f)
		{
			t += 1f;
			if (t <= 8f)
			{
				Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f - (t / 8f));
				Text.GetComponent<TextMesh>().text = Text.GetComponent<TextMesh>().text.Replace((hex[(int)t - 1] + "'>"), (hex[(int)t] + "'>"));
			}
			else if (t >= 13f)
			{
				Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, (t - 13f) / 8f);
				Text.GetComponent<TextMesh>().text = Text.GetComponent<TextMesh>().text.Replace((hex[21 - (int)t] + "'>"), (hex[20 - (int)t] + "'>"));
			}
			if ((int)t == 10)
			{
				current[0] = Rnd.Range(0, 3);
				current[1] = Rnd.Range(0, 9);
				current[2] = Rnd.Range(0, 4);
				current[3] = Rnd.Range(0, 3);
				current[4] = Rnd.Range(0, 9);
				if (!(current[2] == 3))
				{
					Text.GetComponent<TextMesh>().text = deny[current[0]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[1]] + "</color>\n" + op[current[2]] + "\n" + deny[current[3]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[4]] + "</color>";
					display = deny[current[0]] + " " + dir[current[1]] + " " + op[current[2]] + " " + deny[current[3]] + " " + dir[current[4]];
					wordcount = current[0] + current[3] + 3;
				}
				else
				{
					Text.GetComponent<TextMesh>().text = deny[current[0]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[1]] + "</color>";
					display = deny[current[0]] + " " + dir[current[1]];
					wordcount = current[0] + 1;
				}
				if (current[2] == 2)
				{
					then = true;
				}
			}
			UpdateBar(t / 20f);
			yield return new WaitForSeconds(0.02f);
		}
		Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
		while (stage > 0)
		{
			Debug.LogFormat("[NOT NOT #{0}] Statement displayed is: '{1}' at {2}. Valid are: {3}.", _moduleID, ("x " + display.ToUpperInvariant().Replace("\n"," ").Replace("  ", " ")).Replace("x ", "x").Replace("x ", "x").Replace("x", ""), gridchars[xpos + 6 * ypos], dirchars[Check(current, xpos, ypos)]);
			t = (wordcount * settings.plustime + settings.basetime) / 20f;
			while (t > 0f && ans == 0)
			{
				t -= 1f;
				UpdateBar(20f * t / (wordcount * settings.plustime + settings.basetime));
				yield return new WaitForSeconds(0.02f);
			}
			
			int[] answer = { 16, 8, 4, 2, 1 };
			if ((answer[ans] & Check(current, xpos, ypos)) == 0)
			{
				Debug.LogFormat("[NOT NOT #{0}] You chose {1}, which is invalid.", _moduleID, dirchars[answer[ans]]);
				Module.HandleStrike();
				state = "dormant";
				yield break;
			}
			Audio.PlaySoundAtTransform("Woosh", Module.transform);
			Debug.LogFormat("[NOT NOT #{0}] You chose {1}, which is valid.", _moduleID, dirchars[answer[ans]]);
			if (ans == 1)
				ypos--;
			else if(ans == 2)
				xpos--;
			else if (ans == 3)
				xpos++;
			else if (ans == 4)
				ypos++;
			t = 0f;
			while (t < 20f)
			{
				t += 1f;
				if (t <= 8f)
				{
					Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f - ((t + 1f) / 8f));
					Text.GetComponent<TextMesh>().text = Text.GetComponent<TextMesh>().text.Replace((hex[(int)t - 1] + "'>"), (hex[(int)t] + "'>"));
				}
				else if (t >= 13f)
				{
					Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, (t - 12f) / 8f);
					Text.GetComponent<TextMesh>().text = Text.GetComponent<TextMesh>().text.Replace((hex[21 - (int)t] + "'>"), (hex[20 - (int)t] + "'>"));
				}
				if ((int)t == 10)
				{
					if (stage != 1)
					{
						if (then)
						{
							current[0] = current[3];
							current[1] = current[4];
							current[2] = Rnd.Range(2, 4);
							current[3] = Rnd.Range(0, 3);
							current[4] = Rnd.Range(0, 9);
							if (!(current[2] == 3) && !then2)
							{
								Text.GetComponent<TextMesh>().text = op[current[2]] + "\n" + deny[current[3]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[4]] + "</color>";
								display = op[current[2]] + " " + deny[current[3]] + " " + dir[current[4]];
								then2 = true;
								wordcount = current[3] + 2;
							}
							else
							{
								current[2] = 3;
								Text.GetComponent<TextMesh>().text = "";
								display = "";
								then = false;
								wordcount = 0;
							}
						}
						else
						{
							current[0] = Rnd.Range(0, 3);
							current[1] = Rnd.Range(0, 9);
							current[2] = Rnd.Range(0, 4);
							current[3] = Rnd.Range(0, 3);
							current[4] = Rnd.Range(0, 9);
							if (!(current[2] == 3))
							{
								Text.GetComponent<TextMesh>().text = deny[current[0]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[1]] + "</color>\n" + op[current[2]] + "\n" + deny[current[3]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[4]] + "</color>";
								display = deny[current[0]] + " " + dir[current[1]] + " " + op[current[2]] + " " + deny[current[3]] + " " + dir[current[4]];
								wordcount = current[0] + current[3] + 3;
							}
							else
							{
								Text.GetComponent<TextMesh>().text = deny[current[0]] + " <color='#" + colassign[Rnd.Range(0, 5)] + "00'>" + dir[current[1]] + "</color>";
								display = deny[current[0]] + " " + dir[current[1]];
								wordcount = current[0] + 1;
							}
							if (current[2] == 2)
							{
								then = true;
								then2 = false;
							}
						}
					}
					else
					{
						Text.GetComponent<TextMesh>().text = "NOT NOT";
					}
				}
				UpdateBar(t / 20f);
				yield return new WaitForSeconds(0.02f);
			}
			Text.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
			stage--;
			ans = 0;
		}
		Base.GetComponentInChildren<MeshRenderer>().material = Mat;
		for (int i = 0; i < 4; i++)
		{
			Bar[i].GetComponent<MeshRenderer>().material.color = new Color(0.6875f, 0.5f, 0f, 1f);
		}
		Text.GetComponent<TextMesh>().color = new Color(0.6875f, 0.5f, 0f, 1f);
		state = "solved";
		Module.HandlePass();
	}



	private void UpdateBar(float num)
	{
		if (num >= 39f / 49f)
		{
			Bar[0].transform.localScale = new Vector3(((num) - (39f / 49f)) * (49f / 10f) * 10f, 1f, 1f);
			Bar[0].transform.localPosition = new Vector3((((num) - (39f / 49f)) * (49f / 10f) * 5f) - 6.5f, 0f, 7f);
			Bar[1].transform.localScale = new Vector3(1f, 1f, 14f);
			Bar[1].transform.localPosition = new Vector3(-7f, 0f, 0.5f);
			Bar[2].transform.localScale = new Vector3(14f, 1f, 1f);
			Bar[2].transform.localPosition = new Vector3(-0.5f, 0f, -7f);
			Bar[3].transform.localScale = new Vector3(1f, 1f, 11f);
			Bar[3].transform.localPosition = new Vector3(7f, 0f, -2f);
		}
		else if (num >= 25f / 49f)
		{
			Bar[0].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[0].transform.localPosition = new Vector3(-1.5f, 0f, 7f);
			Bar[1].transform.localScale = new Vector3(1f, 1f, ((num) - (25f / 49f)) * (49f / 14f) * 14f);
			Bar[1].transform.localPosition = new Vector3(-7f, 0f, (((num) - (25f / 49f)) * (49f / 14f) * 7f) - 6.5f);
			Bar[2].transform.localScale = new Vector3(14f, 1f, 1f);
			Bar[2].transform.localPosition = new Vector3(-0.5f, 0f, -7f);
			Bar[3].transform.localScale = new Vector3(1f, 1f, 11f);
			Bar[3].transform.localPosition = new Vector3(7f, 0f, -2f);
		}
		else if (num >= 11f / 49f)
		{
			Bar[0].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[0].transform.localPosition = new Vector3(-1.5f, 0f, 7f);
			Bar[1].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[1].transform.localPosition = new Vector3(-7f, 0f, 0.5f);
			Bar[2].transform.localScale = new Vector3(((num) - (11f / 49f)) * (49f / 14f) * 14f, 1f, 1f);
			Bar[2].transform.localPosition = new Vector3(6.5f - (((num) - (11f / 49f)) * (49f / 14f) * 7f), 0f, -7f);
			Bar[3].transform.localScale = new Vector3(1f, 1f, 11f);
			Bar[3].transform.localPosition = new Vector3(7f, 0f, -2f);
		}
		else
		{
			Bar[0].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[0].transform.localPosition = new Vector3(-1.5f, 0f, 7f);
			Bar[1].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[1].transform.localPosition = new Vector3(-7f, 0f, 0.5f);
			Bar[2].transform.localScale = new Vector3(0f, 0f, 0f);
			Bar[2].transform.localPosition = new Vector3(-0.5f, 0f, -7f);
			Bar[3].transform.localScale = new Vector3(1f, 1f, (num) * (49f / 11f) * 11f);
			Bar[3].transform.localPosition = new Vector3(7f, 0f, 3.5f - ((num) * (49f / 11f) * 5.5f));
		}
	}

	private int Check(int[] statement, int x, int y)
	{
		int[] noncol = { 8, 4, 2, 1 };
		//format y,x
		int[,] doorsh =
		{
			{ -1, -1, -1, -1, -1, -1 },
			{  1,  0,  0,  0,  3,  4 },
			{  4,  1,  3,  1,  3,  1 },
			{ -1,  0,  2,  1,  0, -1 },
			{  2,  4,  2,  4,  2,  3 },
			{  3,  4,  0,  0,  0,  2 },
			{ -1, -1, -1, -1, -1, -1 }
		};
		int[,] doorsv =
		{
			{ -1,  1,  3, -1,  2,  4, -1 },
			{ -1,  2,  2,  0,  4,  0, -1 },
			{ -1,  0,  3,  3,  1,  0, -1 },
			{ -1,  0,  2,  4,  4,  0, -1 },
			{ -1,  0,  3,  0,  1,  1, -1 },
			{ -1,  3,  1, -1,  4,  2, -1 }
		};
		int doors = 16;
		if (doorsh[y, x] != -1)
			doors += 8;
		if (doorsv[y, x] != -1)
			doors += 4;
		if (doorsv[y, x + 1] != -1)
			doors += 2;
		if (doorsh[y + 1, x] != -1)
			doors += 1;

		int state1 = 0;
		if (statement[1] <= 4)
		{
			if(statement[1] == 4)
			{
				if (statement[0] != 1)
					state1 = 16;
				else
					state1 = 15;
			}
			else
			{
				state1 = noncol[statement[1]];
				if (statement[0] == 1)
					state1 = 15 - state1;
			}
		}
		else
		{
			if (doorsh[y, x] == statement[1] - 4)
				state1 += 8;
			if (doorsv[y, x] == statement[1] - 4)
				state1 += 4;
			if (doorsv[y, x + 1] == statement[1] - 4)
				state1 += 2;
			if (doorsh[y + 1, x] == statement[1] - 4)
				state1 += 1;

			if (statement[0] == 1)
				state1 = 15 - state1;
		}

		int state2 = 0;
		if (statement[4] <= 4)
		{
			if (statement[4] == 4)
			{
				if (statement[3] != 1)
					state2 = 16;
				else
					state2 = 15;
			}
			else
			{
				state2 = noncol[statement[4]];
				if (statement[3] == 1)
					state2 = 15 - state2;
			}
		}
		else
		{
			if (doorsh[y, x] == statement[4] - 4)
				state2 += 8;
			if (doorsv[y, x] == statement[4] - 4)
				state2 += 4;
			if (doorsv[y, x + 1] == statement[4] - 4)
				state2 += 2;
			if (doorsh[y + 1, x] == statement[4] - 4)
				state2 += 1;

			if (statement[3] == 1)
				state2 = 15 - state2;
		}

		int totalstate;
		if (statement[2] == 0)
			totalstate = doors & (state1 | state2);
		else if (statement[2] == 1)
			totalstate = doors & (state1 & state2);
		else
			totalstate = doors & state1;

		if (totalstate == 0)
			totalstate = 16;

		target = totalstate;
		return totalstate;
	}

#pragma warning disable 414
	private string TwitchHelpMessage = "'!{0} start' to activate the module, '!{0} l/u/r/d' to press a direction. e.g. '!{0} l'";
#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
	{
		yield return null;
        if (!TPmodify)
        {
			settings.basetime += 15000;
			TPmodify = true;
		}
		command = command.ToLowerInvariant();
		if (command == "start" && state == "dormant") { Button[Rnd.Range(0, 4)].OnInteract(); }
		else if (state != "dormant")
		{
			string validCommands = "ulrd";
			if (!validCommands.Contains(command))
			{
				yield return "sendtochaterror Invalid command.";
				yield break;
			}
			yield return null;
			for (int j = 0; j < validCommands.Length; j++)
			{
				if (command[0] == validCommands[j]) { Button[j].OnInteract(); }
			}
		}
		else
			yield return "sendtochaterror The module is not active yet.";
		yield return "strike";
		yield return "solve";
		yield return null;
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		int[] dirs = { 8, 4, 2, 1 };
		int choose = 0;
		yield return null;
		if (state == "dormant")
		{
			Button[Rnd.Range(0, 4)].OnInteract();
			yield return new WaitForSeconds(0.5f);
		}
		while(state != "solved")
        {
			choose = Rnd.Range(0, 4);
			if (((target & dirs[choose]) != 0))
            {
				Button[choose].OnInteract();
				yield return new WaitForSeconds(0.5f);
			}
			yield return null;
		}
	}
}