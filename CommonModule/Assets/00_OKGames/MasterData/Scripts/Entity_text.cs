using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_text : ScriptableObject
{
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int ID;
		public string key;
		public string ja;
		public string en;
	}

    public Param GetById(int id, int sheetIndex = 0) {
        return sheets[sheetIndex].list.Find(data => data.ID == id);
    }
}

