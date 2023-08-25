using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Commons
{
    public static class Authentication
    {
        public static CloudStorageAccount authAccountStorage()
        {

            try
            {
                //Conexión con el azure storage
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cecdev;AccountKey=znWMhJrLlfb7aRsvOiJdnvYMxEjaAF2NRms2Rv4gNM5oBIN4fVpzdgsh37N+6asaPPPvr03bRVWICMpcYRW25w==;EndpointSuffix=core.windows.net");
                return storageAccount;
            }
            catch (Exception ex)
            {
                var error = $"error when trying to authenticate the storage account({ex.ToString()})";
                throw new System.ArgumentException(error);
            }

        }
    }
}
