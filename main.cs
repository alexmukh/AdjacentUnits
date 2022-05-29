using System;
using System.Collections;
using System.Collections.Generic;

class Program {

  public class Unit : IUnit {
    
    int _id;
    int _hitPoints = 10;
    String _name;
    int _cost = 10;

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
    public int Cost { get => _cost; }
    public int Attack { get; set; }
    public int Defence { get; set; }
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
    int Attack { get; set; }
    int Defence { get; set; }
    int Cost { get; }
  }
      
  class Healer : Unit {
    
    private int healAmount = 10;

    public new int Cost { get => base.Cost*4; }
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
    public Magician(String name) : base(name) { }
       public new int Cost { get => base.Cost * 4; }

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
        public Archer(String name) : base(name) { }
        public new int Cost { get => base.Cost * 2; }

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

    public Knight () : base()
    {
            Helm = true;
            Horse = true;
            Lance = true;
            Shield = true;
    }
    public Knight(String name) : base(name) 
    {
            Helm = true;
            Horse = true;
            Lance = true;
            Shield = true;
    }
        public new int Cost { get => base.Cost * 4; }

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

    public Infantry() : base() {
      Helm = true;
      Horse = true;
      Lance = true;
      Shield = true;
    }

    public Infantry(String name) : base(name) 
    {
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
  
  public class Army {
    protected List <Unit> _units;
    public Unit Champ { get => _units[0]; }
    public int ChampHitPoints { get => _units[0].HitPoints; } 
    
    public void Add ( Unit u) { 
      _units.Add(u); 
    }

    public Army()
    {
      _units = new List<Unit>();
    }
    public Army ( List <int> listOfUnits ) : this() { // по ID
      foreach(var n in listOfUnits) {
        var u = new Unit(n);
        _units.Add( u );
      }
    }

    public Army ( List <String> listOfUnits ) : this() { // по именам
      foreach(var n in listOfUnits) {
        var u = new Unit(n);
        _units.Add( u );
      }
    }

    public Army(Army other) : this()
    {
            _units = other._units;
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
  }
    public abstract class Formation : Army 
    {
        public Formation() { }
        public Formation(Army army) : base(army) { }
        public Formation(List<int> listOfUnits) : base(listOfUnits) { }
        public Formation(List<String> listOfUnits) : base(listOfUnits) { }
        public abstract List<Unit> AdjacentUnits(int index);
        public void CastAll(Army opponentArmy)
        {
            for (var i = 1; i < _units.Count; i++)
            {
                var superUnit = _units[i];
                if (superUnit.Alive)
                {
                    var neighborsList = AdjacentUnits(i);
                    superUnit.Cast(neighborsList, this, opponentArmy);
                }
            }
        }
    }
    class FormationColumnX3 : Formation
    {
        public FormationColumnX3(Army army) : base(army) { }
        public FormationColumnX3(List<int> listOfUnits) : base(listOfUnits) { }
        public FormationColumnX3(List<String> listOfUnits) : base(listOfUnits) { }
        public override List<Unit> AdjacentUnits(int index)
        {
            var formationLeftFlank = new List<int>() { -3, -1, 2, 3 };
            var formationCenter = new List<int>() { -3, -2, -1, 1, 2, 3 };
            var formationRightFlank = new List<int>() { -3, -2, 1, 3 };

            // для колонны по 3
            List<Unit> a = new List<Unit>();

            int j = index % 3;
            switch (j)
            {
                case 0:
                    foreach (int i in formationCenter)
                        if (indexIsValid(index + i)) a.Add(_units[index + i]);
                    break;

                case 1:
                    foreach (int i in formationLeftFlank)
                        if (indexIsValid(index + i)) a.Add(_units[index + i]);
                    break;

                case 2:
                    foreach (int i in formationRightFlank)
                        if (indexIsValid(index + i)) a.Add(_units[index + i]);
                    break;
            }
            //foreach( var u in a ) u.Print();
            return a;
        }
    }
    class FormationColumnX1 : Formation
    {
        public FormationColumnX1(Army army) : base(army) { }
        public FormationColumnX1(List<int> listOfUnits) : base(listOfUnits) { }
        public FormationColumnX1(List<String> listOfUnits) : base(listOfUnits) { }
        public override List<Unit> AdjacentUnits(int index)
        {
            List<Unit> a = new List<Unit>();

            if (indexIsValid(index - 1)) a.Add(_units[index - 1]);
            if (indexIsValid(index + 1)) a.Add(_units[index + 1]);
            return a;
        }
    }
    class FormationLine : Formation
    {
        public FormationLine(Army army) : base(army) { }
        public FormationLine(List<int> listOfUnits) : base(listOfUnits) { }
        public FormationLine(List<String> listOfUnits) : base(listOfUnits) { }
        public override List<Unit> AdjacentUnits(int index)
        {
            List<Unit> a = new List<Unit>();

            if (indexIsValid(index - 1)) a.Add(_units[index - 1]);
            if (indexIsValid(index + 1)) a.Add(_units[index + 1]);
            return a;
        }
    }
    public class Opponents
    {
        Tuple<Formation, Formation> Opp;
        public Opponents(Formation Red, Formation White)
        {
            Opp = new Tuple<Formation, Formation> (Red, White);
        }
        public Opponents (Opponents other) : this(other.Opp.Item1, other.Opp.Item2)
        {  
        }
    }
    class History 
    {
        Stack <Opponents> Undo;
        Stack <Opponents> Redo;

        public void SaveToHistory (Opponents S)
        {
            Undo.Push (S);
        }
        public bool GetFromHistory ( ref Opponents S )
        {
            var Current = new Opponents(S);
            if ( Undo.TryPop(out S) )
            {
                Redo.Push(Current);
                return true;
            }
            return false;
        }
        public bool RedoHistory ( ref Opponents S )
        {
            var Current = new Opponents(S);
            if ( Redo.TryPop(out S) )
            {
                Undo.Push(Current);
                return true;
            }
            return false;
        }
    }
    static void ShowMenu ( String menu = "(Esc)Exit (U)ndo (R)edo (Enter)Move" )
    {
        Console.WriteLine(menu);
    }
    static void GameControl ()
    {
        ConsoleKey key;
        //Console.Clear();
        ShowMenu();
        while ((key = Console.ReadKey().Key) != ConsoleKey.Escape)
        {
            switch (key)
            {
                case ConsoleKey.U:
                    Console.WriteLine("Undo");
                    break;
                case ConsoleKey.R:
                    Console.WriteLine("Redo");
                    break;
                case ConsoleKey.Enter:
                    Console.WriteLine("Move");
                    break;
                default:
                    Console.WriteLine("В смысле?");
                    break;
            }
            ShowMenu();
        }
    }
    static void SelectArmyMenu( Army army, int budget ) 
    {
        ConsoleKey key;
        Console.WriteLine("Милорд, в Вашей казне {0} голды.", budget);
        ShowMenu("Выберите юнит: (Esc)Выход (K)Рыцарь (I)Пехотинец (M)Маг (H)Целитель (A)Лучник (T)Перекати-поле");
        while ((key = Console.ReadKey().Key) != ConsoleKey.Escape)
        {
            switch (key)
            {
                case ConsoleKey.K:
                    Knight K = new("Д.Кихот");
                    if (K.Cost <= budget)
                    {
                        Console.WriteLine("Рыцарь {0} пополнил наши ряды за {1} голды", K.Name, K.Cost);
                        army.Add(K);
                        budget -= K.Cost;
                    }
                    else
                    {
                        Console.WriteLine("Казна пуста, не велите казнить...");
                    }
                    break;
                case ConsoleKey.I:
                    Infantry I = new("С.Пансо");
                    if (I.Cost <= budget)
                    {
                        Console.WriteLine("Пехотинец {0} пополнил наши ряды за {1} голды", I.Name, I.Cost);
                        army.Add(I);
                        budget -= I.Cost;
                    }
                    else
                    {
                        Console.WriteLine("Казна пуста, не велите казнить...");
                    }
                    break;
                case ConsoleKey.M:
                    Magician M = new("Мэрлин");
                    if (M.Cost <= budget)
                    {
                        Console.WriteLine("Маг {0} пополнил наши ряды за {1} голды", M.Name, M.Cost);
                        army.Add(M);
                        budget -= M.Cost;
                    }
                    else
                    {
                        Console.WriteLine("Казна пуста, не велите казнить...");
                    }
                    break;
                case ConsoleKey.H:
                    Healer H = new("Стекляшкин");
                    if (H.Cost <= budget)
                    {
                        Console.WriteLine("Целитель {0} пополнил наши ряды за {1} голды", H.Name, H.Cost);
                        army.Add(H);
                        budget -= H.Cost;
                    }
                    else
                    {
                        Console.WriteLine("Казна пуста, не велите казнить...");
                    }
                    break;
                case ConsoleKey.A:
                    Archer A = new("Леголас");
                    if (A.Cost <= budget)
                    {
                        Console.WriteLine("Лучник {0} пополнил наши ряды за {1} голды", A.Name, A.Cost);
                        army.Add(A);
                        budget -= A.Cost;
                    }
                    else
                    {
                        Console.WriteLine("Казна пуста, не велите казнить...");
                    }
                    break;
                case ConsoleKey.T:
                    Console.WriteLine("Adding thumbleweed");
                    //Thumbleweed T = new("Ком с горы");
                    //armyRed.Add(T);
                    break;
                case ConsoleKey.P:
                    Console.WriteLine("Состав армии");
                    army.Print();
                    break;
                default:
                    Console.WriteLine("В смысле?");
                    break;
            }
            Console.WriteLine("На счету {0}", budget);
            ShowMenu("Выберите юнит: (Esc)Выход (K)Рыцарь (I)Пехотинец (M)Маг (H)Целитель (A)Лучник (T)Перекати-поле");
        }
    }

    static void SelectFormationMenu(Army armyRed, Army armyWhite, Formation red, Formation white)
    {
        ConsoleKey key;
        ShowMenu("Выберите тип построения - (1)Шеренга (2)Колонна по одному (3)Колонна по три (Enter)Подтвердить и продолжить");
        while ((key = Console.ReadKey().Key) != ConsoleKey.Enter)
        {
            switch (key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Шеренга");
                    red = new FormationLine(armyRed);
                    white = new FormationLine(armyWhite);
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Колонна по одному");
                    red = new FormationColumnX1(armyRed);
                    white = new FormationColumnX1(armyWhite);
                    break;
                case ConsoleKey.D3:
                    Console.WriteLine("Колонна по три");
                    red = new FormationColumnX3(armyRed);
                    white = new FormationColumnX3(armyWhite);
                    break;
                default:
                    Console.WriteLine("В смысле?");
                    break;
            }
            ShowMenu("Выберите тип построения - (1)Шеренга (2)Колонна по одному (3)Колонна по три (Enter)Подтвердить и продолжить");
        }
    }
    static Opponents SelectArmy( )
    {
        Army armyRed = new();
        Army armyWhite = new();
        int budgetRed = 1000;
        int budgetWhite = 1000;

        SelectArmyMenu(armyRed, budgetRed);
        SelectArmyMenu(armyWhite, budgetWhite);

        Formation formationRed = null; 
        Formation formationWhite = null;

        SelectFormationMenu(armyRed,armyWhite,formationRed,formationWhite);

        Opponents opponents = new Opponents(formationRed, formationWhite);

        return opponents;
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

        SelectArmy();
        GameControl();
  }
}