using ComponentTests.Utitlities;
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
    public abstract class ObjectsToObjectsComponent<TSource, TDestination> : BaseObjectComponent
        where TSource : class, new()
        where TDestination : class, new()
    {
        public string InputObjectColumnName { get { return typeof(TSource).ToString(); } }
        public string OutputObjectColumnName { get { return typeof(TDestination).ToString(); } }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            ReinitializeMetaData();
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            if (!HasOutputColumn(OutputObjectColumnName))
                AddOutputColumn(OutputObjectColumnName, DataType.DT_NTEXT);

            IsAsynchronous = true;
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);
            while (buffer.NextRow())
            {
                var inputColumnId = GetInputColumnId(OutputObjectColumnName);
                var inputBytes = buffer.GetBlobData(inputColumnId, 0, (int)buffer.GetBlobLength(inputColumnId));
                var inputXml = Encoding.UTF8.GetString(inputBytes);
                var inputObject = XmlSerializer.XmlDeserialize<TSource>(inputXml);

                var outputObject = ProcessInput(inputObject);

                var outputXml = XmlSerializer.XmlSerialize(outputObject);
                var outputBytes = Encoding.UTF8.GetBytes(outputXml);
                var outputColumnId = GetOutputColumnId(OutputObjectColumnName);

                AsynchronousOutputBuffer.AddRow();
                AsynchronousOutputBuffer.AddBlobData(outputColumnId, outputBytes);
            }

            if (buffer.EndOfRowset)
                AsynchronousOutputBuffer.SetEndOfRowset();
        }

        public override DTSValidationStatus Validate()
        {
            DTSValidationStatus result;

            if (!HasInputColumns)
            {
                FireError(@"Component should have at least one input column.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectInputColumn = HasInputColumn(OutputObjectColumnName);
            if (!hasObjectInputColumn)
            {
                FireError(@"First input column is not valid.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectInputColumnValidType = IsInputColumnTypeOf(InputObjectColumnName, DataType.DT_NTEXT);
            if (!hasObjectInputColumnValidType)
            {
                FireError(@"First input column should be the type of ""Unicode test stream"" (DT_NTEXT).");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            if (!HasOutputColumns)
            {
                FireError(@"Component should have at least one output column.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectOutputColumn = HasOutputColumn(OutputObjectColumnName);
            if (!hasObjectOutputColumn)
            {
                FireError(@"First output column is not valid.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectOutputColumnValidType = IsOutputColumnTypeOf(OutputObjectColumnName, DataType.DT_NTEXT);
            if (!hasObjectOutputColumnValidType)
            {
                FireError(@"First output column should be the type of ""Unicode test stream"" (DT_NTEXT).");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            result = base.Validate();

            return result;
        }

        public abstract TDestination ProcessInput(TSource inputObject);
    }
}
