using System;

public interface IMovable
{
    public event Action<Cell, Cell> Moved; 
    void OnMoving(Cell from, Cell to);
}