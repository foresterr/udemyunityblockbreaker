using UnityEngine;
using Random = UnityEngine.Random;

public class Capsule : MonoBehaviour {
    public BonusType CapsuleBonus;

    private BonusType pickType(bool rare) {
        BonusType[] bt = rare ? BonusType.Rare : BonusType.Common;
        return bt[Random.Range(0, bt.Length)];
    }

    public void Initialize(bool rare) {
        CapsuleBonus = Random.Range(0, 10) == 0 ? pickType(!rare) : pickType(rare);
        GetComponent<SpriteRenderer>().color = CapsuleBonus.Color;
        GetComponentInChildren<Shine>().Play();
        GetComponent<Rigidbody2D>().velocity = Vector2.down * 50;
    }
}

public class BonusType {
    public string Name;
    public Color Color;
    public bool IsRare;

    public static BonusType[] Rare = {
        new BonusType() {Name = "eraser", Color = Color.red, IsRare = true},
        new BonusType() {Name = "laser", Color = Color.magenta, IsRare = true},
        new BonusType() {Name = "launcher", Color = Color.yellow, IsRare = true},
        new BonusType() {Name = "shield", Color = Color.gray, IsRare = true},
        new BonusType() {Name = "life", Color = Color.black, IsRare = true}
        //testing only from here
        //new BonusType() { Name = "laser", Color = Color.magenta, IsRare = true},
        //new BonusType() { Name = "launcher", Color = Color.yellow, IsRare = true},
        //new BonusType() { Name = "sticky", Color = Color.green, IsRare = false},
        //new BonusType() { Name = "extend", Color = Color.blue, IsRare = false}
    };

    public static BonusType[] Common = {
        new BonusType() {Name = "sticky", Color = Color.green, IsRare = false},
        new BonusType() {Name = "split", Color = Color.cyan, IsRare = false},
        new BonusType() {Name = "extend", Color = Color.blue, IsRare = false}
        //testing only from here
        //new BonusType() { Name = "laser", Color = Color.magenta, IsRare = true},
        //new BonusType() { Name = "launcher", Color = Color.yellow, IsRare = true},
        //new BonusType() { Name = "sticky", Color = Color.green, IsRare = false},
        //new BonusType() { Name = "extend", Color = Color.blue, IsRare = false}
    };

    //just paddlemods for testing
    /*
    new BonusType() { Name = "laser", Color = Color.magenta, IsRare = true},
    new BonusType() { Name = "launcher", Color = Color.yellow, IsRare = true},
    new BonusType() { Name = "sticky", Color = Color.green, IsRare = false},
    new BonusType() { Name = "extend", Color = Color.blue, IsRare = false}
    */
}