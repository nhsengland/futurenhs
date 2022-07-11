Feature: filesManagement
	User journeys around management of Files within a group as an admin on the Future NHS platform

	Background:
		Given I have navigated to '/'
		And I have logged in as an 'group admin' and accept the cookies
		Then the 'My Groups' header is displayed
		When I select the 'Automation Admin Group' group card
		Then the 'Automation Admin Group' header is displayed
		When I click the 'Files' tab
		Then the 'Files' header is displayed

	@Core @NotInLocal
	Scenario Outline: FNHS72 - Upload a file
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload a file' header is displayed
		When I enter '<File name>' into the 'Enter a file title' field
		When I enter '<File description>' into the 'Enter a file description' text area
		When I upload the '<File to upload>' file
		When I click the 'Upload File' button
		Then the 'Empty Folder' header is displayed
		And the '<File name>' row is displayed on the 'Group Files' table
		Examples:
			| File to upload                                                                                              | File name            | File description                           |
			| /media/docTest.doc                                                                                          | docTest              | test doc file for upload                   |
			| /media/pdfTest.pdf                                                                                          | pdfTest              | test pdf file for upload                   |
			| /media/docTest.doc                                                                                          | [STRING: 45]         | test file with title of 45 characters      |
			| /media/docTest.doc                                                                                          | descriptionTest      | [STRING: 150]                              |
			| /media/JdDBz18tDwh1jC2pK3RZHzEJPyLLJUfA6qTcs23MBesG7kZfVvltwg5ixHk2zXEmc8xNYXpunGHL67QAX4EJ3yEAINQMjZp3.pdf | hundredCharacterFile | test file with file name of 100 characters |
			| /media/x.pdf                                                                                                | singleCharacterFile  | test file with file name of 1 character    |


	Scenario: FNHS73 - File details page validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click 'View details' on the 'docTest' row of the 'Group Files' table
		Then the 'docTest' header is displayed
		And the 'Test doc FNHS105' textual value is displayed
		And the 'Owner' textual value is displayed
		And the 'File data' textual value is displayed
		And the 'docTest' row is displayed on the 'File Details' table
		And the breadcrumb navigation displays 'Files > Automation Test Folder'

	@NotInLocal
	Scenario: FNHS19 - File preview page validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click 'docTest' on the 'docTest' row of the 'Group Files' table
		Then the 'docTest' header is displayed
		And the collabora file preview is displayed
		And the 'View details' link is displayed


	Scenario Outline: FNHS75 - Upload a file, form error validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload a file' header is displayed
		When I enter '<File name>' into the 'Enter a file title' field
		When I enter '<File description>' into the 'Enter a file description' text area
		When I upload the '<File to upload>' file
		When I click the 'Upload File' button
		Then the '<error message>' error message is displayed
		Examples:
			| File to upload     | File name    | File description | error message                 |
			| /media/docTest.doc |              | Description      | Enter the file title          |
			| /media/docTest.doc | [STRING: 46] | Description      | Enter 45 or fewer characters  |
			| /media/docTest.doc | Title        | [STRING: 151]    | Enter 150 or fewer characters |


	Scenario: FNHS76 - Upload without a file error validation
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		When I click the 'Upload File' link
		Then the 'Upload a file' header is displayed
		When I enter 'Title' into the 'Enter a file title' field
		When I enter 'Description' into the 'Enter a file description' text area
		When I click the 'Upload File' button
		Then the 'Add a file' error message is displayed

	@Core
	Scenario Outline: FNHS77 - File download and verify
		When I click the 'Empty Folder' link
		Then the 'Empty Folder' header is displayed
		And the '<fileName>' row is displayed on the 'Group Files' table
		And I download the '<file>' file and compare against the uploaded version
		Examples:
			| fileName | file        |
			| docTest  | docTest.doc |
			| pdfTest  | pdfTest.pdf |

	@Pending
	# FEATURE NOT IN PRIVATE BETA
	Scenario: FNHS88 - Edit File Page Validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'docTest' link
		Then the 'docTest' header is displayed
		When I click the 'Edit file' link
		Then the 'Edit file' header is displayed
		And the 'Enter file title' field contains 'docTest'
		And the 'Enter file description' text area contains 'test doc file for upload'

	@Pending
	# FEATURE NOT IN PRIVATE BETA
	Scenario: FNHS89 - Edit File
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'docTest' link
		Then the 'docTest' header is displayed
		When I click the 'Edit file' link
		Then the 'Edit file' header is displayed
		And I enter 'Doc Test' into the 'Enter file title' field
		And I enter 'New File Description' into the 'Enter file description' text area
		And I click the 'Save file details' option
		Then the 'Doc Test' header is displayed
		And the 'New File Description' textual value is displayed

	@Pending
	# FEATURE NOT IN PRIVATE BETA
	Scenario Outline: FNHS90 - Edit File error validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		When I click the 'Doc Test' link
		Then the 'Doc Test' header is displayed
		When I click the 'Edit file' link
		Then the 'Edit file' header is displayed
		And I enter '<Title>' into the 'Enter file title' field
		And I enter '<Description>' into the 'Enter file description' text area
		And I click the 'Save file details' option
		And the '<Error Message>' error message is displayed
		Examples:
			| Title        | Description   | Error Message                 |
			|              | Description   | Please provide a file name    |
			| [STRING: 46] |               | Enter 45 or fewer characters  |
			| Title        | [STRING: 151] | Enter 150 or fewer characters |


	Scenario: FNHS91 - Files Page Breadcrumb Validation
		When I click the 'Automation Test Folder' link
		Then the 'Automation Test Folder' header is displayed
		And the breadcrumb navigation displays 'Files'
		When I click the 'Folder 1' link
		Then the 'Folder 1' header is displayed
		And the breadcrumb navigation displays 'Files > Automation Test Folder'
		When I click the 'Folder 2' link
		Then the 'Folder 2' header is displayed
		And the breadcrumb navigation displays 'Files > Automation Test Folder > Folder 1'
		When I click the 'Folder 3' link
		Then the 'Folder 3' header is displayed
		And the breadcrumb navigation displays 'Files > Automation Test Folder > Folder 1 > Folder 2'
		When I click the 'Folder 4' link
		Then the 'Folder 4' header is displayed
		And the breadcrumb navigation displays 'Files > Automation Test Folder > Folder 1 > Folder 2 > Folder 3'
		When I click the 'Folder 5' link
		Then the 'Folder 5' header is displayed
		And the breadcrumb navigation displays 'Files > ... > Folder 2 > Folder 3 > Folder 4'
		When I click the 'Folder 2' link
		Then the 'Folder 2' header is displayed
		And the breadcrumb navigation displays 'Files > Automation Test Folder > Folder 1'
