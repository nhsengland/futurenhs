Feature: groupEdit
    User journeys covering Edit Group Information form functionality as a group admin

Background:
    Given I have navigated to '/'
    And I have logged in as a 'group admin' and accept the cookies
    Then the 'My Groups' header is displayed
 

Scenario: FNHS31 - Edit Group Information Page Validation
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    And the 'Group name' field contains 'Automation Editable Group'
    And the 'Strap line' field contains 'DO NOT USE - This group is reserved solely for use by our automated test scripts'
    And the 'Logo (optional)' label is displayed
    And the 'Save and close' button is displayed
    And the 'Discard changes' link is displayed


Scenario: FNHS33 - Edit Group Information
    When I select the 'Automation Editable Group' group card
    Then the 'Automation Editable Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I enter 'Automation Edited Group' into the 'Group name' field
    And I enter 'Strapline' into the 'Strap line' field
    And I click the 'Save and close' button
    Then the 'Automation Edited Group' header is displayed
    And the 'Strapline' textual value is displayed


Scenario Outline: FNHS34 - Edit Group Information error validation
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I enter '<name>' into the 'Group name' field
    And I enter '<strapline>' into the 'Strap line' field
    And I click the 'Save and close' button
    Then the 'There is a problem' textual value is displayed
    Then the '<error message>' error summary is displayed
    And the '<error message>' textual value is displayed
Examples:
    | name                    | strapline      | error message                  |
    |                         | Strap line     | Enter the group name           | 
    | [STRING: 256]           | Strapline      | Enter 255 or fewer characters  |
    | Automation Edited Group | [STRING: 256]  | Enter 255 or fewer characters  |


Scenario Outline: FNHS35 - Edit group information change logo
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed
    When I upload the '<image>' file
    Then the image file '<image>' is uploaded and ready
    When I click the 'Save and close' button
    Then the 'Automation Edited Group' header is displayed
Examples:
    | image                 |
    | /media/test.png       |
    | /media/test.jpg       |
    | /media/largeimage.png |


Scenario Outline: FNHS36 - Edit group information change logo error validation
    When I select the 'Automation Edited Group' group card
    Then the 'Automation Edited Group' header is displayed
    When I select 'Edit group information' from the actions accordion
    Then the 'Edit group information' header is displayed    
    When I upload the '<image>' file
    And I click the 'Save and close' button
    Then the '<error message>' error summary is displayed
Examples:
    | image                    | error message                          |    
    | /media/test.gif          | The image is not in an accepted format |
    | /media/toolargeimage.png | Image must be smaller than 5MB         |