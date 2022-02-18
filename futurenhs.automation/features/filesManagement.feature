Feature: Files and Folders
		User Journeys around management of Files within a group as an admin on the Future NHS platform

Background:
		Given I have navigated to '/'
		And I have logged in as an 'admin'
    	And I click the 'Groups' nav icon
		Then the 'My Groups' header is displayed
		When I click the 'Automation Admin Group' link
		Then the 'Automation Admin Group' header is displayed
		When I click the 'Files' tab
		Then the 'Files' header is displayed

@Core
Scenario Outline: FNHS72 - Upload a file
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload File' header is displayed
		When I enter '<File name>' into the 'Enter file title' field
		When I enter '<File description>' into the 'Enter file description' text area
		When I upload the '<File to upload>' file
		When I click the 'Upload' option
		Then the 'Empty Folder' header is displayed
		And the '<File name>' row is displayed on the 'Group Files' table
Examples:
		| File to upload                                                                                              | File name                                     | File description                                                                                                                                       |
		| /media/docTest.doc                                                                                          | docTest                                       | test doc file for upload                                                                                                                               |
		| /media/pdfTest.pdf                                                                                          | pdfTest                                       | test pdf file for upload                                                                                                                               |
		| /media/JdDBz18tDwh1jC2pK3RZHzEJPyLLJUfA6qTcs23MBesG7kZfVvltwg5ixHk2zXEmc8xNYXpunGHL67QAX4EJ3yEAINQMjZp3.pdf | hundredCharacterFile                          | test file with file name of 100 characters                                                                                                             |
		| /media/x.pdf                                                                                                | singleCharacterFile                           | test file with file name of 1 character                                                                                                                |
		| /media/docTest.doc                                                                                          | VSmnp7PvnPRhfiZWCk5GdSjT2nD3TKlxBE2AbyObV1Yza | test file with title of 45 character                                                                                                                   |
		| /media/docTest.doc                                                                                          | descriptionTest                               | gEcdosYCBQC7sY9XxOdPKxMWBqfHY3A78F5sROBjv71hfVuj0nl03SXyGPzychj3ffH7vWW7yKYYbZayRWauCThasQDwfVIawmEiI0HFfBETQzSGnaYOQfq3Nh0HXV89M1sYu5fozAJMzvTA20FezL |


Scenario: FNHS73 - File page validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click 'docTest' on the 'docTest' row of the 'Group Files' table
		Then the 'docTest' header is displayed
		And the 'test doc file for upload' textual value is displayed
		And the 'docTest.doc' row is displayed on the 'File Details' table
		And the breadcrumb navigation displays 'Files Empty Folder DocTest'
		And the 'File preview' header is displayed
		And the collabora file preview is displayed


Scenario Outline: FNHS74 - Upload a file, file error validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload File' header is displayed
		When I enter '<File name>' into the 'Enter file title' field
		When I enter '<File description>' into the 'Enter file description' text area
		When I upload the '<File to upload>' file
		When I click the 'Upload' option
		Then the '<error message>' error summary is displayed
Examples:
		| File to upload                                                                                                   | File name                                      | File description                                                                                                                                        | error message                                                            |
		| /media/test.jpg                                                                                                  | Title                                          | Description                                                                                                                                             | The file above was not uploaded because the type is not allowed.         |
		| /media/.txt                                                                                                      | Title                                          | Description                                                                                                                                             | The file must have a name.                                               |
		| /media/JdDBz18tDwh1jC2pK3RZHzEJPyLLJUfA6qTcs23MBesG7kZfVvltwg5ixHk2zXEmc8xNYXpunGHL67QAX4EJ3yEAINQMjZp3cn257.pdf | Title                                          | Description                                                                                                                                             | The name of the file cannot be more than 100 characters.                 |
		| /media/invalidDocTest.doc                                                                                        | InvalidFile                                    | Description                                                                                                                                             | The file above was not uploaded because the type could not be identified |
		| /media/docTest.doc                                                                                               | VSmnfp7PvnPRhfiZWCk5GdSjT2nD3TKlxBE2AbyObV1Yza | Description                                                                                                                                             | The title cannot be more than 45 characters.                             |
		| /media/docTest.doc                                                                                               | Title                                          | gEcdosYCBQC7sY9XxOdPKxMWBqfHY3A78F5sROBjv71hfVuj0nl03SXsyGPzychj3ffH7vWW7yKYYbZayRWauCThasQDwfVIawmEiI0HFfBETQzSGnaYOQfq3Nh0HXV89M1sYu5fozAJMzvTA20FezL | The description cannot be more than 150 characters.                      |

Scenario Outline: FNHS75 - Upload a file, form error validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload File' header is displayed
		When I enter '<File name>' into the 'Enter file title' field
		When I enter '<File description>' into the 'Enter file description' text area
		When I upload the '<File to upload>' file
		When I click the 'Upload' option
		Then the '<error message>' error summary is displayed
		Then the '<error message>' error message is displayed
Examples:
		| File to upload     | File name | File description | error message              |
		| /media/docTest.doc |           | Description      | The Name field is required |

Scenario: FNHS76 - Upload without a file error validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload File' header is displayed
		When I enter 'Title' into the 'Enter file title' field
		When I enter 'Description' into the 'Enter file description' text area
		When I click the 'Upload' option
		Then the 'Please select a file to upload' error message is displayed

@Core
Scenario Outline: FNHS77 - File download and verify
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the '<fileName>' link
		Then the '<fileName>' header is displayed
		And I download the '<file>' file and compare against the uploaded version
Examples:
		| fileName | file        |
		| docTest  | docTest.doc |
		| pdfTest  | pdfTest.pdf |


Scenario: FNHS88 - Edit File Page Validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'docTest' link
		Then the 'docTest' header is displayed
		When I click the 'Edit ile' link
		Then the 'Edit ile' header is displayed
		And the 'Enter file title' field contains 'docTest'
		And the 'Enter file description' text area contains 'test doc file for upload'


Scenario: FNHS89 - Edit File
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'docTest' link
		Then the 'docTest' header is displayed
		When I click the 'Edit ile' link
		Then the 'Edit ile' header is displayed
		And I enter 'Doc Test' into the 'Enter file title' field
		And I enter 'New File Description' into the 'Enter file description' text area
		And I click the 'Save file details' option
		Then the 'Doc Test' header is displayed
		And the 'New File Description' textual value is displayed


Scenario Outline: FNHS90 - Edit File error validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'Doc Test' link
		Then the 'Doc Test' header is displayed
		When I click the 'Edit ile' link
		Then the 'Edit ile' header is displayed
		And I enter '<Title>' into the 'Enter file title' field
		And I enter '<Description>' into the 'Enter file description' text area
		And I click the 'Save file details' option
		Then the '<Error Message>' error summary is displayed
		And the '<Error Message>' error message is displayed
Examples:
		| Title                                          | Description | Error Message                                       |
		|                                                | Description | Please provide a file name                          |
		| VSmnfp7PvnPRhfiZWCk5GdSjT2nD3TKlxBE2AbyObV1Yza |             | The file title must not be more than 45 characters  |
		| Title | gEcdosYCBQC7sY9XxOdPKxMWBqfHY3A78F5sROBjv71hfVuj0nl03SXsyGPzychj3ffH7vWW7yKYYbZayRWauCThasQDwfVIawmEiI0HFfBETQzSGnaYOQfq3Nh0HXV89M1sYu5fozAJMzvTA20FezL | The file description must not be more than 150 characters |


Scenario: FNHS91 - Files Page Breadcrumb Validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'Folder 1' link
		Then the 'Folder 1' header is displayed
		When I click the 'Folder 2' link
		Then the 'Folder 2' header is displayed
		When I click the 'Folder 3' link
		Then the 'Folder 3' header is displayed
		And the breadcrumb navigation displays 'Files ... Folder 1 Folder 2 Folder 3'
		When I click the '...' link
		Then the 'Automation Test Folder' header is displayed