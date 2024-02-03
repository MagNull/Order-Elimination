using OrderElimination;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class SectionalProgressBar : SerializedMonoBehaviour
    {
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _sectionsHolder;
        [SerializeField]
        private RectTransform _sectionPrefab;
        [HideInInspector]
        [SerializeField]
        private List<RectTransform> _sections = new();

        [ShowInInspector]
        [OdinSerialize]
        public int SectionsCount
        {
            get => _sections != null ? _sections.Count : 0;
            set
            {
                if (_sections == null || _sectionPrefab == null || _sectionsHolder == null)
                {
                    Logging.LogError("Object is not ready");
                    return;
                }
                if (value == _sections.Count) return;
                if (value < 0) value = 0;
                if (value > 100)
                {
                    value = 100;
                    Logging.LogError("Sections count is limited to 100 for safety measures");
                }
                if (value > _sections.Count)
                {
                    //expand
                    for (var i = _sections.Count; i < value; i++)
                    {
                        var section = Instantiate(_sectionPrefab, _sectionsHolder.transform);
                        _sections.Add(section);
                    }
                }
                else
                {
                    //decrease
                    for (var i = _sections.Count - 1; i >= value; i--)
                    {
                        var section = _sections[i];
                        _sections.RemoveAt(i);
                        #if UNITY_EDITOR
                        DestroyImmediate(section.gameObject);
                        #else
                        Destroy(section.gameObject);
                        #endif
                    }
                }
            }
        }

        public RectTransform GetSection(int id) => _sections[id];

        public IReadOnlyList<RectTransform> GetSections() => _sections;

        public void RecalculateLayout()
        {
            _sectionsHolder.CalculateLayoutInputHorizontal();
            _sectionsHolder.CalculateLayoutInputVertical();
            _sectionsHolder.SetLayoutHorizontal();
            _sectionsHolder.SetLayoutVertical();
        }
    }
}
