using System;
using System.Collections;
using System.Collections.Generic;

class Program {

  public class Unit : IUnit {
    
    int _id;
    int _hitPoints = 10;
    String _name;
    int _cost;
    // для использования методов класса String в дальнейшем

    Double _expectation = 0.5;
    
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth;

    public bool killed;

    public String Name { get => _name; set => _name = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public bool Alive { get => !killed; }
    public bool Killed { get => killed; }
    public int ID { get => _id; set => _id = value; }
    public int Cost { get => _cost; set => _cost = value; }
    public virtual int HitPoints { get => _hitPoints; }
    
    public Unit() {
      currentHealth = maxHealth;
    }
  
    public Unit( int n ) : this() {
      _id = n;
    } // конструктор по _id

    public Unit( String name ) : this() {
      Name = name;
    } // конструктор по name
    
    public virtual void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy) {}
    // задаем формат метода Cast для каждого юнита в дальнейшем
    
    public virtual void Heal( int healAmount ) {
      if (killed) return;

      currentHealth += healAmount;
      if (currentHealth > maxHealth) currentHealth = maxHealth;
      Console.Write("Healed:");
      Print(); // для анимации :)
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
    
    public void Print() {
      Console.WriteLine("{0}:{1}", _id, Name);
    }

    public bool CastChance {
      get 
      {
        Random r = new Random();
      
        Double chance = (Double)r.Next()/Int32.MaxValue;
        Console.WriteLine($"The cast chance is {chance}");
        if ( chance > _expectation ) {
          Console.WriteLine("KAKAYA JALOSTЬ");
          return false;
        }
        return true;
      }
    }
    
  }

  public interface IUnit {
    String Name { get; set; }  
    int HitPoints { get; }
    int ID { get; set; }
    int Cost { get; set; }
  }
      
  class Healer : Unit {

    private int healAmount = 10;
    public Healer(String name) : base(name) {} // запускает конструктор базового класса
    
    public override void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy=null) {
      if (!CastChance) return;
      foreach( var u in adjacentUnits ) u.Heal(healAmount);
    } // применяем heal на соседних юнитах

    public override void Heal(int healAmount) {
      Console.WriteLine("Method overriden");
      base.Heal(healAmount);
    } // тестирование возможности кастомного лечения
    
  }

  class Magician : Unit {

    public override void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy=null) {
      if (!CastChance) return;
      foreach (var u in adjacentUnits)
      {
        if (!(u is Healer || u is Knight)) ourArmy.Add(u); // запрещено клонирование хилера и рыцаря
      }
    }
    
    public override void Heal(int healAmount) {} // маг не вылечивается
  }

  class Archer : Unit {
    int rangedDamage = 10;

    public override void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy) {
      if (!CastChance) return;
      opponentArmy.RandomUnit.Damage(rangedDamage);
    }
  }


    public interface IArmor
    {
        public bool Horse { get; set; }
        public bool Lance { get; set; }
        public bool Shield { get; set; }
        public bool Helm { get; set; }
    }
  class Knight : Unit, IArmor {

    bool _helm, 
        _shield, 
        _lance, 
        _horse;
    int _helmPoints = 10, 
        _shieldPoints = 10, 
        _lancePoints = 10, 
        _horsePoints = 10;

    public override void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy) {} // нет спецдействия

    public override int HitPoints { get => base.HitPoints + LanceHitPoints + HorseHitPoints; }

    public override void Damage( int damageAmount ) {
      if (killed) return;

      int finDamageAmount = damageAmount;

      if (finDamageAmount < ShieldProtectionPoints) return; // щит сдержал удар
      finDamageAmount -= ShieldProtectionPoints;  // урон ослаблен
      Shield = false;  // но щита больше нет
      
      if (finDamageAmount < HelmProtectionPoints) return; // шлем сдержал удар
      finDamageAmount -= HelmProtectionPoints;  // урон ослаблен
      Helm = false;  // но шлема больше нет
      
      base.Damage(finDamageAmount);  // оставшийся урон рыцарь терпит как все
    }

    public bool Helm { get => _helm; set => _helm = value; }
    public bool Horse { get => _horse; set => _horse = value; }
    public bool Lance { get => _lance; set => _lance = value; }
    public bool Shield { get => _shield; set => _shield = value; }
    public int LanceHitPoints { get { if(Lance) return _lancePoints; return 0; } }
    public int HorseHitPoints { get { if(Horse) return _horsePoints; return 0; } }
    public int HelmProtectionPoints { get { if(Helm) return _helmPoints; return 0; } }
    public int ShieldProtectionPoints { get {if(Shield) return _shieldPoints; return 0; } }

  }

  class Infantry : Unit, IArmor {
    
    bool _helm, 
        _shield, 
        _lance, 
        _horse;

    Infantry() {
      Helm = true;
      Horse = true;
      Lance = true;
      Shield = true;
    }

    public override void Cast(List <Unit> adjacentUnits, Army ourArmy, Army opponentArmy=null) {
      if (!CastChance) return;
      foreach( var u in adjacentUnits ) {
        if (!(u is Knight)) continue; // проверяем юнит на рыцарство  
        if (Horse) { 
          ((Knight)u).Horse = true;
          Horse = false;
          return;
          }
        if (Lance) { 
          ((Knight)u).Lance = true;
          Lance = false;
          return;
          }
        if (Shield) { 
          ((Knight)u).Shield = true;
          Shield = false;
          return;
          }
        if (Helm) { 
          ((Knight)u).Helm = true;
          Helm = false;
          return;
          }
      }
    }
        public bool Helm { get => _helm; set => _helm = value; }
        public bool Horse { get => _horse; set => _horse = value; }
        public bool Lance { get => _lance; set => _lance = value; }
        public bool Shield { get => _shield; set => _shield = value; }

  }
  
  public abstract class Army {
    protected List <Unit> _units;
    public Unit Champ { get => _units[0]; }
    public int ChampHitPoints { get => _units[0].HitPoints; } 
    
    public void Add ( Unit u) { 
      _units.Add(u); 
    }

    public Army ( List <int> listOfUnits ) { // по ID
      _units = new List <Unit> ();
      
      foreach(var n in listOfUnits) {
        var u = new Unit(n);
        _units.Add( u );
      }
    }

    public Army ( List <String> listOfUnits ) { // по именам
      _units = new List <Unit> ();

      foreach(var n in listOfUnits) {
        var u = new Unit(n);
        _units.Add( u );
      }
    }

    public Unit RandomUnit { // случайный юнит из армии для каста стрелка
      get {
        Random r = new Random();
        int index = r.Next(_units.Count);
        return _units[index];
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

    public void CastAll(Army opponentArmy) {
      for(var i=1; i<_units.Count; i++) {
        var superUnit = _units[i];
        if(superUnit.Alive) {
          var neighborsList = AdjacentUnits(i);
          superUnit.Cast(neighborsList, this, opponentArmy);
          }
        }
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
    
    var healer = new Healer("Doctor Healer");

    army.Add(healer);
    army.Add(new Healer("Doctor House"));
    army.Print();
    
    army.CastAll(null);

    var listOfStr = new List <String> () {"Gendalf","Frodo"};

    FormationColumnX3 RedArmy = new FormationColumnX3(listOfStr);
    FormationColumnX3 WhiteArmy = new FormationColumnX3(listOfStr);

    Console.WriteLine();
    RedArmy.Champ.Print();

    
    WhiteArmy.Champ.Damage(RedArmy.Champ.HitPoints);
    if(WhiteArmy.Champ.Alive) RedArmy.Champ.Damage(WhiteArmy.Champ.HitPoints);
    RedArmy.CastAll(WhiteArmy); // ход 1 штука
  }
}