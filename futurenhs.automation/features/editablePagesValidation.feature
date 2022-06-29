Feature: editablePagesValidation
    User journeys covering Validation and Navigation of Editable Pages CMS within a group

    Background:
        Given I have navigated to '/'
        And I have logged in as a 'group admin' and accept the cookies
        Then the 'My Groups' header is displayed
        When I select the 'Automation Editable Group' group card
        Then the 'Automation Editable Group' header is displayed


    Scenario: FNHS115 - Edit Mode Page Validation
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        And the 'Stop editing page' button is displayed
        And the 'Add content block' button is displayed
        And the 'Discard updates' button is not displayed
        And the 'Preview page' button is not displayed
        And the 'Publish group page' button is not displayed
        And the text block in preview mode is displayed
        And the 'Move block down, Edit, Delete' buttons are availabe on the text block
        And the 'Welcome to FutureNHS' header is displayed
        And the 'A platform from the NHS, helping the health and social care sector connect and collaborate' textual value is displayed
        And the key links block in preview mode is displayed
        Then the 'Move block up, Edit, Delete' buttons are availabe on the key links block
        And the 'FutureNHS Support' textual value is displayed 
        And the 'NHS England' textual value is displayed 
        And the 'NHS Website' textual value is displayed 


    Scenario: FNHS116 - Add Content Blocks Page Validation
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        And the 'Add content block' button is displayed
        When I click the 'Add content block' button
        Then the 'Add content block' header is displayed
        And the 'Choose a content block to add to your group homepage' textual value is displayed
        And the 'Cancel' button is displayed
        And the text block in preview mode is displayed
        Then the 'Add' button is availabe on the text block
        And the key links block in preview mode is displayed
        Then the 'Add' button is availabe on the text block


    Scenario: FNHS117 - Blocks: Add Text Block Journey Validation
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click the 'Add content block' button
        Then the 'Add content block' header is displayed
        When I click 'Add' on the text block
        Then the 'Editing group homepage' header is displayed
        And the '2nd' text block in edit mode is displayed
        And the 'Discard updates' button is displayed
        And the 'Subtitle' label is displayed
        And the 'Main text' label is displayed
        And the 'Delete' button is availabe on the '2nd' text block
        When I enter 'Text Block Form Test' into the 'Main text' text editor
        And I enter 'Test Subtitle' into the 'Subtitle' field
        Then the 'Finish editing, Delete' buttons are availabe on the '2nd' text block
        When I click 'Finish editing' on the '2nd' text block
        Then the 'Move block up, Edit, Delete' buttons are availabe on the '2nd' text block
        And the 'Test Subtitle' header is displayed
        And the 'Text Block Form Test' textual value is displayed
        And the 'Preview page' button is displayed
        When I click 'Delete' on the '2nd' text block
        Then I confirm this on the open dialog
        When I click the 'Stop editing' button
        Then the 'Edit page' button is displayed


    Scenario Outline: FNHS118 - Blocks: Text Block Form Boundary Validation
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click the 'Add content block' button
        Then the 'Add content block' header is displayed
        When I click 'Add' on the text block
        Then the 'Editing group homepage' header is displayed
        And the '2nd' text block in edit mode is displayed
        When I enter '<main text>' into the 'Main text' text editor
        And I enter '<subtitle>' into the 'Subtitle' field
        When I click 'Finish editing' on the '2nd' text block
        Then the '<error>' error message is displayed
        When I click the 'Discard updates' button
        Then I confirm this on the open dialog
        Then the 'Edit page' button is displayed
        Examples:
            | main text              | subtitle      | error                          |
            |                        | subtitle      | Enter the main text            |
            | subtitle boundary test | [STRING: 256] | Enter 255 or fewer characters  |
            | [STRING: 4001]         | subtitle      | Enter 4000 or fewer characters |


    Scenario: FNHS119 - Blocks: Key Links Block
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click the 'Add content block' button
        Then the 'Add content block' header is displayed
        When I click 'Add' on the key links block
        Then the 'Editing group homepage' header is displayed
        And the '2nd' key links block in edit mode is displayed
        And the 'Discard updates' button is displayed
        And the 'Subtitle' label is displayed
        And the 'Add a link' button is displayed
        And the 'Delete' button is availabe on the '2nd' key links block
        When I enter 'Test Subtitle' into the 'Subtitle' field
        Then the 'Finish editing, Delete' buttons are availabe on the '2nd' key links block
        When I click the 'Add a link' button
        Then the 'Link title' label is displayed
        And the 'Link' label is displayed
        And the 'Delete link' button is displayed
        When I enter 'Test Link' into the 'Link title' field
        And I enter 'https://www.google.com/' into the 'Link' field
        Then the 'Finish editing, Delete' buttons are availabe on the '2nd' key links block
        When I click 'Finish editing' on the '2nd' key links block
        Then the 'Move block up, Edit, Delete' buttons are availabe on the '2nd' key links block
        And the 'Test Subtitle' header is displayed
        And the 'Test Link' textual value is displayed
        And the 'Preview page' button is displayed
        When I click 'Delete' on the '2nd' key links block
        Then I confirm this on the open dialog
        When I click the 'Stop editing' button
        Then the 'Edit page' button is displayed


    Scenario Outline: FNHS120 - Blocks: Key Links Block Form Validation
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click the 'Add content block' button
        Then the 'Add content block' header is displayed
        When I click 'Add' on the key links block
        Then the 'Editing group homepage' header is displayed
        And the '2nd' key links block in edit mode is displayed
        When I click the 'Add a link' button
        Then the 'Link title' label is displayed
        When I enter '<subtitle>' into the 'Subtitle' field
        And I enter '<link title>' into the 'Link title' field
        And I enter '<link>' into the 'Link' field
        And I click the 'Publish group page' button
        Then the '<error>' error message is displayed
        Examples:
            | subtitle      | link title   | link       | error                         |
            |               | Link Title   | Blank Link | Enter the subtitle            |
            | Subtitle      |              | Blank Link | Enter the link title          |
            | Subtitle      | Link Title   |            | Enter the link                |
            | [STRING: 256] | Link Title   | Blank Link | Enter 255 or fewer characters |
            | Subtitle      | [STRING: 31] | Blank Link | Enter 30 or fewer characters  |


    Scenario: FNHS121 - Preview Changes
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click 'Edit' on the text block
        Then the 'Subtitle' field contains 'Welcome to FutureNHS'
        When I enter 'Welcome to my edited homepage' into the 'Subtitle' field
        And I click 'Finish editing' on the text block
        Then the 'Welcome to my edited homepage' header is displayed
        And the 'Discard updates' button is displayed
        And the 'Preview page' button is displayed
        And the 'Publish group page' button is displayed
        When I click the 'Preview page' button
        Then the 'You are previewing the group homepage in editing mode' textual value is displayed
        And the 'Edit page' button is displayed
        And the 'Publish group page' button is displayed
        And the 'Welcome to my edited homepage' header is displayed
        When I click the 'Edit page' button
        Then the 'Editing group homepage' header is displayed
        When I click the 'Discard updates' button
        Then the 'Edit page' button is displayed
