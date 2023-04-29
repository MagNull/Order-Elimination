using RoguelikeMap.Panels;

namespace RoguelikeMap.Points
{
    public class PointView
    {
        private readonly Panel _panel;
        
        public PointView(Panel panel)
        {
            _panel = panel;
        }

        public void SetActivePanel(bool isActive)
        {
            if(isActive)
                _panel.Open();
            else
                _panel.Close();
        }

        public void SetModel(PointModel model)
        {
            _panel.SetInfo(model);
        }
    }
}