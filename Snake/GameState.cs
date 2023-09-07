using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        //Anzahl der Zeilen und Spalten
        public int Rows { get; }
        public int Cols { get; }
        // 2d Array für die Gesamtfläche des Boards
        public GridValue[,] Grid { get; }
        // Die Richtung in die die Schlange sich bewegen soll
        public Direction Dir { get; private set; }
        // Scorebestimmung
        public int Score { get; private set; }
        // Gameover boolean
        public bool GameOver { get; private set; }


        // Puffer für die Gedrückten Tasten, für einen smoothen Richtungswechsel
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();

        // Liste mit der aktuell bestzten Felder von der Schlange --- LinkedList erlaubt es von beiden Seiten der Liste Werte zu entfernen und Hinzuzufügen
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();

        // Random Objekt um das Spawnen der Nahrung zu bestimmen
        private readonly Random random = new Random();


        // Konstruktor
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols]; // 2D Array wird initialisiert
            Dir = Direction.Right;

            AddSnake();
            AddFood();

        }

        // Der Snake wird dem Brett hinzugefügt (Methode)
        private void AddSnake()
        {
            int r = Rows / 2; // Mittlere Zeile des Boards

            // Loop für die Spaltenbestimmung
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        // Methode um die Felder ohne Inhalt zu bestimmen, damit die Nahrung darauf gespawnt werden kann

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        // Methode um die Nahrung Spawnen zu lassen

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            // Damit das Spiel nicht abstürzt, wenn es keine leeren Felder mehr gibt, findet eine Fallüberprüfung statt.

            if (empty.Count == 0)
            {
                return;
            }

            // Nahrung wird in ein Zuffäliges Feld, welches leer sein muss, reingespawnt.
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        // Methode für die Positionsbestimmung des Heads des Snakes
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        // Methode für die Positionsbestimmung des Tails des Snakes
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        // Methode um alle SnakePositionen als IEnumerable zu returnen.
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        // 2 Methoden um den Snake zu modifizieren
        private void AddHead(Position pos) // Es tut die aktuelle Position vor den Snake stellen, um den neuen Kopf in dieses Feld reinzubringen.
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail() // Der Schwanz wird hunzugefügt
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();

        }

        //Methode: um die letzte Richtung des Snakes zu erfahren
        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        // Methode: Can Change Direction
        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }


        // Public Methoden um den Gamestate zu modifizieren /////

        // Methode: Die Richtung des Snakes wird geändert
        public void ChangeDirection(Direction dir)
        {
            // Wenn Richtung änderbar ist
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        // Methode für die Fortbewegung des Snakes //
        
        // Methode: Ist das nächste Feld vor den Snake Outside oder nicht ?
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        //Methode: Die Position wird als Parameter genommen und es wird das returned, was der Snake Hitten wird, wenn er sich dahin bewegt.
        private GridValue WillHit (Position newHeadPos)
        {
            if(OutsideGrid(newHeadPos)) 
            {
                return GridValue.Outside; // Outside wird getroffen
            }
            if(newHeadPos == TailPosition())  // wird überprüft, ob der Kopf der Schlange, den Schwanz trifft. Wenn beide Felder im nächsten Schritt gleich sind, wird der Schwanz nicht getroffen.
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        // Methode: Move.
        public void Move()
        {
            // Puffer
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake) // Snake stoßt gegen Outside oder sich selbst.
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)    // Wenn das nächste Feld leer ist, wird Head ins nächste Feld hinzugefügt und Schwanz/Tail entfernt
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food) // Wenn das nächste Feld, Nahrung enthält, wird Kopf ins nächste Feld eingefügt, Score wird um 1 erhöht und Die Nahrung wird an einer anderen Stelle gespawnt.
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
