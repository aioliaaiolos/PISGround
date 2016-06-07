Notes:
- All WSDL files and their corresponding .xsd Files have been generated in the utest Client projects.
- Note also, that the WSDL contains host addresses, they should correspond to the service hosted by IIS.
- Generation was done using the "Add Service Reference" in client project.
- WSDL and its corresponding XSDs can be used to generate the "app.config" and the client proxy "Reference.cs" by issuing the following command:
                  svcutil /out:Reference.cs /config:app.config path/file.wsdl *.xsd 