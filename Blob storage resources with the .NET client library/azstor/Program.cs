using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;

Console.WriteLine("Azure Blob Storage exercise\n");

// Create a DefaultAzureCredentialOptions object to configure the DefaultAzureCredential
DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

// Run the examples asynchronously, wait for the results before proceeding
await ProcessAsync();

Console.WriteLine("\nPress enter to exit the sample application.");
Console.ReadLine();

async Task ProcessAsync()
{
    // CREATE A BLOB STORAGE CLIENT
        // Create a credential using DefaultAzureCredential with configured options
        string accountName = "storageacct11788"; // Replace with your storage account name

        // Use the DefaultAzureCredential with the options configured at the top of the program
        DefaultAzureCredential credential = new DefaultAzureCredential(options);

        // Create the BlobServiceClient using the endpoint and DefaultAzureCredential
        string blobServiceEndpoint = $"https://{accountName}.blob.core.windows.net";
        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), credential);

    // CREATE A CONTAINER
        // Create a unique name for the container
        string containerName = "wtblob" + Guid.NewGuid().ToString();

        // Create the container and return a container client object
        Console.WriteLine("Creating container: " + containerName);
        BlobContainerClient containerClient = 
            await blobServiceClient.CreateBlobContainerAsync(containerName);

        // Check if the container was created successfully
        if (containerClient != null)
        {
            Console.WriteLine("Container created successfully, press 'Enter' to continue.");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("Failed to create the container, exiting program.");
            return;
        }


    // CREATE A LOCAL FILE FOR UPLOAD TO BLOB STORAGE
        // Create a local file in the ./data/ directory for uploading and downloading
        Console.WriteLine("Creating a local file for upload to Blob storage...");
        string localPath = "./data/";
        string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
        string localFilePath = Path.Combine(localPath, fileName);

        // Write text to the file
        await File.WriteAllTextAsync(localFilePath, "Hello, World!");
        Console.WriteLine("Local file created, press 'Enter' to continue.");
        Console.ReadLine();


    // UPLOAD THE FILE TO BLOB STORAGE
        // Get a reference to the blob and upload the file
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}", blobClient.Uri);

        // Open the file and upload its data
        using (FileStream uploadFileStream = File.OpenRead(localFilePath))
        {
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();
        }

        // Verify if the file was uploaded successfully
        bool blobExists = await blobClient.ExistsAsync();
        if (blobExists)
        {
            Console.WriteLine("File uploaded successfully, press 'Enter' to continue.");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("File upload failed, exiting program..");
            return;
        }


    // LIST BLOBS IN THE CONTAINER
        Console.WriteLine("Listing blobs in container...");
        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            Console.WriteLine("\t" + blobItem.Name);
        }

        Console.WriteLine("Press 'Enter' to continue.");
        Console.ReadLine();


    // DOWNLOAD THE BLOB TO A LOCAL FILE
        string downloadFilePath = Path.Combine(localPath, "downloaded-" + fileName);
        Console.WriteLine("Downloading blob to local file: " + downloadFilePath);

        using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
        {
            await blobClient.DownloadToAsync(downloadFileStream);
            downloadFileStream.Close();
        }

        Console.WriteLine("Download completed, press 'Enter' to continue.");
        Console.ReadLine();


}