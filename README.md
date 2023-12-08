# Icod Work on Demand Framework

## What the Hell is this thing?
The Icod Work on Demand Framework, or WoD, is a class library intended to automate a great variety of tasks.
These tasks can be roughly divided into categories of:
* File Operations
* Database Operations
* Salesforce Operations
* Email Operations

A common job might be to download records from multiple SalesForce tables, load them into a SQL database to 
process the records with a SQL program, export the modified records back into SalesForce, export other 
records as a set of XML files, copy the XML files to an SFTP site, export another set of records as a set of 
JSON files, export those JSON files to an on-line web resource (such as WorkDay), and email a CSV report file 
to some other people.  And do all of this on a specific schedule, such as at 7pm Monday through Friday.  And 
describe the work in a platform neutral format, such a human-readable XML file (called a Schematic), rather 
than some binary tool offered by a particular Database vendor.

Oh! And did I mention you don't need a cloud? Yes, it will work on any platform which supports .Net 
Framework 4.8.1.  Such as any desktop, application server, or even your smartphone.  Yes you can have a cloud 
if you like, but you don't need one.

The WoD exposes the necessary classes and interfaces to perform all this work.  There is a corresponding 
client program, [Icod.Wod.Client](https://github.com/uniblab/Icod.Wod.Client), which can be used for running 
WoD Schematics.  It would typically be invoked aline the lines of:
``` sh
\\application-server\Icod.Wod\Icod.Wod.Client.exe \\application-server\jobs\daily\SFstaff-to-Workday.xml
```

### Database Operations
These operations involve working with a database of some sort.  The WoD Framework will connect to any 
database product for which you have a driver and can specify via a connection-string.
See [ConnectionStrings.com](https://www.connectionstrings.com/).

Example:
``` xml
<dbFileExport connectionStringName="CustomerData" namespace="dbo"
    commandText="getCustomerMailingList" commandType="StoredProcedure"
>
    <destination xsi:type="delimitedFile" codePage="utf-8" convertEmptyStringToNull="false" hasHeader="true"
        fieldSeperator="44" quoteChar="34" recordSeparator="&#13;&#10;" append="false"
        path="\\file-store\weekly-reports\" name="customer-mailing-list.csv" writeIfEmpty="true"
    />
</dbFileExport>
```

#### DbCommand
Executes a stored procedure or even raw text in the specified database.

#### FileExport
Exports data to a specified file.

#### FileImport
Imports data from a specified file.


### Email Operations
Sends email to one or more specified addresses.  May contain zero or more file attachments.

Example:
``` xml
<email to="boss@example.com" sendIfEmpty="false" subjectCodePage="us-ascii" subject="Customer mailing list">
	<attachments>
		<attach path="\\file-store\weekly-reports\" name="customer-mailing-list.csv" />
	</attachments>
</email>
```


### File Operations
There are a great many file operations supported by the Icod.Wod Framework, but we should first describe the 
different end-points supported.

Example:
``` xml
<fileOperation xsi:type="deleteFile" path="\\file-store\weekly-reports\" name="customer-mailing-list.csv" />
```

#### End-points
A file or directory is specified by an end-point.  This end-point could be almost anything.  A web page, 
a spreadsheet on an Sftp site, or even a Jpeg on the local computer itself.  
All operations require at least one end-point specification.

##### Local Files
This is any file accessible by any installed file system driver, such as NTFS or Ext4, or CIFS, or even NFS.

##### Ftp/Ftps
This is any file accessible via the Ftp or Ftps protocols.

##### Http/Https
This is any file accessible via the Http or Https protocols.

##### Sftp
This is any file accessible via the Sftp protocol.

#### Operations
These are the core function which manipulate one or more files or directories.  Each requires one or more 
end-points.

##### AddZip
Appends one or more files to a Zip archive.

##### AppendFile
Appends text from one or more files to the end of a file.

##### CopyFile
Copies one or more files.

##### DeflateFile
Attempts to decompress one or more files with the Deflate algorithm.

##### DeleteFile
Attempts to delete one or more files.

##### ExecuteFile
Attempts to launch the file as an executable.

##### ExistsFile
Perform specified work if and only if the specified file exists.

##### FromZip
Retrieve/unpack one or more files from one or more Zip archives.

##### GZipFile
Compresses the specified files using the GZip algorithm.

##### HeadFile
Retrieve the top-most specified number of lines from one or more files.

##### JsonToXml
Translates the specified Json files to Xml files.

##### List
List all files and directories in a specified path.

##### ListDirectory
List all directories in a specified path.

##### ListFile
List all files in a specified path.

##### ListZip
List the contents of the specified Zip archive.

##### MkDir
Creates a directory in the specified path.

##### MkZip
Creates a Zip archive.

##### PreambleFile
Prepends text to the beginning of the specified ciles.

##### PrefixFile
Prefixes each line of the specified files with the specified text.

##### PruneFile
Removes all blank lines and trims all trailing and leading whitespace from the lines of the specified files.

##### RebaseFile
Changes the Code Page of the specified files.

##### RenameFile
Renames the specified file.

##### RmDir
Removes the specified directory.

##### RmZip
Removes the specified file from a Zip archive.

##### SuffixFile
Appends the specified text to each line in the specified files.

##### TailFile
Retrieve the bottom-most specified number of lines from one or more files.

##### TouchFile
Creates the specified file if it does not exist; otherwise it updates the last-write and last-access times.

##### TouchZip
Creates the specified Zip archive if it does not exist; otherwise it updates the last-write and last-access times.

##### XmlToJson
Translates the specified Xml files to Json files.

##### XmlTransformFile
Transforms the specified XML files according to the rules of the specified XSLT file.


## Salesforce Operations
These operations are for exchanging data with the SalesForce cloud.

### BulkOperations
These operations use the highly efficient Bulk API.

#### Delete
Deletes the specified records.

#### Insert
Inserts the specified records.

#### Query
Downloads records from the SalesForce cloud as per the specified soql to a file.

#### Update
Updates the specified records.

#### Upsert
Perhaps an upsert opertion in the SalesForce cloud as per the specified records.

### RestSelect
Downloads records from the SalesForce cloud as per the specified soql to a file, using the inefficient Rest Api.


## Does it need some sort of cloud?
No.  It'll run on any computer which supports the .Net Framework 4.8.1.

## Is it hard to write a WoD Schematic?
It's rather easy.  
A schematic is a collection of one or more steps.  
Each step is an operation to execute, such as delete or email a file.  
Some steps can be done in paralell, and even conditionally on the existence of some file.
Since we like intellisense and autocomplete there is a pair of Xsd schema files which Visual Studio, Glade, and Notepad++ will use.  
Thanks to intellisense and autocomplete you can focus on ''what'' you want to do rather than ''how'' to do it.  
If what you want done can't be done with those then give me a ring and we'll see how your task fits into things.

## Do you have an example I could take a look at?
Sure! You can find a few examples here, [at my pastebin](https://pastebin.com/u/uniblab/1/94QwU3QE).

## Wow! This does more than Dell's Boomi.
Yes, I know.  It's easier to use too.  And did I mention it doesn't need a cloud and doesn't cost a fortune?
