using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    public class BaseObjectComponent : PipelineComponent
    {
        protected bool HasOutputColumns
        {
            get
            {
                var outputCollections = ComponentMetaData.OutputCollection;
                if (outputCollections.Count == 0)
                    return false;

                var firstOutputCollection = ComponentMetaData.OutputCollection[0];
                var firstOutputColumnCollection = firstOutputCollection.OutputColumnCollection;
                var hasOutputColumns = firstOutputColumnCollection.Count > 0;

                return hasOutputColumns;
            }
        }

        protected IEnumerable<IDTSOutputColumn100> OutputColumns
        {
            get
            {
                var outputCollections = ComponentMetaData.OutputCollection;
                var firstOutputCollection = ComponentMetaData.OutputCollection[0];
                var firstOutputColumnCollection = firstOutputCollection.OutputColumnCollection;
                var firstOutputColumns = firstOutputColumnCollection.Cast<IDTSOutputColumn100>();
                return firstOutputColumns;
            }
        }

        protected void AddOutputColumn(string columnName, DataType dataType)
        {
            var outputCollection = ComponentMetaData.OutputCollection[0];
            var outputColumns = outputCollection.OutputColumnCollection;
            var newColumn = outputColumns.New();

            newColumn.Name = columnName;
            newColumn.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
        }

        protected bool HasOutputColumn(string columnName)
        {
            if (!HasOutputColumns)
                return false;

            var hasNameSpecifiedColumn = OutputColumns.Any(oc => columnName == oc.Name);

            return hasNameSpecifiedColumn;
        }

        protected bool IsOutputColumnTypeOf(string columnName, DataType type)
        {
            var isOutputColumnTypeOf = OutputColumns
                .Any(c => c.Name == columnName && c.DataType == type);

            return isOutputColumnTypeOf;
        }

        protected int GetOutputColumnId(string columnName)
        {
            var inputBuffer = ComponentMetaData.InputCollection[0].Buffer;
            var outputColumn = OutputColumns.Single(oc => oc.Name == columnName);

            var columnId = BufferManager.FindColumnByLineageID(inputBuffer, outputColumn.LineageID);

            return columnId;
        }

        protected void FireError(string errorMessage)
        {
            bool cancel;
            ComponentMetaData.FireError(0, ComponentMetaData.Name, errorMessage, string.Empty, 0, out cancel);
        }
    }
}
