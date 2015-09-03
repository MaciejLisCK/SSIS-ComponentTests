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
    public abstract class RowsToObjectComponent<TDestination> : BaseObjectComponent
        where TDestination : class, new()
    {
        public string ObjectColumnName { get { return typeof(TDestination).ToString(); } }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            ReinitializeMetaData();
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            if (!HasOutputColumn(ObjectColumnName))
                AddOutputColumn(ObjectColumnName, DataType.DT_NTEXT);
        }

        public override DTSValidationStatus Validate()
        {
            DTSValidationStatus result;

            if (!HasOutputColumns)
            {
                FireError(@"Component should have at least one input column.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectOutputColumn = HasOutputColumn(ObjectColumnName);
            if (!hasObjectOutputColumn)
            {
                FireError(@"First output column is not valid.");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            bool hasObjectOutputColumnValidType = IsOutputColumnTypeOf(ObjectColumnName, DataType.DT_NTEXT);
            if (!hasObjectOutputColumnValidType)
            {
                FireError(@"First output column should be the type of ""Unicode test stream"" (DT_NTEXT).");
                return DTSValidationStatus.VS_ISBROKEN;
            }

            result = base.Validate();

            return result;
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);
            while (buffer.NextRow())
            {
                var outputColumnId = GetOutputColumnId(ObjectColumnName);

                var outputObject = ProcessInput(buffer);

                var outputXml = XmlSerializer.XmlSerialize(outputObject);
                var outputBytes = Encoding.UTF8.GetBytes(outputXml);
                buffer.AddBlobData(outputColumnId, outputBytes);
            }
        }

        public abstract TDestination ProcessInput(PipelineBuffer buffer);
    }
}