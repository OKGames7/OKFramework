using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_item1 : ScriptableObject {
    public List<Sheet> sheets = new List<Sheet>();

    [System.SerializableAttribute]
    public class Sheet {
        public string name = string.Empty;
        public List<Param> list = new List<Param>();
    }

    [System.SerializableAttribute]
    public class Param {

        public int ID;
        public string string_data;
        public int int_data;
        public double double_data;
        public bool bool_data;
        public float math_1;
        public string math_2;
        public int[] array;
    }

    public Param GetById(int id, int sheetIndex = 0) {
        return sheets[sheetIndex].list.Find(data => data.ID == id);
    }
}

