using System;
using System.Collections.Generic;

namespace Snake
{
    public class Direction
    {
        // Richtungen links,rechts,hoch,runter -> Spalten und Zeilen werden geändert
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);

        // Eigenschaften für Zeile und SPalte mit getter Methode
        public int RowOffset { get; }
        public int ColOffset { get; }

        //KOnstruktor

        private Direction(int rowOffset, int colOffset) 
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        //Eine Methode 
        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColOffset);
        }

        // Equals und Hashcode wird generiert
        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}
