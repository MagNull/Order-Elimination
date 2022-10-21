using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination
{
    public class SquadPresenter
    {
        private readonly SquadModel _model;
        private readonly SquadView _view;
        public SquadPresenter(SquadModel model, SquadView view)
        {
            _model = model;
            _view = view;
        }

        public void Subscribe()
        {
            _model.Moved += _view.OnMove;
            _model.Selected += _view.OnSelect;
            _model.Unselected += _view.OnUnselect;
        }

        public void Unsubscribe()
        {
            _model.Moved -= _view.OnMove;
            _model.Selected -= _view.OnSelect;
            _model.Unselected -= _view.OnUnselect;
        }
    }
}
