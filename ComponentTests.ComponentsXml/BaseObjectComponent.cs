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
    public abstract class BaseObjectComponent : PipelineComponent
    {
        protected bool IsAsynchronous
        {
            get
            {
                var outputCollections = ComponentMetaData.OutputCollection;
                var firstOutputCollection = outputCollections[0];
                bool isAsynchronous = firstOutputCollection.SynchronousInputID == 0;

                return isAsynchronous;
            }
            set
            {
                var outputCollections = ComponentMetaData.OutputCollection;
                var firstOutputCollection = outputCollections[0];
                firstOutputCollection.SynchronousInputID = 0;
            }
        }
        protected PipelineBuffer AsynchronousOutputBuffer { get; set; }

        protected bool HasInputColumns
        {
            get
            {
                var inputCollections = ComponentMetaData.InputCollection;
                if (inputCollections.Count == 0)
                    return false;

                var firstInputCollection = ComponentMetaData.InputCollection[0];
                var firstInputColumnCollection = firstInputCollection.InputColumnCollection;
                var hasInputColumns = firstInputColumnCollection.Count > 0;

                return hasInputColumns;
            }
        }
        protected IEnumerable<IDTSInputColumn100> InputColumns
        {
            get
            {
                var inputCollections = ComponentMetaData.InputCollection;
                var firstInputCollection = inputCollections[0];
                var firstInputColumnCollection = firstInputCollection.InputColumnCollection;
                var firstInputColumns = firstInputColumnCollection.Cast<IDTSInputColumn100>();
                return firstInputColumns;
            }
        }
        protected bool HasInputColumn(string columnName)
        {
            if (!HasInputColumns)
                return false;

            var hasNameSpecifiedColumn = InputColumns.Any(oc => columnName == oc.Name);

            return hasNameSpecifiedColumn;
        }
        protected bool IsInputColumnTypeOf(string columnName, DataType type)
        {
            var isInputColumnTypeOf = InputColumns
                .Any(c => c.Name == columnName && c.DataType == type);

            return isInputColumnTypeOf;
        }
        protected int GetInputColumnId(string columnName)
        {
            var inputBuffer = ComponentMetaData.InputCollection[0].Buffer;
            var inputColumn = InputColumns.Single(oc => oc.Name == columnName);

            var columnId = BufferManager.FindColumnByLineageID(inputBuffer, inputColumn.LineageID);

            return columnId;
        }


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
                var firstOutputCollection = outputCollections[0];
                var firstOutputColumnCollection = firstOutputCollection.OutputColumnCollection;
                var firstOutputColumns = firstOutputColumnCollection.Cast<IDTSOutputColumn100>();
                return firstOutputColumns;
            }
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
        protected void AddOutputColumn(string columnName, DataType dataType)
        {
            var outputCollection = ComponentMetaData.OutputCollection[0];
            var outputColumns = outputCollection.OutputColumnCollection;
            var newColumn = outputColumns.New();

            newColumn.Name = columnName;
            newColumn.SetDataTypeProperties(DataType.DT_NTEXT, 0, 0, 0, 0);
        }
        protected int GetOutputColumnId(string columnName)
        {
            int bufferNumber;
            if (!IsAsynchronous)
                bufferNumber = ComponentMetaData.InputCollection[0].Buffer;
            else
                bufferNumber = ComponentMetaData.OutputCollection[0].Buffer;

            var outputColumn = OutputColumns.Single(oc => oc.Name == columnName);
            var columnId = BufferManager.FindColumnByLineageID(bufferNumber, outputColumn.LineageID);

            return columnId;
        }

        protected void FireError(string errorMessage)
        {
            bool cancel;
            ComponentMetaData.FireError(0, ComponentMetaData.Name, errorMessage, string.Empty, 0, out cancel);
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            if (buffers.Length != 0)
                AsynchronousOutputBuffer = buffers[0];
        }
    }
}
