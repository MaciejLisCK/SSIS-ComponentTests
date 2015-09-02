using ComponentTests.Utitlities;
using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.ComponentsXml
{
    public class XmlPipelineComponent<TSource, TDestination> : PipelineComponent
        where TSource : class, new()
        where TDestination : class, new()
    {
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            base.ProcessInput(inputID, buffer);
            while(buffer.NextRow())
            {
                var inputBytes = buffer.GetBytes(0);
                var inputXml = Encoding.UTF8.GetString(inputBytes);
                var inputObject = XmlSerializer.XmlDeserialize<TSource>(inputXml);

                var outputObject = ProcessInput(inputObject);

                var outputXml = XmlSerializer.XmlSerialize(outputObject);
                var outputBytes = Encoding.UTF8.GetBytes(outputXml);
                buffer.SetBytes(0, outputBytes);
            }
        }

        public virtual TDestination ProcessInput(TSource sourceObject)
        {
            return new TDestination();
        }
    }
}
/*
        var inputOrderXml = Row.OrderXml.ConvertToString();
        var order = XmlSerializer.XmlDeserialize<Order>(inputOrderXml);
        var mocaProxy = new MocaProxy();

        var xmlOffer = mocaProxy.GetOffer();

        var orderSerialized = XmlSerializer.XmlSerialize<Order>(order);
        Output0Buffer.OrderXml.AddStringData(orderSerialized); 
*/