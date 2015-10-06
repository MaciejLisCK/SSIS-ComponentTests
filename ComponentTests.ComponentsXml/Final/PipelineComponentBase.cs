using AutoMapper;
using ComponentTests.ComponentsXml.X3WebService;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ComponentTests.ComponentsXml.Final
{
    public abstract class PipelineComponentBase : PipelineComponent
    {
        public List<IDTSCustomProperty100> CustomProperties 
        { 
            get
            {
                var customPropertiesCollection = ComponentMetaData.CustomPropertyCollection;
                var customProperties = customPropertiesCollection.Cast<IDTSCustomProperty100>();

                return customProperties.ToList();
            }
        }

        public List<IDTSOutputColumn100> OutputColumns
        {
            get
            {
                var outputCollection = ComponentMetaData.OutputCollection[0];
                var outputColumnsCollection = outputCollection.OutputColumnCollection;

                var outputColumns = outputColumnsCollection.Cast<IDTSOutputColumn100>();

                return outputColumns.ToList();
            }
        }

        public List<IDTSInputColumn100> InputColumns
        {
            get
            {
                var inputCollection = ComponentMetaData.InputCollection[0];
                var inputColumnsCollection = inputCollection.InputColumnCollection;

                var inputColumns = inputColumnsCollection.Cast<IDTSInputColumn100>();

                return inputColumns.ToList();
            }

        }

        protected string GetCustomPropertyValue(string customPropertyName)
        {
            var customProperty = CustomProperties.Single(cp => cp.Name == customPropertyName);

            return customProperty.Value;
        }

        protected void CreateCustomProperty(string name, string description)
        {
            var customProperty = ComponentMetaData.CustomPropertyCollection.New();
            customProperty.Name = name;
            customProperty.Description = description;
            customProperty.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
        }

        protected void CreateCustomProperty(string name)
        {
            CreateCustomProperty(name, String.Empty);
        }

        protected IDTSOutputColumn100 GetOutputColumn(string columnName)
        {
            var column = OutputColumns.Single(oc => oc.Name == columnName);

            return column;
        }

        protected int GetOutputColumnIndex(string columnName)
        {
            var outputCollection = ComponentMetaData.OutputCollection[0];
            var outputColumns = outputCollection.OutputColumnCollection.Cast<IDTSOutputColumn100>();
            var outputColumn = outputColumns.Single(c => c.Name == columnName);

            var index = (int)BufferManager.FindColumnByLineageID(outputCollection.Buffer, outputColumn.LineageID);

            return index;
        }

        protected int GetInputCollumnIndex(string name)
        {
            var inputCollection = ComponentMetaData.InputCollection[0];
            var inputColumns = inputCollection.InputColumnCollection.Cast<IDTSInputColumn100>();
            var inputColumn = inputColumns.Single(c => c.Name == name);

            var index = (int)BufferManager.FindColumnByLineageID(inputCollection.Buffer, inputColumn.LineageID);

            return index;
        }
    }
}
