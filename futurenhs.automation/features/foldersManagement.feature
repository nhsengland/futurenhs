Feature: forumAdmin
    User journeys covering functionality and navigation around forums as a Site/Forum Administrator

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Admin Group' group card
        Then the 'Automation Admin Group' header is displayed
        When I click the 'Files' tab
        Then the 'Files' header is displayed


    Scenario: FNHS64 - Group Files tab page validation
        Then the 'Add Folder' link is displayed
        And the 'Group Files' table is displayed
            | Type | Name                   | Description                        | Modified |
            |      | Automation Test Folder |                                    |          |
            |      | DeleteFolder           | Folder to be deleted by automation |          |
            |      | EditableFolder         | Folder to be edited by automation  |          |
            |      | Empty Folder           | Empty Folder for testing           |          |


    Scenario: FNHS65 - Folder page validation
        When I click the 'Automation Test Folder' link
        Then the 'Automation Test Folder' header is displayed
        Then the 'Upload File' link is displayed
        And the 'Add Folder' link is displayed
        And the 'Edit Folder' link is displayed
        # And the 'Delete Folder' link is displayed
        And the 'Group Files' table is displayed
            | Type | Name       | Description      | Modified                                      | Actions                     |
            |      | Folder 1   |                  |                                               |                             |
            |      | docTest    | Test doc FNHS105 | 10 Jan 2022\nBy auto Admin\nAuthor auto Admin | Download file\nView details |
            |      | pdfTest    | test pdf         | 16 Dec 2021\nAuthor auto Admin                | Download file\nView details |
            |      | test excel | test excel       | 05 Nov 2021\nAuthor auto Admin                | Download file\nView details |
            |      | test ppt   | test ppt         | 05 Nov 2021\nAuthor auto Admin                | Download file\nView details |


    Scenario: FNHS92 - Empty Folder page validation
        When I click the 'Empty Folder' link
        Then the 'Empty Folder' header is displayed
        And the 'Empty Folder for testing' textual value is displayed
        Then the 'Upload File' link is displayed
        And the 'Add Folder' link is displayed
        And the 'Edit Folder' link is displayed
        # And the 'Delete Folder' link is displayed
        And the 'Group Files' table is not displayed


    @Core
    Scenario: FNHS66 - Create a folder
        Then the 'Add Folder' link is displayed
        When I click the 'Add Folder' link
        Then the 'Add Folder' header is displayed
        When I enter '<title>' into the 'Enter a folder title' field
        And I enter '<description>' into the 'Enter a folder description' text area
        And I click the 'Save and continue' button
        Then the '<title>' header is displayed
        Examples:
            | title         | description                   |
            | AutoFolder    | automation folder description |
            | [STRING: 200] | Title 200 char limit test     |
            | 4000DescTest  | [STRING: 4000]                |



    Scenario Outline: FNHS67 - Create a folder error validation
        When I click the 'Add Folder' link
        Then the 'Add Folder' header is displayed
        When I enter '<title>' into the 'Enter a folder title' field
        And I enter '<description>' into the 'Enter a folder description' text area
        And I click the 'Save and continue' button
        Then the '<errorMsg>' error message is displayed
        And the '<errorMsg>' error summary is displayed
        Examples:
            | title         | description                   | errorMsg                       |
            |               |                               | Enter the folder title         |
            | Empty Folder  | automation folder description | Enter a unique folder title    |
            | [STRING: 201] | description                   | Enter 200 or fewer characters  |
            | FolderName    | [STRING: 4001]                | Enter 4000 or fewer characters |

    @Core
    Scenario: FNHS68 - Update a folder with new title
        When I click the 'EditableFolder' link
        Then the 'EditableFolder' header is displayed
        When I click the 'Edit Folder' link
        Then the 'Edit Folder' header is displayed
        And the 'Enter a folder title' field contains 'EditableFolder'
        When I enter 'EditedFolder' into the 'Enter a folder title' field
        And I click the 'Save and continue' button
        Then the 'EditedFolder' header is displayed


    Scenario Outline: FNHS69 - Update folder error validation
        When I click the 'AutoFolder' link
        Then the 'AutoFolder' header is displayed
        When I click the 'Edit Folder' link
        Then the 'Edit Folder' header is displayed
        And the 'Enter a folder title' field contains 'AutoFolder'
        When I enter '<foldertitle>' into the 'Enter a folder title' field
        When I enter '<description>' into the 'Enter a folder description' text area
        And I click the 'Save and continue' button
        Then the '<errorMsg>' error message is displayed
        Then the '<errorMsg>' error summary is displayed
        Examples:
            | foldertitle   | description    | errorMsg                       |
            |               |                | Enter the folder title         |
            | Empty Folder  |                | Enter a unique folder title    |
            | [STRING: 201] | description    | Enter 200 or fewer characters  |
            | FolderName    | [STRING: 4001] | Enter 4000 or fewer characters |

    @Pending
    # FEATURE NOT YET IN PRIVATE BETA
    Scenario: FNHS70 - Delete folder, Cancel action
        When I click the 'DeleteFolder' link
        Then the 'DeleteFolder' header is displayed
        When I click the 'Delete Folder' link
        Then the 'Folder will be deleted' header is displayed
        And the 'Any folder contents will also be discarded. Are you sure you wish to proceed?' textual value is displayed
        When I cancel this on the open dialog
        Then the 'DeleteFolder' header is displayed

    @Core @Pending
    # FEATURE NOT YET IN PRIVATE BETA
    Scenario: FNHS71 - Delete a folder
        When I click the 'DeleteFolder' link
        Then the 'DeleteFolder' header is displayed
        When I click the 'Delete Folder' link
        Then the 'Folder will be deleted' header is displayed
        And the 'Any folder contents will also be discarded. Are you sure you wish to proceed?' textual value is displayed
        When I confirm this on the open dialog
        Then the 'Files' header is displayed
        Then the 'AutoFolderEdited' link is not displayed