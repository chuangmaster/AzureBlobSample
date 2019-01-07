using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobSample
{
    public class AzueBlobService
    {
        string _SourceConectString { get; set; }
        public AzueBlobService(string sourceConectString)
        {
            _SourceConectString = sourceConectString;
        }

        /// <summary>
        /// 印出Container
        /// </summary>
        /// <param name="containerName">container name</param>
        public void PrintContainer(string containerName, string blobDirectoryUrn = "")
        {
            CloudStorageAccount sourceAccount = CloudStorageAccount.Parse(_SourceConectString);
            CloudBlobClient client = sourceAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            if (string.IsNullOrEmpty(blobDirectoryUrn) == false)
            {
                GotoInnerBlobs(container.GetDirectoryReference(blobDirectoryUrn).ListBlobs());
            }
            else
            {
                GotoInnerBlobs(container.ListBlobs());
            }
        }

        /// <summary>
        /// 取出Blob資料夾結構內容
        /// </summary>
        /// <param name="blobItems"></param>
        private void GotoInnerBlobs(IEnumerable<IListBlobItem> blobItems)
        {
            foreach (var blob in blobItems)
            {
                //Console.WriteLine(blob.Uri); //https://Azureblobdomain/containerName/blobFile...

                if (blob is CloudBlobDirectory cloudBlobDirectory)
                {
                    Console.WriteLine("----" + cloudBlobDirectory.Prefix + "----");
                    GotoInnerBlobs(cloudBlobDirectory.ListBlobs());
                }
                else if (blob is CloudBlob blobItem)
                {
                    Console.WriteLine(blobItem.Name);
                }
            }
        }
        /// <summary>
        /// 移動目標位置
        /// </summary>
        /// <param name="sourceUrn">來源位置</param>
        /// <param name="destinationUrn">目標位置</param>
        public void MoveBlobFile(string sourceUrn, string destinationUrn)
        {
            if (string.IsNullOrEmpty(sourceUrn) == false && string.IsNullOrEmpty(destinationUrn) == false)
            {
                CloudStorageAccount sourceAccount = CloudStorageAccount.Parse(_SourceConectString);
                CloudBlobClient client = sourceAccount.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(sourceUrn);
                CloudBlockBlob sourceBlockBlob = container.GetBlockBlobReference(sourceUrn);
                CloudBlockBlob destinationBlockBlob = container.GetBlockBlobReference(sourceUrn);
                TransferManager.CopyAsync(sourceBlockBlob, destinationBlockBlob, true/* isServiceCopy */);
            }
            else
            {
                throw new Exception("URN is not exist");
            }
        }
    }
}
