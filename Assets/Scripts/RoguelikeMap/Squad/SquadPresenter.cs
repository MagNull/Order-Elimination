namespace OrderElimination
{
    public class SquadPresenter
    {
        private readonly SquadModel _model;
        private readonly SquadView _view;
        private IPoint _point;
        public IPoint Point => _point;

        public SquadPresenter(SquadModel model, SquadView view, IPoint point)
        {
            _model = model;
            _view = view;
            _point = point;
        }

        public void Subscribe()
        {
            _model.Moved += _view.OnMove;
            if (_point != null)
            {
                _model.Selected += _point.ShowPaths;
                _model.Unselected += _point.HidePaths;
            }
        }

        public void Unsubscribe()
        {
            _model.Moved -= _view.OnMove;
            if (_point != null)
            {
                _model.Selected -= _point.ShowPaths;
                _model.Unselected -= _point.HidePaths;
            }
        }

        public void UpdatePlanetPoint(IPoint point)
        {
            Unsubscribe();
            _point = point;
            Subscribe();
        }
    }
}