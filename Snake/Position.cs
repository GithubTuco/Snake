
using System;
using System.Collections.Generic;

namespace Snake
{
    public class Position
    {
        //Getter der Attribute von der Klasse Position
        public int Row { get; }
        public int Col { get; }

        //Konstruktor

        public Position(int row, int col) 
        {
            Row = row;
            Col = col;
        }

        // Methode für die Positionsbestimmung 

        public Position Translate(Direction dir)
        {
            return new Position(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        // Hashcode und Equals
        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
