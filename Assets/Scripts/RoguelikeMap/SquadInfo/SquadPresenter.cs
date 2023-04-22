using RoguelikeMap.Points;

namespace OrderElimination
{
    public class SquadPresenter
    {
        private readonly SquadModel _model;
        private readonly SquadView _view;
        private Point _point;
        public Point Point => _point;

        public SquadPresenter(SquadModel model, SquadView view, Point point)
        {
            _model = model;
            _view = view;
            _point = point;
        }

        public void Subscribe()
        {
            _model.Moved += _view.OnMove;
        }

        public void Unsubscribe()
        {
            _model.Moved -= _view.OnMove;
        }

        public void UpdatePlanetPoint(Point point)
        {
            Unsubscribe();
            _point = point;
            Subscribe();
        }
    }
}