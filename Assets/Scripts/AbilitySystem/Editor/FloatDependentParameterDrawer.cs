//using OrderElimination.Infrastructure;
//using Sirenix.OdinInspector.Editor;
//using Sirenix.Serialization;
//using Sirenix.Utilities.Editor;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;

//namespace OrderElimination.AbilitySystem.Editor
//{
//    public class FloatDependentParameterDrawer : OdinValueDrawer<IContextValueGetter>
//    {
//        private readonly static Dictionary<Type, List<string>> _typeProperties;
//        private InspectorProperty expressionProperty;
//        private string _valueInput;

//        protected override void Initialize()
//        {
//            base.Initialize();
//            expressionProperty = this.Property.Children["_valueGetterExpression"];
//            //stringFunction = (string)functionProperty.ValueEntry.WeakSmartValue;
//        }

//        protected override void DrawPropertyLayout(GUIContent label)
//        {
//            base.DrawPropertyLayout(label);
//            //function = this.ValueEntry.SmartValue.ToString();
//            //SirenixEditorGUI.BeginShakeableGroup(identifier);
//            EditorGUILayout.BeginHorizontal();
//            //functionProperty.ValueEntry.WeakSmartValue = SirenixEditorFields.TextField(label, (string)functionProperty.ValueEntry.WeakSmartValue);
//            var compileSuccess = false;
//            var compiledValue = 0f;
//            _valueInput = SirenixEditorFields.TextField("Simple value", _valueInput);
//            if (GUILayout.Button("Compile"))
//            {
//                if (float.TryParse(_valueInput, out var result))
//                {
//                    compileSuccess = true;
//                    compiledValue = result;
//                }
//                if (compileSuccess)
//                {
//                    var dependentParameter = new FloatDependentParameter(context => compiledValue);
//                    this.ValueEntry.SmartValue = dependentParameter;
//                }
//                else
//                {
//                    var dependentParameter = new FloatDependentParameter(context => 0);
//                    this.ValueEntry.SmartValue = dependentParameter;
//                }
//            }
//            EditorGUILayout.EndHorizontal();
//            //SirenixEditorFields.TextField();
            
//            var expr = ValueEntry.SmartValue.ValueGetterExpression.ToSafeString();
//            SirenixEditorFields.TextField("Compiled lambda", expr);
//        }

//        static FloatDependentParameterDrawer()
//        {
//            _typeProperties = new();
//            var containsNumericValuesInChildren = new Dictionary<Type, bool>();
//            var rootType = typeof(ActionExecutionContext);
//            AddTypePropertiesRecursive(rootType);
//            var unusedTypes = _typeProperties.Keys.Where(t => !containsNumericValuesInChildren[t]).ToArray();
//            foreach (var type in unusedTypes)
//            {
//                _typeProperties.Remove(type);
//            }

//            void AddTypePropertiesRecursive(Type rootType)
//            {
//                if (_typeProperties.ContainsKey(rootType))
//                    return;

//                _typeProperties.Add(rootType, new List<string>());
//                containsNumericValuesInChildren.Add(rootType, false);

//                var nestedProperties = rootType
//                    .GetProperties(BindingFlags.Public)
//                    .Where(p => p.CanRead)
//                    .ToArray();
//                var nestedFields = rootType
//                    .GetFields(BindingFlags.Public)
//                    .ToArray();
//                foreach (var property in nestedProperties)
//                {
//                    var propertyType = property.PropertyType;
//                    AddTypePropertiesRecursive(propertyType);
//                    if (propertyType.IsNumeric() || containsNumericValuesInChildren[propertyType])
//                        containsNumericValuesInChildren[rootType] = true;
//                }
//                foreach (var field in nestedFields)
//                {
//                    var fieldType = field.FieldType;
//                    AddTypePropertiesRecursive(fieldType);
//                    if (fieldType.IsNumeric() || containsNumericValuesInChildren[fieldType])
//                        containsNumericValuesInChildren[rootType] = true;
//                }
//            }
//        }
//    }
//}
