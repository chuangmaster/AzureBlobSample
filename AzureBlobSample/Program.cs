using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //PrintContainer
            var sourceConnectString = "Azure Blob Connection String";
            var service = new AzureBlobService(sourceConnectString);
            service.PrintContainer("MyContainer", "Directory1");

            //MoveBlobFile
            service.MoveBlobFile("Directory1/2018122001/03/1-1.jpg", "Directory2/1-1.jpg");
            Console.Read();
        }
    }
}
