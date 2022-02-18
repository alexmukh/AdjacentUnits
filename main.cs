using System;
using System.Collections;
using System.Collections.Generic;

class Program {

  public class Unit : IUnit {
    int name;

    // to change to private
    
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth;

    public bool killed;

    public Unit() {
      currentHealth = maxHealth;
    }
  
    public Unit( int n ) : this() {
      name = n;
    }

    public virtual void Heal( int healAmount ) {
      if (killed) return;
      currentHealth += healAmount;
      if (currentHealth > maxHealth) currentHealth = maxHealth;
      Console.WriteLine("Default method applied");
    }

    public void Damage( int damageAmount ) {
      if (killed) return;
      currentHealth -= damageAmount;
      if (currentHealth < minHealth) {
        Destroy();
      }
    }

    public void Destroy() {
      killed = true;
    }
    
    //public abstract void Cast();
    
    public void Print() {
      Console.WriteLine(name);
    }

  }

  public interface IUnit {
    
  }

  
      
  class Healer : Unit {
    private int healAmount = 10;
    
    public void Cast(List <Unit> adjacentUnits) {
        //foreach( var u in adjacentUnits ) u?.Heal(u, healAmount);
    }

    public override void Heal(int healAmount) {
      Console.WriteLine("Method overriden");
      base.Heal(healAmount);
    }
    
  }
  
  abstract class Army {
    protected List <Unit> _units;

    public void Add ( Unit u) { 
      _units.Add(u); 
    }

    public Army ( List <int> listOfUnits ) {
      _units = new List <Unit> ();

      foreach(var n in listOfUnits) {
        var u = new Unit(n);
        _units.Add( u );
      }
    }

    public bool indexIsValid(int index) {
      if ( 0 <= index && index < _units.Count ) return true;
      return false;
    } 

    public void Print() {
      foreach(var u in _units) u.Print();
    }

    public abstract List <Unit> AdjacentUnits( int index );
  }

  class Formation : Army {

    public Formation ( List <int> listOfUnits ) : base(listOfUnits) {}
      
    public override List <Unit> AdjacentUnits( int index ) {

      var formationLeftFlank = new List <int> () {-3, -1, 2, 3};
      var formationCenter = new List <int> () {-3, -2, -1, 1, 2, 3};
      var formationRightFlank = new List <int> () {-3, -2, 1, 3};

      // для колонны по 3
      List <Unit> a = new List <Unit> ();

      int j = index % 3;
      switch ( j ) {

        case 0:
          foreach( int i in formationCenter ) 
            if (indexIsValid(index+i)) a.Add(_units[index+i]);
          break;

        case 1:
          foreach( int i in formationLeftFlank ) 
            if (indexIsValid(index+i)) a.Add(_units[index+i]);
          break;
          
        case 2:
          foreach( int i in formationRightFlank ) 
            if (indexIsValid(index+i)) a.Add(_units[index+i]);
          break;
      }
      
      foreach( var u in a ) u.Print();
      
      return a;
    }
  }


  public static void Main (string[] args) {
    Console.WriteLine ("Hello World");
    var listOfInt = new List <int> () {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23};
    Formation army = new Formation(listOfInt) ;
    
    army.Print();
    Console.WriteLine();

    army.AdjacentUnits(8);
    Console.WriteLine();
    
    army.AdjacentUnits(7);
    Console.WriteLine();
    
    army.AdjacentUnits(21);

    var healer = new Healer();

    healer.Heal(10);
  }
}

//YAGNI - реализуем только те функции и структуры данных, которые нужны для конкреного действия - определения соседних юнитов

//DRY - AdjacentUnits3 тут переписана по сравнению с предыдущим кодом

//KISS