using UnityEngine;
namespace OrderElimination
{
    public class SquadPresenter
    {
        private readonly SquadModel _model;
        private readonly SquadView _view;
        private PlanetPoint _planetPoint;
        public SquadPresenter(SquadModel model, SquadView view, PlanetPoint planetPoint)
        {
            _model = model;
            _view = view;
            _planetPoint = planetPoint;
        }

        public void Subscribe()
        {
            _model.Moved += _view.OnMove;
            _model.Selected += _view.OnSelect;
            _model.Unselected += _view.OnUnselect;
            if(_planetPoint != null)
            {
                _model.Selected += _planetPoint.ShowPaths;
                _model.Unselected += _planetPoint.HidePaths;
            }
        }

        public void Unsubscribe()
        {
            _model.Moved -= _view.OnMove;
            _model.Selected -= _view.OnSelect;
            _model.Unselected -= _view.OnUnselect;
            if(_planetPoint != null)
            {
                _model.Selected -= _planetPoint.ShowPaths;
                _model.Unselected -= _planetPoint.HidePaths;
            }
        }

        public void UpdatePlanetPoint(PlanetPoint planetPoint)
        {
            Unsubscribe();
            _planetPoint = planetPoint;
            Subscribe();
        }
    }
}
