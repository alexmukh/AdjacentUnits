using System;
using System.Collections;
using System.Collections.Generic;

class Program {

  public class Unit : IUnit {
    int name;
    int _hitPoints = 10;
    String _name;
    // to change to private
    
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth;

    public bool killed;

    public String Name { get => _name; set => _name = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public bool Alive { get => !killed; }
    public int HitPoints { get => _hitPoints; }
    
    public Unit() {
      currentHealth = maxHealth;
    }
  
    public Unit( int n ) : this() {
      name = n;
    }

    public Unit( String name ) : this() {
      Name = name;
    }
    
    public virtual void Cast(Army army, List <Unit> adjacentUnits) {}
    
    public virtual void Heal( int healAmount ) {
      if (killed) return;
      currentHealth += healAmount;
      if (currentHealth > maxHealth) currentHealth = maxHealth;
      Console.Write("Healed:");
      Print();
    }

    public virtual void Damage( int damageAmount ) {
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
      Console.WriteLine("{0}:{1}", name, Name);
    }

  }

  public interface IUnit {
    String Name { get; set; }  
    int HitPoints { get; }
  }

  
      
  class Healer : Unit {
    private int healAmount = 10;

    public Healer(String name) : base(name) {}
    
    public override void Cast(Army army, List <Unit> adjacentUnits) {
        foreach( var u in adjacentUnits ) u.Heal(healAmount);
    }

    public override void Heal(int healAmount) {
      Console.WriteLine("Method overriden");
      base.Heal(healAmount);
    }
    
  }

  class Magian : Unit {
    public override void Cast(Army army, List <Unit> adjacentUnits) {
      foreach( var u in adjacentUnits ) army.Add(u);
    }
    public override void Heal(int healAmount) {}
  }
  
  public abstract class Army {
    protected List <Unit> _units;
    public Unit Champ { get => _units[0]; }
    
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

    public Army ( List <String> listOfUnits ) {
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

    public void CastAll() {
      for(var i=1; i<_units.Count; i++) {
        var superUnit = _units[i];
        if(superUnit.Alive) {
          var neighborsList = AdjacentUnits(i);
          superUnit.Cast(this, neighborsList);
          }
        }
    }
  
    public int ChampHitPoints { get => _units[0].HitPoints; }
    
    public void HitChamp(int damageAmount) {
      if(Champ.killed) return;
      Champ.Damage(damageAmount);
    }
  
    public void ChampAttack(Army opponentArmy) {
      if (Champ.killed) return;
      opponentArmy.HitChamp(Champ.HitPoints);    
    }
    
  }

  class FormationColumnX3 : Army {

    public FormationColumnX3 ( List <int> listOfUnits ) : base(listOfUnits) {}
    public FormationColumnX3 ( List <String> listOfUnits ) : base(listOfUnits) {} 
    
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
      //foreach( var u in a ) u.Print();
      return a;
    }
  }


  public static void Main (string[] args) {
    Console.WriteLine ("Hello World");
    var listOfInt = new List <int> () {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23};
    FormationColumnX3 army = new FormationColumnX3(listOfInt) ;
    
    var healer = new Healer("Д-р Пилюлькин");

    army.Add(healer);
    army.Add(new Healer("Д-р Стекляшкин"));
    army.Print();
    
    army.CastAll();

    var listOfStr = new List <String> () {"Gendalf","Frodo"};

    FormationColumnX3 RedArmy = new FormationColumnX3(listOfStr);
    FormationColumnX3 WhiteArmy = new FormationColumnX3(listOfStr);

    Console.WriteLine();
    RedArmy.Champ.Print();

    RedArmy.ChampAttack(WhiteArmy);
    if(WhiteArmy.Champ.Alive) WhiteArmy.ChampAttack(RedArmy);
    RedArmy.CastAll();
  }
}

//YAGNI - реализуем только те функции и структуры данных, которые нужны для конкреного действия - определения соседних юнитов

//DRY - AdjacentUnits3 тут переписана по сравнению с предыдущим кодом

//KISS