Feature: forumAdmin
    User journeys covering functionality and navigation around forums as a Site/Forum Administrator 

Background:
    Given I have navigated to '/'
    And I have logged in as a 'admin' and accept the cookies
    Given I click the 'Groups' nav icon
    Then the 'My Groups' header is displayed
    When I click the 'Automation Admin Group' link
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
    And the 'Delete Folder' link is displayed 
    And the 'Group Files' table is displayed
    | Type | Name       | Description | Modified                                      | Actions                     |
    |      | Folder 1   |             |                                               |                             |
    |      | docTest    | test doc    | 10 Jan 2022\nBy auto Admin\nAuthor auto Admin | Download file\nView details |
    |      | pdfTest    | test pdf    | 16 Dec 2021\nAuthor auto Admin                | Download file\nView details |
    |      | test excel | test excel  | 05 Nov 2021\nAuthor auto Admin                | Download file\nView details |
    |      | test ppt   | test ppt    | 05 Nov 2021\nAuthor auto Admin                | Download file\nView details |


Scenario: FNHS92 - Empty Folder page validation
    When I click the 'Empty Folder' link
    Then the 'Empty Folder' header is displayed
    And the 'Empty Folder for testing' textual value is displayed     
    Then the 'Upload File' link is displayed
    And the 'Add Folder' link is displayed
    And the 'Edit Folder' link is displayed
    And the 'Delete Folder' link is displayed
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
    | title      | description                   |
    | AutoFolder | automation folder description |
    | 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 | 



Scenario Outline: FNHS67 - Create a folder error validation
    When I click the 'Add Folder' link
    Then the 'Add Folder' header is displayed
    When I enter '<title>' into the 'Enter a folder title' field
    And I enter '<description>' into the 'Enter a folder description' text area
    And I click the 'Save and continue' button
    Then the '<errorMsg>' error message is displayed
    And the '<errorMsg>' error summary is displayed
Examples:
    | title        | description | errorMsg                                              |
    |              |             | Enter the folder title                                |
    | Empty Folder | description | A folder with this name already exists in this folder |
    | eRacM5cnCLj1QiZ3fLxJzfyduloK9ICFtqTlNPJirR6Q8Sx66hslAURYY2BHhXUgI8W7liWxkEnSatHEowhmuSz15vXRFUyDzsk5pBUD0DHefy0yQkxX24t9jigs6gJzjCXh4uACDxhmXX1i8Zep34rBOv8WMA1Aom2QLQ6GTMtIaUX8uDSENmiEATt1JEk8aZaBJltyk3vBxCBSNBQY7eTgk5MuKdsU8Vc9VFyRI7LDoy9SKCrZQySCWhxn1bSCwlci2YACAf7tiscsxzXUN5u42lD5MUp5vePtpyPivLZivAvCkTAwjxpF4aidt9eo0ksBspZ0dLABHKIEmyfKvFUHxCCt2KmZgtuPMci8hnFIrgKensCH7fOpLa7ycZHDN6bwNsCtKZenECl9ydsv5tbVvQQKV97fSVg8aH9Rf3qaJYWZDv661JlY0SSwdG6V0DW4zdC3jgeIIMHVhQIoHQwAwWx7sh91vHMeUv4RyuHXN8sIYH1HmsYY9wGkHyr3HzjdaxIAxfLMrTlBokqMnzcq3i6tbBGcSetopjVAFr3btacpBIiRpH1cZKAnegU9wvt4EAPXlxZZ5MgQB5hAlcj9P50ZQTV8HTz5DjJ3Eh0sEFY5tHoaQRxEBFQIDXkDlOyFRrb4h8HjzKph3Im6OuMvwQTZjvD7D5B0buXpXv6UohrWxE2JVFa4YkAH8Kz9Geg1l9QT1dhI0zGLUUAGEBiz0j8NbDBgIqSORCXR3D35ohaor2WaWFouqn2eTNlphC3XEe9uxq3LCrtJ7kTM53Sg3SPAKaoeZh48cHyIxhnuupCWJtfGQTGyHVIkEqrGievotncg8NjnSeTDY8dL5O905loEAgXUKSCCfJAyH2JTAvnMcSOu0XKIgrDhlVgc3H1TKuAVcXdcsoDumHCSCFQlJP0DyQHyYyOzMkrbKW8viOFauAghwRFUBo9yvkZ3mpFapEmLnXpUdJMXQvXxR0mmnFYxQ5mYe1YHr4PZF | description | Enter 1000 or fewer characters |
    | FolderName | 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 | Enter 4000 or fewer characters |

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
    When I enter '<description>' into the 'Enter folder description' text area
    And I click the 'Save and continue' button
    Then the '<errorMsg>' textual value is displayed
Examples: 
    | foldertitle  | description| errorMsg                                              |    
    |              |            | Enter the folder title                                |
    | Empty Folder |            | A folder with this name already exists in this folder |
    | eRacM5cnCLj1QiZ3fLxJzfyduloK9ICFtqTlNPJirR6Q8Sx66hslAURYY2BHhXUgI8W7liWxkEnSatHEowhmuSz15vXRFUyDzsk5pBUD0DHefy0yQkxX24t9jigs6gJzjCXh4uACDxhmXX1i8Zep34rBOv8WMA1Aom2QLQ6GTMtIaUX8uDSENmiEATt1JEk8aZaBJltyk3vBxCBSNBQY7eTgk5MuKdsU8Vc9VFyRI7LDoy9SKCrZQySCWhxn1bSCwlci2YACAf7tiscsxzXUN5u42lD5MUp5vePtpyPivLZivAvCkTAwjxpF4aidt9eo0ksBspZ0dLABHKIEmyfKvFUHxCCt2KmZgtuPMci8hnFIrgKensCH7fOpLa7ycZHDN6bwNsCtKZenECl9ydsv5tbVvQQKV97fSVg8aH9Rf3qaJYWZDv661JlY0SSwdG6V0DW4zdC3jgeIIMHVhQIoHQwAwWx7sh91vHMeUv4RyuHXN8sIYH1HmsYY9wGkHyr3HzjdaxIAxfLMrTlBokqMnzcq3i6tbBGcSetopjVAFr3btacpBIiRpH1cZKAnegU9wvt4EAPXlxZZ5MgQB5hAlcj9P50ZQTV8HTz5DjJ3Eh0sEFY5tHoaQRxEBFQIDXkDlOyFRrb4h8HjzKph3Im6OuMvwQTZjvD7D5B0buXpXv6UohrWxE2JVFa4YkAH8Kz9Geg1l9QT1dhI0zGLUUAGEBiz0j8NbDBgIqSORCXR3D35ohaor2WaWFouqn2eTNlphC3XEe9uxq3LCrtJ7kTM53Sg3SPAKaoeZh48cHyIxhnuupCWJtfGQTGyHVIkEqrGievotncg8NjnSeTDY8dL5O905loEAgXUKSCCfJAyH2JTAvnMcSOu0XKIgrDhlVgc3H1TKuAVcXdcsoDumHCSCFQlJP0DyQHyYyOzMkrbKW8viOFauAghwRFUBo9yvkZ3mpFapEmLnXpUdJMXQvXxR0mmnFYxQ5mYe1YHr4PZF | description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | Enter 1000 or fewer characters       |
    | FolderName   | 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 | Enter 4000 or fewer characters |


Scenario: FNHS70 - Delete folder, Cancel action
    When I click the 'DeleteFolder' link
    Then the 'DeleteFolder' header is displayed
    When I click the 'Delete Folder' link
    Then the 'Folder will be deleted' header is displayed
    And the 'Any folder contents will also be discarded. Are you sure you wish to proceed?' textual value is displayed
    When I cancel this on the open 'Delete Folder' dialog
    Then the 'DeleteFolder' header is displayed

@Core
Scenario: FNHS71 - Delete a folder
    When I click the 'DeleteFolder' link
    Then the 'DeleteFolder' header is displayed
    When I click the 'Delete Folder' link
    Then the 'Folder will be deleted' header is displayed
    And the 'Any folder contents will also be discarded. Are you sure you wish to proceed?' textual value is displayed
    When I confirm this on the open 'Delete Folder' dialog
    Then the 'Files' header is displayed
    Then the 'AutoFolderEdited' link is not displayed