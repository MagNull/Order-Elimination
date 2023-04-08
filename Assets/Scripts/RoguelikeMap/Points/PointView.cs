using RoguelikeMap.Panels;

namespace RoguelikeMap.Points
{
    public class PointView
    {
        private IPanel _panel;
        
        public PointView(IPanel panel)
        {
            _panel = panel;
            SetActivePanel(true);
        }

        public void SetActivePanel(bool isActive)
        {
            if(isActive)
                _panel.Open();
            else
                _panel.Close();
        }
    }
}