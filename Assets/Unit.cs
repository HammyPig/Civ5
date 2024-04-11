using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    private string name;

    public override string ToString() {
        return name;
    }

    public string GetName() {
        return name;
    }

    public void SetName(string name) {
        this.name = name;
    }
}
