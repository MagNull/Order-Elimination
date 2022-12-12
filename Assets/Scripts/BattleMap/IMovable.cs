using System;

public interface IMovable
{
    public event Action<Cell, Cell> Moved; 
    void OnMoved(Cell from, Cell to);
}