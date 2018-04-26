# SharePoint Functions #
A template function for calling SharePoint from Microsoft Flow with the help of Azure Functions.



## Create SharePoint Folder Action ##

Parameters:
- sharePointSiteUrl — SharePoint site URL
- baseFolderServerRelativeUrl — Server relative URL of a folder where you want to create a sub-folder
- newFolderName — Name for a new sub-folder


## Get Next Available SP File Name Action ##

Parameters
- sharePointSiteUrl - SharePoint site path
- fileServerRelativeUrl - Server relative URL path of where to test for an existing file
- newFileName - Name for file to test
