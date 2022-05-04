Feature: editablePagesValidation
    User journeys covering Validation and Navigation of Editable Pages CMS within a group 

Background:    
    Given I have navigated to '/'
    And I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed


Scenario Outline: FNHS114 - Edit Page Permissions Validation
    Given I have logged off as the current user
    And I have logged in as a '<user>'
    Then the 'Automation Editable Group' header is displayed
    And the 'You are a Group Admin of this page. Please click edit to switch to edit mode' textual value <visibility> displayed
    And the 'Edit page' button <visibility> displayed
Examples:
    | user         | visibility |
    | group admin  | is         |
    | admin        | is         |
    | user         | is not     |


Scenario: FNHS115 - Edit Mode Page Validation
    When I click the 'Edit page' button
    Then the 'Editing group homepage' header is displayed
    And the 'Stop editing page' button is displayed
    And the 'Add content block' link is displayed
    And the 'Discard changes' button is not displayed
    And the 'Preview page' button is not displayed
    And the 'Publish group page' button is not displayed
    # BLOCK DISPLAYED
    # BLOCK DISPLAYED

    
Scenario: FNHS116 - Add Content Blocks Page Validation
    # CLICK ADD CONTENT BLOCKS
    # HEADER VALIDATION
    # LINKS/BUTTON VALIDATION
    # VALIDATE BLOCKS IN PREVIEW MODE
Scenario: FNHS117 - Blocks: Text Block
    # BLOCK EXISTS
    # FORM FIELDS CONTAIN X
Scenario Outline: FNHS118 - Blocks: Text Block Form Validation
    # BLOCK EXISTS
    # FIELD VALIDATION
    # VALIDATE ERRORS
Examples:
    | Header 1 | Header 2 | Header 3 |
    | Value 1  | Value 2  | Value 3  |
Scenario: FNHS119 - Blocks: Key Links Block
    # BLOCK EXISTS
    # FORM FIELDS CONTAIN X
Scenario Outline: FNHS120 - Blocks: Key Links Block Form Validation
    # BLOCK EXISTS
    # FIELD VALIDATION
    # VALIDATE ERRORS
Examples:
    | Header 1 | Header 2 | Header 3 |
    | Value 1  | Value 2  | Value 3  |
Scenario: FNHS121 - Publish Changes
    # PUBLISH NOT EXISTING
    # MAKE CHANGE
    # PUBLISH EXISTING
    # CLICK PUBLISH
    # VALIDATE CHANGE