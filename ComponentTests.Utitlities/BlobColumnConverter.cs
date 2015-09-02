using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.Utitlities
{
    public static class BlobConverter
    {
        public static string ConvertToString(this BlobColumn blobColumn)
        {
            string result = String.Empty;

            bool hasBlobColumnData = blobColumn != null && !blobColumn.IsNull;
            if (hasBlobColumnData)
            {
                var blobColumnLength = System.Convert.ToInt32(blobColumn.Length);
                var blobColumnData = blobColumn.GetBlobData(0, blobColumnLength);
                result = System.Text.Encoding.Unicode.GetString(blobColumnData);
            }

            return result;
        }

        public static void AddStringData(this BlobColumn blobColumn, string @string)
        {
            var unicodeBytes = Encoding.Unicode.GetBytes(@string);

            blobColumn.AddBlobData(unicodeBytes);
        }
    }
}
