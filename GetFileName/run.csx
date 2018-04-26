using System;
using System.Net;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
using PnPAuthenticationManager = OfficeDevPnP.Core.AuthenticationManager;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{

    dynamic data = await req.Content.ReadAsAsync<object>();

    string sharePointSiteUrl = data["sharePointSiteUrl"];
    string fileServerRelativeUrl = data["fileServerRelativeUrl"];
    string newFileName = data["newFileName"];

    log.Info($"sharePointSiteUrl = '{sharePointSiteUrl}'");
    log.Info($"fileServerRelativeUrl = '{fileServerRelativeUrl}'");
    log.Info($"newFileName = '{newFileName}'");

    string userName = System.Environment.GetEnvironmentVariable("SharePointUser", EnvironmentVariableTarget.Process);
    string password = System.Environment.GetEnvironmentVariable("SharePointPassword", EnvironmentVariableTarget.Process);
   

    var authenticationManager = new PnPAuthenticationManager();
    var clientContext = authenticationManager.GetSharePointOnlineAuthenticatedContextTenant(sharePointSiteUrl, userName, password);
    var pnpClientContext = PnPClientContext.ConvertFrom(clientContext);

    string newFileUrl = UrlUtility.Combine(fileServerRelativeUrl, newFileName);

    string availableFileName = "";

    if(!doesFileExist(pnpClientContext, newFileUrl)) {
        availableFileName = newFileName;
    }
    else
    {
        string altFilename;
        string altFileUrl;
        int filenameIndex = 0;
        do {
            filenameIndex += 1;
            altFilename = createNumberedFilename(newFileName, filenameIndex);
            altFileUrl = UrlUtility.Combine(fileServerRelativeUrl, altFilename);
        } while (doesFileExist(pnpClientContext, altFileUrl));

        availableFileName = altFilename;
        //var file = pnpClientContext.Web.GetFilebyServerRelativeUrl(fileServerRelativeUrl);
        //folder.AddSubFolder(newFileName);

        pnpClientContext.ExecuteQuery();

    }
    
    return req.CreateResponse(HttpStatusCode.OK, new{filename = availableFileName});
}

public static string createNumberedFilename(string filename, int number) {

    string fileRootName = System.IO.Path.GetFileNameWithoutExtension(filename);
    string fileNumber = "-" + number.ToString("00");
    string extension = System.IO.Path.GetExtension(filename);

    return string.Format("{0}{1}{2}", fileRootName, fileNumber, extension );

}

public static bool doesFileExist(PnPClientContext clientContext, string fileUrl)
{
    try
    {
        // The serverRelativeUrl does not start with a '/' or serverRelativeUrl does not correspond to a file.
        var file = clientContext.Web.GetFileByServerRelativeUrl(fileUrl);
        clientContext.Load(file);
        clientContext.ExecuteQuery();

        return true;
    }
    catch(Microsoft.SharePoint.Client.ServerException ex)
    {
        if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
            {
                //file = null;
                return false;
            }
            else 
                throw;
        
    }
}