using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobSample
{
    public class AzureBlobService
    {
        string _SourceConectString { get; set; }
        public AzureBlobService(string sourceConectString)
        {
            _SourceConectString = sourceConectString;
        }

        /// <summary>
        /// 印出Container
        /// </summary>
        /// <param name="containerName">container name</param>
        public void PrintContainer(string containerName, string blobDirectoryURI = "")
        {
            CloudStorageAccount sourceAccount = CloudStorageAccount.Parse(_SourceConectString);
            CloudBlobClient client = sourceAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            if (string.IsNullOrEmpty(blobDirectoryURI) == false)
            {
                GotoInnerBlobs(container.GetDirectoryReference(blobDirectoryURI).ListBlobs());
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
        /// <param name="containerName">Container 名稱</param>
        /// <param name="sourceURI">來源位置</param>
        /// <param name="destinationURI">目標位置</param>
        public void MoveBlobFile(string containerName, string sourceURI, string destinationURI)
        {
            if (string.IsNullOrEmpty(sourceURI) == false && string.IsNullOrEmpty(destinationURI) == false)
            {
                CloudStorageAccount sourceAccount = CloudStorageAccount.Parse(_SourceConectString);
                CloudBlobClient client = sourceAccount.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(containerName);
                CloudBlockBlob sourceBlockBlob = container.GetBlockBlobReference(sourceURI);
                CloudBlockBlob destinationBlockBlob = container.GetBlockBlobReference(sourceURI);
                TransferManager.CopyAsync(sourceBlockBlob, destinationBlockBlob, true/* isServiceCopy */);
            }
            else
            {
                throw new Exception("URI is not exist");
            }
        }
        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="stream">檔案串流</param>
        /// <param name="containerName">Container 名稱</param>
        /// <param name="URI">URI(預期路徑.檔案名稱.副檔名)</param>
        /// <param name="extension">副檔名</param>
        public void Upload(Stream stream, string containerName, string URI, string extension)
        {
            CloudStorageAccount sourceAccount = CloudStorageAccount.Parse(_SourceConectString);
            CloudBlobClient client = sourceAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(URI);
            cloudBlockBlob.Properties.ContentType = extension;
            cloudBlockBlob.UploadFromStream(stream);
        }
    }
}
